using Newtonsoft.Json;

namespace WozAlboPrzewoz
{
    public class TrainConnection : JPacket
    {
        [JsonProperty("R")]
        public int TimetableYear { get; set; }
        [JsonProperty("Z")]
        public int Z { get; set; }
        [JsonProperty("DK")]
        public int Dk { get; set; }
        [JsonProperty("P")]
        public string PlatformStart { get; set; }
        [JsonProperty("T")]
        public string TrackStart { get; set; }
        [JsonProperty("G")]
        public double TimeDeparture { get; set; }
        [JsonProperty("GO")]
        public int DelayStart { get; set; }
        [JsonProperty("PS")]
        public string Carrier { get; set; }
        [JsonProperty("KH")]
        public string Line { get; set; }
        [JsonProperty("PNM")]
        public string TrainNumber { get; set; }
        [JsonProperty("PN")]
        public string TrainName { get; set; }
        [JsonProperty("RP")]
        public string StationStart { get; set; }
        [JsonProperty("RPL")]
        public string CountryStart { get; set; }
        [JsonProperty("RPP")]
        public string Platform { get; set; }
        [JsonProperty("RPT")]
        public string Track { get; set; }
        [JsonProperty("RPG")]
        public double TimeDepartureStart { get; set; }
        [JsonProperty("RPO")]
        public int DelayEnd { get; set; }
        [JsonProperty("RK")]
        public string StationEnd { get; set; }
        [JsonProperty("RKL")]
        public string CountryEnd { get; set; }
        [JsonProperty("RKP")]
        public string PlatoformEnd { get; set; }
        [JsonProperty("RKT")]
        public string TrackEnd { get; set; }
        [JsonProperty("RKG")]
        public double TimeArrivalEnd { get; set; }
        [JsonProperty("RKO")]
        public int Rko { get; set; }
        [JsonProperty("SPNNT")]
        public int Spnnt { get; set; }
        [JsonProperty("sknnt")]
        public int Sknnt { get; set; }
        [JsonProperty("UP")]
        public string Up { get; set; }

        public TrainConnection()
        {

        }
    }
}
