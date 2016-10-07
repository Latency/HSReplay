namespace HSReplay
{
	public class ClientConfig
	{
		private const string DefaultClaimAccountUrl = "https://hsreplay.net/api/v1/claim_account/";
		private const string DefaultTokensUrl = "https://hsreplay.net/api/v1/tokens/";
		private const string DefaultUploadRequestUrl = "https://upload.hsreplay.net/api/v1/replay/upload/request/";
		private const string DefaultUploadPackUrl = "https://hsreplay.net/api/v1/packs/";
		public string ClaimAccountUrl { get; set; } = DefaultClaimAccountUrl;
		public string TokensUrl { get; set; } = DefaultTokensUrl;
		public string UploadRequestUrl { get; set; } = DefaultUploadRequestUrl;
		public string UploadPackUrl { get; set; } = DefaultUploadPackUrl;
	}
}
