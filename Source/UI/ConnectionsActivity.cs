using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using Com.Orangegangsters.Github.Swipyrefreshlayout.Library;
using Java.Lang;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class ConnectionsActivity : AppCompatActivity, TimePickerDialog.IOnTimeSetListener
    {
        private Station mSelectedStation;
        private List<TrainConnectionListItem> mTrainConnData;
        private ConnectionsAdapter mTrainConnAdapter;
        private TickReceiver mTickReceiver;
        private SwipyRefreshLayout mSwipyRefreshLayout;
        private DateTime mSearchTime;
        private IMenuItem mDatetimeAction;
        private RecyclerView mRecyclerView;
        private LinearLayoutManager mLayoutManager;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_train_connections);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            int sid = Intent.GetIntExtra("id", 0);
            mSelectedStation = StationsCache.Stations[sid];
            mSearchTime = DateTime.Now.AddMinutes(-1);

            Title = mSelectedStation.name;

            //
            //  Swipy refresh layout
            //

            mSwipyRefreshLayout = (SwipyRefreshLayout)FindViewById(Resource.Id.swipyRefreshLayoutTrains);
            mSwipyRefreshLayout.SetDistanceToTriggerSync((int)(Resources.DisplayMetrics.Density * 64));
            mSwipyRefreshLayout.Refresh += MSwipeRefreshLayout_Refresh;

            //
            //  Train connections recycler
            //

            mRecyclerView = (RecyclerView)FindViewById(Resource.Id.recyclerView);

            mLayoutManager = new LinearLayoutManager(this);
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mTrainConnData = new List<TrainConnectionListItem>();

            mTrainConnAdapter = new ConnectionsAdapter(this, mTrainConnData);
            mTrainConnAdapter.ItemClick += MTrainConnAdapter_ItemClick;
            mRecyclerView.SetAdapter(mTrainConnAdapter);

            //
            //  Results time update timer
            //

            RegisterTickReceiver();

            UpdateAdapterData();
        }

        private void MSwipeRefreshLayout_Refresh(object sender, SwipyRefreshLayout.RefreshEventArgs e)
        {
            LoadMore(e.P0);
        }

        private void MTrainConnAdapter_ItemClick(object sender, ConnectionsAdapterClickEventArgs e)
        {
            TrainConnectionListItem item = mTrainConnData[e.Position];
            OpenDetailsActivity(item.Connection);
        }

        private void OpenDetailsActivity(TrainConnection conn)
        {
            var intent = new Intent(this, typeof(DetailsActivity));
            intent.PutExtra("train_conn", JsonConvert.SerializeObject(conn));
            StartActivity(intent);
        }


        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_connections, menu);
            if (FavoritesManager.HasFavorite(mSelectedStation))
            {
                IMenuItem favoriteAction = menu.FindItem(Resource.Id.action_favorite);
                favoriteAction.SetIcon(Resource.Drawable.favorite_24px);
            }

            mDatetimeAction = menu.FindItem(Resource.Id.action_datetime);
            mDatetimeAction.SetTitle(mSearchTime.ToShortTimeString());

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
            else if (item.ItemId == Resource.Id.action_datetime)
            {
                TimePickerDialog timePickerDialog = new TimePickerDialog(this, this, mSearchTime.Hour, mSearchTime.Minute, true);
                timePickerDialog.Show();
            }

            return base.OnOptionsItemSelected(item);
        }

        public override bool OnSupportNavigateUp()
        {
            Finish();
            return true;
        }

        private void LoadMore(SwipyRefreshLayoutDirection direction)
        {
            new System.Threading.Thread(() =>
            {
                if (direction == SwipyRefreshLayoutDirection.Top)
                {
                    var firstConn = mTrainConnData.First().Connection;
                    var connections = PKPAPI.GetStationTimetable(mSelectedStation.id, DateTime.FromOADate(firstConn.TimeDeparture), 0, 10);

                    RunOnUiThread(() =>
                    {
                        var list = new List<TrainConnectionListItem>();

                        foreach (var conn in connections)
                        {
                            list.Add(new TrainConnectionListItem(conn));
                        }

                        mTrainConnData.InsertRange(0, list);
                        CleanUpData();
                        mTrainConnAdapter.NotifyDataSetChanged();
                        mSwipyRefreshLayout.Refreshing = false;
                    });
                }
                else if (direction == SwipyRefreshLayoutDirection.Bottom)
                {
                    var lastConn = mTrainConnData.Last().Connection;
                    var connections = PKPAPI.GetStationTimetable(mSelectedStation.id, DateTime.FromOADate(lastConn.TimeDeparture), 2, 10);

                    RunOnUiThread(() =>
                    {
                        foreach (var conn in connections)
                        {
                            if (lastConn.Sknnt == conn.Sknnt && lastConn.Spnnt == conn.Spnnt) continue;

                            mTrainConnData.Add(new TrainConnectionListItem(conn));
                        }

                        CleanUpData();
                        mTrainConnAdapter.NotifyDataSetChanged();
                        mSwipyRefreshLayout.Refreshing = false;
                    });
                }
            }).Start();

        }

        private void CleanUpData()
        {
            DateTime lastDate = DateTime.Now;

            mTrainConnData.Sort((a, b) =>
            {
                return System.Math.Sign(a.Connection.TimeDeparture - b.Connection.TimeDeparture);
            });

            foreach (var item in mTrainConnData)
            {
                var date = DateTime.FromOADate(item.Connection.TimeDeparture).Date;

                if (lastDate < date)
                {
                    item.HasHeader = true;
                    item.HeaderText = date.ToLongDateString();
                }

                lastDate = date;
            }
        }

        private void UpdateAdapterData()
        {
            mTrainConnData.Clear();
            mSwipyRefreshLayout.Post(() =>
            {
                mSwipyRefreshLayout.Refreshing = true;
            });
            new System.Threading.Thread(() =>
            {
                try
                {
                    var connections = PKPAPI.GetStationTimetable(mSelectedStation.id, mSearchTime);

                    RunOnUiThread(() =>
                    {
                        DateTime lastDate = DateTime.Now;

                        for (int i = 0; i < connections.Length; i++)
                        {
                            var conn = connections[i];
                            var item = new TrainConnectionListItem(conn);
                            var date = DateTime.FromOADate(conn.TimeDeparture).Date;

                            if (lastDate < date)
                            {
                                item.HasHeader = true;
                                item.HeaderText = date.ToLongDateString();
                            }

                            lastDate = date;
                            mTrainConnData.Add(item);
                        }
                        CleanUpData();
                        mTrainConnAdapter.NotifyDataSetChanged();
                        mSwipyRefreshLayout.Refreshing = false;
                    });
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    RunOnUiThread(() =>
                    {
                        mSwipyRefreshLayout.Refreshing = false;
                        if (ex.Response != null)
                        {
                            var status = ((HttpWebResponse)ex.Response).StatusCode;

                            var dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                            dialog.SetTitle(Resource.String.dialog_server_error_title);
                            dialog.SetMessage(Resources.GetString(Resource.String.dialog_server_error_body, (int)status, status.ToString()));
                            dialog.SetNeutralButton(Resource.String.dialog_button_try_again, (s, e) =>
                            {
                                UpdateAdapterData();
                            });
                            dialog.SetPositiveButton(Resource.String.dialog_button_ok, (s, e) =>
                            {

                            });
                            dialog.Show();
                        }
                        else
                        {
                            var dialog = new Android.Support.V7.App.AlertDialog.Builder(this);
                            dialog.SetTitle(Resource.String.dialog_connection_error_title);
                            dialog.SetMessage(Resource.String.dialog_connection_error_body);
                            dialog.SetNeutralButton(Resource.String.dialog_button_try_again, (s, e) =>
                            {
                                UpdateAdapterData();
                            });
                            dialog.SetPositiveButton(Resource.String.dialog_button_ok, (s, e) =>
                            {

                            });
                            dialog.Show();
                        }
                    });
                }
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
            private ConnectionsAdapter mAdapter;

            public TickReceiver(ConnectionsAdapter adapter)
            {
                mAdapter = adapter;
            }

            public override void OnReceive(Context context, Intent intent)
            {
                mAdapter.NotifyDataSetChanged();
            }
        }

        public void OnTimeSet(Android.Widget.TimePicker view, int hourOfDay, int minute)
        {
            DateTime currentTime = DateTime.Now;
            DateTime selectedTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hourOfDay, minute, 0);
            mDatetimeAction.SetTitle(selectedTime.ToShortTimeString());
            mSearchTime = selectedTime;
            UpdateAdapterData();
        }
    }
}