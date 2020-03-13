using Newtonsoft.Json;

namespace WozAlboPrzewoz
{
    public class TranslationPhrase : JPacket
    {
        [JsonProperty("D")]
        public string D { get; set; }
        [JsonProperty("W")]
        public string W { get; set; }
        [JsonProperty("I")]
        public string I { get; set; }
        [JsonProperty("N")]
        public string N { get; set; }
        [JsonProperty("T")]
        public string T { get; set; }

        public TranslationPhrase()
        {

        }
    }
}