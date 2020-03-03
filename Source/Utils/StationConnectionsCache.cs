using System;

namespace WozAlboPrzewoz.Source.Utils
{
    public class StationConnectionsCache
    {
        public int StationId { get; set; }
        public DateTime Time { get; set; }
        public int PastConnCount { get; set; }
        public int FutureConnCount { get; set; }
    }
}