using Android.App;
using Android.OS;
using Android.Widget;
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

            var mToolbar = (AndroidX.AppCompat.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);

            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            var mToolbarText = (TextView)mToolbar.FindViewById(Resource.Id.toolbar_title);
            mToolbarText.SetText(Resource.String.action_settings);

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