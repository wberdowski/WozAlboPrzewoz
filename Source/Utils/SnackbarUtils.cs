using Android.Support.Design.Widget;
using Android.Views;
using AndroidX.Core.View;

namespace WozAlboPrzewoz
{
    public class SnackbarUtils
    {
        public static Snackbar MakeWithMargins(View view, string text, int duration)
        {
            var snack = Snackbar.Make(view, text, duration);

            ViewGroup.MarginLayoutParams parameters = (ViewGroup.MarginLayoutParams)snack.View.LayoutParameters;
            parameters.SetMargins(16, 16, 16, 16);

            snack.View.SetBackgroundResource(Resource.Drawable.snackbar_background);

            ViewCompat.SetElevation(snack.View, 6f);

            return snack;
        }
    }
}