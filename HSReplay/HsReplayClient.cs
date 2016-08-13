using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HSReplay.Responses;
using Newtonsoft.Json;

namespace HSReplay
{
	/// <summary>
	///     API client for hsreplay.net
	/// </summary>
	public class HsReplayClient
	{
		private const string ClaimAccountUrl = "https://hsreplay.net/api/v1/claim_account/";
		private const string TokensUrl = "https://hsreplay.net/api/v1/tokens/";
		private const string UploadRequestUrl = "https://upload.hsreplay.net/api/v1/replay/upload/request";
		private const string WebUrl = "https://hsreplay.net";

		private readonly string _apiKey;
		private readonly bool _testData;
		private readonly WebClient _webClient;

		/// <summary>
		/// </summary>
		/// <param name="apiKey">hsreplay.net API key</param>
		/// <param name="testData">Set to true when not uploading actual user data.</param>
		public HsReplayClient(string apiKey, bool testData = false)
		{
			_apiKey = apiKey;
			_webClient = new WebClient();
			_testData = testData;
		}

		private Header GetAuthHeader(string token) => new Header("Authorization", $"Token {token}");

		private Header ApiHeader => new Header("X-Api-Key", _apiKey);

		/// <summary>
		///     Creates a new upload token
		/// </summary>
		/// <returns>Created upload token</returns>
		public async Task<string> CreateUploadToken()
		{
			var content = _testData ? JsonConvert.SerializeObject(new {test_data = true}) : null;
			var response = await _webClient.PostJsonAsync(TokensUrl, content, false, ApiHeader);
			using(var responseStream = response.GetResponseStream())
			using(var reader = new StreamReader(responseStream))
			{
				var token = JsonConvert.DeserializeObject<UploadToken>(reader.ReadToEnd());
				if(string.IsNullOrEmpty(token.Key))
					throw new Exception("Reponse contained no upload-token.");
				return token.Key;
			}
		}

		/// <summary>
		///     Returns a url, which allows for the token to be claimed.
		///     The user has to open this url in a browser for the claiming to complete.
		/// </summary>
		/// <param name="token">Auth token</param>
		/// <returns>Url for account claiming.</returns>
		public async Task<string> GetClaimAccountUrl(string token)
		{
			var response = await _webClient.PostAsync(ClaimAccountUrl, string.Empty, false, ApiHeader, GetAuthHeader(token));
			using(var responseStream = response.GetResponseStream())
			using(var reader = new StreamReader(responseStream))
				return JsonConvert.DeserializeObject<AccountClaim>(reader.ReadToEnd()).Url;
		}

		/// <summary>
		///     Returns the status of a given auth token.
		/// </summary>
		/// <param name="token">Auth token</param>
		/// <returns>Status of given auth token.</returns>
		public async Task<AccountStauts> GetAccountStatus(string token)
		{
			var response = await _webClient.GetAsync($"{TokensUrl}{token}", ApiHeader);
			using(var responseStream = response.GetResponseStream())
			using(var reader = new StreamReader(responseStream))
				return JsonConvert.DeserializeObject<AccountStauts>(reader.ReadToEnd());
		}

		/// <summary>
		///     Creates a new log upload request. This request is then passed to UploadLog().
		///     The request contains a ShortId, which will be the game URL on the website.
		///     Use LogValidation.LogValidator.Validate to ensure the log is valid before making this request.
		/// </summary>
		/// <param name="metaData">Meta data about the match.</param>
		/// <param name="token">Auth token</param>
		/// <returns>Upload request, containing the future game URL</returns>
		public async Task<LogUploadRequest> CreateUploadRequest(UploadMetaData metaData, string token)
		{
			var content = JsonConvert.SerializeObject(metaData);
			var response = await _webClient.PostAsync(UploadRequestUrl, content, true, ApiHeader, GetAuthHeader(token));
			using(var responseStream = response.GetResponseStream())
			using(var reader = new StreamReader(responseStream))
			{
				var reponse = reader.ReadToEnd();
				return JsonConvert.DeserializeObject<LogUploadRequest>(reponse);
			}
		}

		/// <summary>
		///     Uploads the given log.
		///     The URL to of the replay is included in the LogUploadRequest object.
		/// </summary>
		/// <param name="request">Created by CreateUploadRequest()</param>
		/// <param name="log">Log to be uploaded</param>
		/// <returns></returns>
		public async Task UploadLog(LogUploadRequest request, IEnumerable<string> log) => await UploadLog(request.PutUrl, log);

		/// <summary>
		///     Uploads the given log.
		///     The URL to of the replay is included in the LogUploadRequest object.
		/// </summary>
		/// <param name="putUrl">Found in LogUploadRequest.PutUrl</param>
		/// <param name="log">Log to be uploaded</param>
		/// <returns></returns>
		public async Task UploadLog(string putUrl, IEnumerable<string> log)
			=> await _webClient.PutAsync(putUrl, string.Join(Environment.NewLine, log), true);
	}
}