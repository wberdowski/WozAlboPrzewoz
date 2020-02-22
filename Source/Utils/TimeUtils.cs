using System;

namespace WozAlboPrzewoz
{
    public class TimeUtils
    {
        public static DateTime DiscardSeconds(DateTime time)
        {
            return time.AddSeconds(-time.Second);
        }
    }
}