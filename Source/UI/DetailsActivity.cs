using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Xamarin.Forms;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class DetailsActivity : AppCompatActivity
    {
        private int r;
        private int z;
        private double dk;
        private int spnnt;
        private int sknnt;
        private RecyclerView mRecyclerDetails;
        private List<StationSchedule> mConnectionDetails;
        private DetailsAdapter mDetailsAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_connection_details);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            r = Intent.GetIntExtra("r", 0);
            z = Intent.GetIntExtra("z", 0);
            dk = Intent.GetDoubleExtra("dk", 0);
            spnnt = Intent.GetIntExtra("spnnt", 0);
            sknnt = Intent.GetIntExtra("sknnt", 0);

            Title = r + " " + z + " " + dk + " " + spnnt + " " + sknnt;

            //
            //  Recycler details
            //

            mRecyclerDetails = (RecyclerView)FindViewById(Resource.Id.recyclerViewDetails);

            var mLayoutManager = new LinearLayoutManager(this);
            mRecyclerDetails.SetLayoutManager(mLayoutManager);

            mConnectionDetails = new List<StationSchedule>();

            mDetailsAdapter = new DetailsAdapter(mConnectionDetails);
            mDetailsAdapter.ItemClick += MDetailsAdapter_ItemClick;
            mRecyclerDetails.SetAdapter(mDetailsAdapter);

            new System.Threading.Thread(() =>
            {
                var details = PKPAPI.GetConnectionRoute(r, z, dk, spnnt, sknnt);
                Device.BeginInvokeOnMainThread(() =>
                {
                    var dt = new ConnectionDetails()
                    .FromJson(details);

                    mConnectionDetails.AddRange(dt.Stations);

                    mDetailsAdapter.NotifyDataSetChanged();
                });
            }).Start();
        }

        private void MDetailsAdapter_ItemClick(object sender, DetailsAdapterClickEventArgs e)
        {

        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }
    }
}