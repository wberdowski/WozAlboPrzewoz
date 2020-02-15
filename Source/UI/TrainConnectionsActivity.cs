using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class TrainConnectionsActivity : AppCompatActivity
    {
        private Station mSelectedStation;
        private List<TrainConnection> mTrainConnData;
        private ConnectionsRecyclerAdapter mTrainConnAdapter;
        private TickReceiver mTickReceiver;
        private SwipeRefreshLayout mSwipeRefreshLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_train_connections);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            Forms.Init(this, savedInstanceState);

            int sid = Intent.GetIntExtra("id", 0);
            mSelectedStation = StationsCache.Stations[sid];

            Title = mSelectedStation.nazwa;

            //
            //  Swipe refresh layout
            //

            mSwipeRefreshLayout = (SwipeRefreshLayout)FindViewById(Resource.Id.swipeRefreshLayoutTrains);
            mSwipeRefreshLayout.Refresh += MSwipeRefreshLayout_Refresh;

            //
            //  Train connections recycler
            //

            RecyclerView mRecyclerView = (RecyclerView)FindViewById(Resource.Id.recyclerView);

            LinearLayoutManager mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mTrainConnData = new List<TrainConnection>();

            mTrainConnAdapter = new ConnectionsRecyclerAdapter(this, mTrainConnData);
            mTrainConnAdapter.ItemClick += MTrainConnAdapter_ItemClick;
            mRecyclerView.SetAdapter(mTrainConnAdapter);

            //
            //  Results time update timer
            //

            RegisterTickReceiver();

            UpdateAdapterData();
        }

        private void MTrainConnAdapter_ItemClick(object sender, RecyclerAdapterClickEventArgs e)
        {
            TrainConnection conn = mTrainConnData[e.Position];
            OpenDetailsActivity(conn);
        }

        private void OpenDetailsActivity(TrainConnection conn)
        {
            Intent startActivityIntent = new Intent(this, typeof(ConnectionDetailsActivity));
            startActivityIntent.PutExtra("r", conn.timetableYear);
            startActivityIntent.PutExtra("z", conn.z);
            startActivityIntent.PutExtra("dk", conn.dk);
            startActivityIntent.PutExtra("spnnt", conn.spnnt);
            startActivityIntent.PutExtra("sknnt", conn.sknnt);
            StartActivity(startActivityIntent);
        }

        private void MSwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            UpdateAdapterData();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            if (FavoritesManager.HasFavorite(mSelectedStation))
            {
                IMenuItem favoriteAction = menu.FindItem(Resource.Id.action_favorite);
                favoriteAction.SetIcon(Resource.Drawable.favorite_24px);
            }

            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_favorite)
            {
                if (!FavoritesManager.HasFavorite(mSelectedStation))
                {
                    FavoritesManager.AddFavorite(mSelectedStation);
                    item.SetIcon(Resource.Drawable.favorite_24px);
                }
                else
                {
                    FavoritesManager.RemoveFavorite(mSelectedStation);
                    item.SetIcon(Resource.Drawable.favorite_border_24px);
                }
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }

        private void UpdateAdapterData()
        {
            mTrainConnData.Clear();
            mSwipeRefreshLayout.Refreshing = true;
            new System.Threading.Thread(() =>
            {
                TrainConnection[] connections = PKPAPI.GetStationTimetable(mSelectedStation.id, DateTime.Now);
                Device.BeginInvokeOnMainThread(() =>
                {
                    mTrainConnData.AddRange(connections);
                    mTrainConnAdapter.NotifyDataSetChanged();
                    mSwipeRefreshLayout.Refreshing = false;
                });
            }).Start();
        }

        protected override void OnResume()
        {
            RegisterTickReceiver();
            mTrainConnAdapter.NotifyDataSetChanged();
            base.OnResume();
        }

        private void RegisterTickReceiver()
        {
            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAction(Intent.ActionTimeTick);
            mTickReceiver = new TickReceiver(mTrainConnAdapter);
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
            private ConnectionsRecyclerAdapter mAdapter;

            public TickReceiver(ConnectionsRecyclerAdapter adapter)
            {
                mAdapter = adapter;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                mAdapter.NotifyDataSetChanged();
            }
        }
    }
}