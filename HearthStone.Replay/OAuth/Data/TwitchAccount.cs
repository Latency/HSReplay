using Newtonsoft.Json;


namespace HearthStone.Replay.OAuth.Data
{
	public class TwitchAccount
	{
		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }
	}
}
