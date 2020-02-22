using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class DetailsActivity : AppCompatActivity
    {
        private RecyclerView mRecyclerDetails;
        private List<StationSchedule> mConnectionDetails;
        private DetailsAdapter mDetailsAdapter;
        private TrainConnection mTrainConnection;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_connection_details);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            mTrainConnection = JsonConvert.DeserializeObject<TrainConnection>(Intent.GetStringExtra("train_conn"));

            Title = $"{mTrainConnection.trainNumber} {mTrainConnection.stationEnd}";

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
                var req = new ConnectionDetailsRequest(
                    mTrainConnection.timetableYear,
                    mTrainConnection.z,
                    mTrainConnection.dk,
                    mTrainConnection.spnnt,
                    mTrainConnection.sknnt
                    );

                var details = PKPAPI.GetConnectionRoute(req);
                RunOnUiThread(() =>
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