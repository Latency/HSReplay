using Newtonsoft.Json;


namespace HearthStone.Replay
{
	public class CardData
	{
		[JsonProperty("card")]
		public string CardId { get; set; }

		[JsonProperty("premium", DefaultValueHandling = DefaultValueHandling.Ignore)]
		public bool Premium { get; set; }
	}
}