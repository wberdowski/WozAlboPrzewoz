using Newtonsoft.Json;

namespace WozAlboPrzewoz
{
    public class DictionaryRequest : JPacket
    {
        [JsonProperty("SID")]
        public string sid;
        [JsonProperty("OZ")]
        public double oz;
        [JsonProperty("SD")]
        public string sd;
        [JsonProperty("API")]
        public string api;

        public DictionaryRequest()
        {

        }

        public DictionaryRequest(string sid, double oz, string sd, string api)
        {
            this.sid = sid;
            this.oz = oz;
            this.sd = sd;
            this.api = api;
        }

        public override string Serialize()
        {
            return "[" + base.Serialize() + "]";
        }
    }
}
