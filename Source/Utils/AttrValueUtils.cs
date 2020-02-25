using Android.App;
using Android.Graphics;
using Android.Util;

namespace WozAlboPrzewoz
{
    public class AttrValueUtils
    {
        public static Color GetColor(int attr)
        {
            var arr = Application.Context.Theme.ObtainStyledAttributes(Resource.Style.AppTheme, new int[] { attr });
            return arr.GetColor(0, 0);
        }
    }
}