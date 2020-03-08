using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/setting_licenses", Theme = "@style/AppTheme", MainLauncher = false)]
    public class LicensesActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_licenses);

            var mToolbar = (Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            var mRecyclerView = (RecyclerView)FindViewById(Resource.Id.recyclerViewLicenses);
            mRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

            var mLicenseData = new List<License>();

            var licenses = Resources.GetStringArray(Resource.Array.licenses);

            foreach (var license in licenses)
            {
                var split = license.Split('#');
                mLicenseData.Add(new License(split[0], split[1]));
            }

            var mLicensesAdapter = new LicensesAdapter(mLicenseData);
            mRecyclerView.SetAdapter(mLicensesAdapter);
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }
    }
}