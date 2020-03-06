using Newtonsoft.Json;

namespace WozAlboPrzewoz
{
    public class Station : JPacket
    {
        [JsonProperty("ID")]
        public int Id { get; set; }
        [JsonProperty("N")]
        public string Name { get; set; }
        [JsonProperty("P")]
        public int P { get; set; }
        [JsonProperty("M")]
        public string M { get; set; }
        [JsonProperty("I")]
        public string I { get; set; }
        [JsonProperty("D")]
        public double D { get; set; }
        [JsonProperty("S")]
        public double S { get; set; }
        [JsonProperty("T")]
        public string T { get; set; }
        [JsonProperty("W")]
        public int W { get; set; }
        [JsonProperty("NZ")]
        public string Nz { get; set; }

        public Station()
        {

        }
    }
}
