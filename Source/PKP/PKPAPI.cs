using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace WozAlboPrzewoz
{
    public class PKPAPI
    {
        public static Station[] GetStations()
        {
            DictionaryRequest packet = new DictionaryRequest("S", 0.0, "", "1");

            string parameters = EncryptionHelpers.Encrypt(packet.Serialize());
            HttpStatusCode status;
            string response = HttpUtils.SendGETRequest($"https://portalpasazera.pl/API/PobierzSlowniki?parametry={parameters}", out status);

            JToken token = JsonConvert.DeserializeObject<JToken>(response);
            JArray arr = JsonConvert.DeserializeObject<JArray>(token[0]["SD"].ToString());

            return arr.ToObject<Station[]>();
        }

        public static TrainConnection[] GetStationTimetable(int sid, DateTime time)
        {
            StationTimetableRequest packet = new StationTimetableRequest(sid, time.ToOADate(), 1, 1, 0, 0.0);
            string parameters = EncryptionHelpers.Encrypt(packet.Serialize());
            HttpStatusCode status;
            string response = HttpUtils.SendGETRequest($"https://portalpasazera.pl/API/RozkladNaStacji?parametry={parameters}", out status);
            JArray arr = JsonConvert.DeserializeObject<JArray>(response);

            return arr.ToObject<TrainConnection[]>();
        }

        public static string GetConnectionDetails(int r, int z, double dk, int spnnt, int sknnt)
        {
            //[{"R":2020,"Z":130026500,"DK":43871.0,"SPNNT":6,"SKNNT":25}]

            ConnectionDetailsRequest packet = new ConnectionDetailsRequest(r, z, dk, spnnt, sknnt);
            string parameters = EncryptionHelpers.Encrypt(packet.Serialize());
            HttpStatusCode status;
            string response = HttpUtils.SendGETRequest($"https://portalpasazera.pl/API/SzczegolyPolaczeniaDane?parametry={parameters}&v=kb", out status);
            return response;
        }

        public static string GetConnectionRoute(int r, int z, double dk, int spnnt, int sknnt)
        {
            //[{"R":2020,"Z":130026500,"DK":43871.0,"SPNNT":6,"SKNNT":25}]

            ConnectionDetailsRequest packet = new ConnectionDetailsRequest(r, z, dk, spnnt, sknnt);
            string parameters = EncryptionHelpers.Encrypt(packet.Serialize());
            HttpStatusCode status;
            string response = HttpUtils.SendGETRequest($"https://portalpasazera.pl/API/SzczegolyPolaczeniaTrasa?parametry={parameters}&v=kb", out status);
            return response;
        }
    }
}
