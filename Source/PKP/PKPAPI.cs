﻿using Newtonsoft.Json;
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

        public static TranslationPhrase[] GetTranslations()
        {
            DictionaryRequest packet = new DictionaryRequest("E", 0.0, "PL", "1");

            string parameters = EncryptionHelpers.Encrypt(packet.Serialize());
            HttpStatusCode status;
            string response = HttpUtils.SendGETRequest($"https://portalpasazera.pl/API/PobierzSlowniki?parametry={parameters}", out status);

            JToken token = JsonConvert.DeserializeObject<JToken>(response);
            Console.WriteLine(response);
            JArray arr = JsonConvert.DeserializeObject<JArray>(token[0]["SD"].ToString());

            return arr.ToObject<TranslationPhrase[]>();
        }

        public static TrainConnection[] GetStationTimetable(int sid, DateTime time, int k = 1, int s = 0)
        {
            StationTimetableRequest packet = new StationTimetableRequest(sid, time.ToOADate(), 1, k, s, 0.0);
            string parameters = EncryptionHelpers.Encrypt(packet.Serialize());
            HttpStatusCode status;
            string response = HttpUtils.SendGETRequest($"https://portalpasazera.pl/API/RozkladNaStacji?parametry={parameters}", out status);
            JArray arr = JsonConvert.DeserializeObject<JArray>(response);

            return arr.ToObject<TrainConnection[]>();
        }

        public static string GetConnectionDetails(ConnectionDetailsRequest packet)
        {
            //[{"R":2020,"Z":130026500,"DK":43871.0,"SPNNT":6,"SKNNT":25}]

            string parameters = EncryptionHelpers.Encrypt(packet.Serialize());
            HttpStatusCode status;
            string response = HttpUtils.SendGETRequest($"https://portalpasazera.pl/API/SzczegolyPolaczeniaDane?parametry={parameters}&v=kb", out status);
            return response;
        }

        public static string GetConnectionRoute(ConnectionDetailsRequest packet, bool full = false)
        {
            //[{"R":2020,"Z":130026500,"DK":43871.0,"SPNNT":6,"SKNNT":25}]
            if (full)
            {
                packet.spnnt = short.MinValue;
                packet.sknnt = short.MaxValue;
            }

            string parameters = EncryptionHelpers.Encrypt(packet.Serialize());
            HttpStatusCode status;
            string response = HttpUtils.SendGETRequest($"https://portalpasazera.pl/API/SzczegolyPolaczeniaTrasa?parametry={parameters}&v=kb", out status);
            return response;
        }
    }
}
