using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Java.Lang;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class DetailsActivity : AppCompatActivity
    {
        private RecyclerView mRecyclerDetails;
        private List<StationSchedule> mConnectionDetails;
        private DetailsAdapter mDetailsAdapter;
        private TrainConnection mTrainConnection;
        private SwipeRefreshLayout mSwipeRefreshLayout;
        private TickReceiver mTickReceiver;
        private ConnectionsAdapterViewHolder vh;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_details);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            mTrainConnection = JsonConvert.DeserializeObject<TrainConnection>(Intent.GetStringExtra("train_conn"));

            Title = $"{mTrainConnection.TrainNumber} {mTrainConnection.StationEnd}";

            //
            //  SwipeRefreshLayout
            //

            mSwipeRefreshLayout = (SwipeRefreshLayout)FindViewById(Resource.Id.swipeRefreshLayoutDetails);
            mSwipeRefreshLayout.SetColorSchemeResources(Resource.Color.colorAccent_Light, Resource.Color.colorAccent_Dark);
            mSwipeRefreshLayout.Refresh += MSwipeRefreshLayout_Refresh;

            //
            //  Recycler details
            //

            mRecyclerDetails = (RecyclerView)FindViewById(Resource.Id.recyclerViewDetails);

            var mLayoutManager = new LinearLayoutManager(this);
            mRecyclerDetails.SetLayoutManager(mLayoutManager);

            mConnectionDetails = new List<StationSchedule>();

            mDetailsAdapter = new DetailsAdapter(mTrainConnection, mConnectionDetails);
            mDetailsAdapter.ItemClick += MDetailsAdapter_ItemClick;
            mRecyclerDetails.SetAdapter(mDetailsAdapter);

            //
            //  Info
            //

            var mTextViewRelation = (TextView)FindViewById(Resource.Id.textViewRelation);
            mTextViewRelation.Text = $"{mTrainConnection.StationStart} ({DateTime.FromOADate(mTrainConnection.TimeDepartureStart).ToShortTimeString()}) > {mTrainConnection.StationEnd} ({DateTime.FromOADate(mTrainConnection.TimeArrivalEnd).ToShortTimeString()})";

            var mTextViewDifficulties = (TextView)FindViewById(Resource.Id.textViewDifficulties);
            mTextViewDifficulties.Text = mTrainConnection.Up;

            var view = FindViewById(Resource.Id.include1);

            vh = new ConnectionsAdapterViewHolder(view, (e) =>
            {

            }, (e) =>
            {

            });

            RegisterTickReceiver();

            ConnectionItemHelper.BindViewHolder(this, new TrainConnectionListItem(mTrainConnection), vh);

            UpdateAll();
        }

        private void MSwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll()
        {
            UpdateAdapterData();
        }

        private void UpdateAdapterData()
        {
            mConnectionDetails.Clear();

            mSwipeRefreshLayout.Refreshing = true;
            new System.Threading.Thread(() =>
            {
                try
                {
                    var req = new ConnectionDetailsRequest(
                        mTrainConnection.TimetableYear,
                        mTrainConnection.Z,
                        mTrainConnection.Dk,
                        mTrainConnection.Spnnt,
                        mTrainConnection.Sknnt
                        );

                    var details = PKPAPI.GetConnectionRoute(req);
                    RunOnUiThread(() =>
                    {
                        var dt = new ConnectionDetails()
                        .FromJson(details);

                        mConnectionDetails.AddRange(dt.Stations);

                        mDetailsAdapter.NotifyDataSetChanged();

                        mSwipeRefreshLayout.Refreshing = false;
                    });
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    RunOnUiThread(() =>
                    {
                        if (ex.Response != null)
                        {
                            var status = ((HttpWebResponse)ex.Response).StatusCode;

                            ErrorDialogHelper.ShowServerErrorDialog(this, status, (s, e) =>
                            {
                                UpdateAdapterData();
                            });
                        }
                        else
                        {
                            ErrorDialogHelper.ShowConnectionErrorDialog(this, (s, e) =>
                            {
                                UpdateAdapterData();
                            });
                        }
                    });
                }
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

        protected override void OnResume()
        {
            RegisterTickReceiver();
            //mTrainConnAdapter.NotifyDataSetChanged();
            base.OnResume();
        }

        private void RegisterTickReceiver()
        {
            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAction(Intent.ActionTimeTick);
            mTickReceiver = new TickReceiver(() =>
            {
                UpdateAll();
            });
            RegisterReceiver(mTickReceiver, intentFilter);
        }

        protected override void OnDestroy()
        {
            try
            {
                UnregisterReceiver(mTickReceiver);
            }
            catch (IllegalArgumentException ex)
            {
                ex.PrintStackTrace();
            }
            base.OnDestroy();
        }

        private class TickReceiver : BroadcastReceiver
        {
            Action Action { get; set; }

            public TickReceiver(Action action)
            {
                Action = action;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                Action.Invoke();
            }
        }
    }
}