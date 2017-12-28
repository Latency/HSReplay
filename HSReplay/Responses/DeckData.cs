using Newtonsoft.Json;

namespace HSReplay.Responses
{
	public class DeckData
	{
		[JsonProperty("archetype_id")]
		public int Archetype { get; set; }
		
		[JsonProperty("shortid")]
		public string ShortId { get; set; }
		
		[JsonProperty("cards")]
		public int[] Cards { get; set; }

		[JsonProperty("heroes")]
		public int[] Heroes { get; set; }

		[JsonProperty("format")]
		public int Format { get; set; }
	}
}