using Newtonsoft.Json;


namespace HearthStone.Replay.OAuth.Data
{
	public class TwitchUserData
	{
		[JsonProperty("display_name")]
		public string DisplayName { get; set; }
	}
}
