using Newtonsoft.Json;

namespace WozAlboPrzewoz
{
    public class ConnectionDetailsRequest : JPacket
    {
        [JsonProperty("R")]
        public int r;
        [JsonProperty("Z")]
        public int z;
        [JsonProperty("DK")]
        public double dk;
        [JsonProperty("SPNNT")]
        public short spnnt;
        [JsonProperty("SKNNT")]
        public short sknnt;

        public ConnectionDetailsRequest()
        {

        }

        public ConnectionDetailsRequest(int r, int z, double dk, short spnnt, short sknnt)
        {
            this.r = r;
            this.z = z;
            this.dk = dk;
            this.spnnt = spnnt;
            this.sknnt = sknnt;
        }

        public override string Serialize()
        {
            return "[" + base.Serialize() + "]";
        }
    }
}
