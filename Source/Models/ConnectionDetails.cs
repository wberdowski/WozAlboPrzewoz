using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WozAlboPrzewoz
{
    public class ConnectionDetails : IJsonModel<ConnectionDetails>
    {
        public List<StationSchedule> Stations { get; set; }

        public ConnectionDetails()
        {
            Stations = new List<StationSchedule>();
        }

        public ConnectionDetails FromJson(string str)
        {
            var json = JsonConvert.DeserializeObject<JToken>(str);
            foreach (var item in json["T"][0]["S"])
            {
                var s = new StationSchedule();
                s.Name = item["N"].ToString();
                s.ArrivalTime = DateTime.FromOADate(item["PPDT"].Value<double>());
                s.DepartureTime = DateTime.FromOADate(item["PODT"].Value<double>());
                s.Platform = item["P0"].ToString();
                s.Track = item["T0"].ToString();
                Stations.Add(s);
            }

            return this;
        }
    }
}