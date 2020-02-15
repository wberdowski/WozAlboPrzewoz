using Newtonsoft.Json;

namespace WozAlboPrzewoz
{
    public class StationTimetableRequest : JPacket
    {
        [JsonProperty("SID")]
        public int sid;
        [JsonProperty("D")]
        public double d;
        [JsonProperty("O")]
        public int o;
        [JsonProperty("K")]
        public int k;
        [JsonProperty("S")]
        public int s;
        [JsonProperty("C")]
        public double c;

        public StationTimetableRequest()
        {

        }

        public StationTimetableRequest(int sid, double d, int o, int k, int s, double c)
        {
            this.sid = sid;
            this.d = d;
            this.o = o;
            this.k = k;
            this.s = s;
            this.c = c;
        }
    }
}
