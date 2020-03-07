using System;

namespace WozAlboPrzewoz
{
    public static class MathUtils
    {
        public static double Clamp(this double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        public static int Clamp(this int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}