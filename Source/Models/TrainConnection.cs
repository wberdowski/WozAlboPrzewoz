using Newtonsoft.Json;

namespace WozAlboPrzewoz
{
    public class TrainConnection : JPacket
    {
        [JsonProperty("R")]
        public int timetableYear;
        [JsonProperty("Z")]
        public int z;
        [JsonProperty("DK")]
        public int dk;
        [JsonProperty("P")]
        public string platformStart;
        [JsonProperty("T")]
        public string trackStart;
        [JsonProperty("G")]
        public double timeDeparture;
        [JsonProperty("GO")]
        public int delay;
        [JsonProperty("PS")]
        public string carrier;
        [JsonProperty("KH")]
        public string line;
        [JsonProperty("PNM")]
        public string trainNumber;
        [JsonProperty("PN")]
        public string trainName;
        [JsonProperty("RP")]
        public string stationStart;
        [JsonProperty("RPL")]
        public string countryStart;
        [JsonProperty("RPP")]
        public string platform;
        [JsonProperty("RPT")]
        public string track;
        [JsonProperty("RPG")]
        public double timeDepartureStart;
        [JsonProperty("RPO")]
        public int rpo;
        [JsonProperty("RK")]
        public string stationEnd;
        [JsonProperty("RKL")]
        public string countryEnd;
        [JsonProperty("RKP")]
        public string platoformEnd;
        [JsonProperty("RKT")]
        public string trackEnd;
        [JsonProperty("RKG")]
        public double timeArrivalEnd;
        [JsonProperty("RKO")]
        public int rko;
        [JsonProperty("SPNNT")]
        public int spnnt;
        [JsonProperty("sknnt")]
        public int sknnt;
        [JsonProperty("UP")]
        public string up;

        public TrainConnection()
        {

        }
    }
}
