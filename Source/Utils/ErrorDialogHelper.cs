using Android.Content;
using System;
using System.Net;

namespace WozAlboPrzewoz
{
    public class ErrorDialogHelper
    {
        public static void ShowServerErrorDialog(Context context, HttpStatusCode status, EventHandler<DialogClickEventArgs> onTryAgain)
        {
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context);
            dialog.SetTitle(Resource.String.dialog_server_error_title);
            dialog.SetMessage(context.GetString(Resource.String.dialog_server_error_body, (int)status, status.ToString()));
            dialog.SetNeutralButton(Resource.String.dialog_button_try_again, onTryAgain);
            dialog.SetPositiveButton(Resource.String.dialog_button_ok, (s, e) =>
            {

            });
            dialog.Show();
        }

        public static void ShowConnectionErrorDialog(Context context, EventHandler<DialogClickEventArgs> onTryAgain)
        {
            var dialog = new Android.Support.V7.App.AlertDialog.Builder(context);
            dialog.SetTitle(Resource.String.dialog_connection_error_title);
            dialog.SetMessage(Resource.String.dialog_connection_error_body);
            dialog.SetNeutralButton(Resource.String.dialog_button_try_again, onTryAgain);
            dialog.SetPositiveButton(Resource.String.dialog_button_ok, (s, e) =>
            {

            });
            dialog.Show();
        }
    }
}