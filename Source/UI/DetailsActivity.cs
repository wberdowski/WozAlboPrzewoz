﻿using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Newtonsoft.Json;
using System;
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
        private SwipeRefreshLayout mSwipeRefreshLayout;

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

            UpdateAdapterData();
        }

        private void MSwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            UpdateAdapterData();
        }

        private void UpdateAdapterData()
        {
            mConnectionDetails.Clear();

            mSwipeRefreshLayout.Refreshing = true;
            new System.Threading.Thread(() =>
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