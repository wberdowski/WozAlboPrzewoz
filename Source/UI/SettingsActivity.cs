using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/action_settings", Theme = "@style/AppTheme", MainLauncher = false)]
    public class SettingsActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_settings);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.settingsView, new SettingsFragment())
                .Commit();
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }

    }
}