using Android.Graphics;
using System.Collections.Generic;

namespace WozAlboPrzewoz
{
    public class CarrierInfo
    {
        private static Dictionary<string, Color> colors = new Dictionary<string, Color>()
        {
            {"KM",  Color.ParseColor("#009624") },
            {"SKM", Color.ParseColor("#a30000") },
            {"IC",  Color.ParseColor("#0039cb") }
        };

        public static bool GetCarrierColor(string carrier, out Color c)
        {
            return colors.TryGetValue(carrier, out c);

        }
    }
}