using Newtonsoft.Json;

namespace WozAlboPrzewoz
{
    public class Station : JPacket
    {
        [JsonProperty("ID")]
        public int id;
        [JsonProperty("N")]
        public string nazwa;
        [JsonProperty("P")]
        public int p;
        [JsonProperty("M")]
        public string m;
        [JsonProperty("I")]
        public string i;
        [JsonProperty("D")]
        public double d;
        [JsonProperty("S")]
        public double s;
        [JsonProperty("T")]
        public string t;
        [JsonProperty("W")]
        public int w;
        [JsonProperty("NZ")]
        public string nz;

        public Station()
        {

        }
    }
}
