using Android.Support.Design.Widget;
using Android.Views;

namespace WozAlboPrzewoz
{
    public class SnackbarUtils
    {
        public static Snackbar MakeWithMargins(View view, string text, int duration)
        {
            var snackbar = Snackbar.Make(view, text, duration);
            var snackBarView = snackbar.View;
            AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams param = (AndroidX.CoordinatorLayout.Widget.CoordinatorLayout.LayoutParams)snackBarView.LayoutParameters;

            param.SetMargins(param.LeftMargin + 8,
                        param.TopMargin,
                        param.RightMargin + 8,
                        param.BottomMargin + 8);

            snackBarView.LayoutParameters = param;
            return snackbar;
        }
    }
}