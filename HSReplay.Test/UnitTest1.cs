using System;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HSReplay.Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void KeyGen_AccountStatus_Upload()
		{
			var client = new HsReplayClient("89c8bbc1-474a-4b1b-91b5-2a116d19df7a", "HSReplay-API-Test/1.0", true);

			var token = client.CreateUploadToken().Result;
			Assert.IsFalse(string.IsNullOrEmpty(token), "string.IsNullOrEmpty(key)");

			var account = client.GetAccountStatus(token).Result;
			Assert.AreEqual(token, account.Key, "Key matches sent token");
			Assert.IsTrue(account.TestData, "account.TestData");
			Assert.IsNull(account.User);

			var metaData = new UploadMetaData()
			{
				TestData = true,
				HearthstoneBuild = 1,
				MatchStart = DateTime.Now.ToString("o")
			};
			var uploadEvent = client.CreateUploadRequest(metaData, token).Result;
			Assert.IsFalse(string.IsNullOrEmpty(uploadEvent.PutUrl));
			Assert.IsFalse(string.IsNullOrEmpty(uploadEvent.ShortId));
			Assert.IsFalse(string.IsNullOrEmpty(uploadEvent.ReplayUrl));

			string[] log;
			using(var sr = new StreamReader("TestData/Power.log"))
				log = sr.ReadToEnd().Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
			client.UploadLog(uploadEvent, log).Wait();
		}

		[TestMethod]
		public void ClientConfig_InvalidUrl_Error()
		{
			var webException = false;
			var config = new ClientConfig() { TokensUrl  = "https://hsreplay.net/api/v0/tokens/" };
			var client = new HsReplayClient("89c8bbc1-474a-4b1b-91b5-2a116d19df7a", "HSReplay-API-Test/1.0", true, config);
			try
			{
				var token = client.CreateUploadToken().Result;
			}
			catch(AggregateException aggregateException)
			{
				Assert.IsInstanceOfType(aggregateException.InnerExceptions[0], typeof(WebException));
				webException = true;
			}
			Assert.IsTrue(webException);
		}
	}
}
