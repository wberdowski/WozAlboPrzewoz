using System;

namespace WozAlboPrzewoz
{
    public class StationSchedule
    {
        public string Name { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Platform { get; set; }
        public string Track { get; set; }
        public int Delay { get; set; }
    }
}