using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.View;
using AndroidX.RecyclerView.Widget;
using Com.Orangegangsters.Github.Swipyrefreshlayout.Library;
using Java.Lang;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class ConnectionsActivity : AppCompatActivity, TimePickerDialog.IOnTimeSetListener, DatePickerDialog.IOnDateSetListener
    {
        private Station mSelectedStation;
        private List<TrainConnectionListItem> mTrainConnData;
        private ConnectionsAdapter mTrainConnAdapter;
        private TickReceiver mTickReceiver;
        private StationConnectionsManager mManager;
        private SwipyRefreshLayout mSwipyRefreshLayout;
        private RecyclerView mRecyclerView;
        private LinearLayoutManager mLayoutManager;
        private Button mButtonPickDate;
        private Button mButtonPickTime;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_train_connections);

            var mToolbar = (AndroidX.AppCompat.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);

            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            int sid = Intent.GetIntExtra("id", 0);
            mSelectedStation = StationsCache.Stations[sid];

            //
            //  Toolbar
            //

            var mToolbarText = (TextView)mToolbar.FindViewById(Resource.Id.toolbar_title);
            mToolbarText.Text = mSelectedStation.Name;

            //
            //  Connections Manager
            //

            mManager = new StationConnectionsManager(mSelectedStation, DateTime.Now.AddMinutes(-1));
            mManager.UpdateEnd += MManager_UpdateEnd;
            mManager.HttpError += MManager_HttpError;

            //
            //  Swipy refresh layout
            //

            mSwipyRefreshLayout = (SwipyRefreshLayout)FindViewById(Resource.Id.swipyRefreshLayoutTrains);
            mSwipyRefreshLayout.SetColorSchemeResources(Resource.Color.colorAccent_Light);
            mSwipyRefreshLayout.SetDistanceToTriggerSync((int)(Resources.DisplayMetrics.Density * 64));
            mSwipyRefreshLayout.Refresh += MSwipeRefreshLayout_Refresh;

            //
            //  Train connections recycler
            //

            mRecyclerView = (RecyclerView)FindViewById(Resource.Id.recyclerView);

            mLayoutManager = new LinearLayoutManager(this)
            {
                ItemPrefetchEnabled = true
            };
            mRecyclerView.SetLayoutManager(mLayoutManager);

            mTrainConnData = mManager.Connections;

            mTrainConnAdapter = new ConnectionsAdapter(this, mTrainConnData);
            mTrainConnAdapter.ItemClick += MTrainConnAdapter_ItemClick;
            mRecyclerView.SetAdapter(mTrainConnAdapter);

            //
            //  Date picker
            //

            mButtonPickDate = (Button)FindViewById(Resource.Id.buttonPickDate);
            mButtonPickDate.Text = mManager.Time.ToString("d MMMM yyyy");
            mButtonPickDate.Click += MButtonPickDate_Click;

            //
            //  Time picker
            //

            mButtonPickTime = (Button)FindViewById(Resource.Id.buttonPickTime);
            mButtonPickTime.Text = mManager.Time.ToShortTimeString();
            mButtonPickTime.Click += MButtonPickTime_Click;

            //
            //  Results time update timer
            //

            RegisterTickReceiver();
            UpdateAdapterData();
        }

        private void MManager_HttpError(object sender, WebExceptionEventArgs e)
        {
            RunOnUiThread(() =>
            {
                mSwipyRefreshLayout.Post(() =>
                {
                    mSwipyRefreshLayout.Refreshing = false;
                });

                if (e.Exception.Response != null)
                {
                    var status = ((HttpWebResponse)e.Exception.Response).StatusCode;

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

        private void UpdateAdapterData()
        {
            mSwipyRefreshLayout.Post(() =>
            {
                mSwipyRefreshLayout.Refreshing = true;
            });
            mManager.Update();
        }

        private void MManager_UpdateEnd(object sender, ConnectionsEventArgs e)
        {
            RunOnUiThread(() =>
            {
                mSwipyRefreshLayout.Post(() =>
                {
                    mSwipyRefreshLayout.Refreshing = false;
                });

                mTrainConnAdapter.NotifyDataSetChanged();

                if (e.Direction != Direction.Unknown)
                {
                    var manager = (mRecyclerView.GetLayoutManager() as LinearLayoutManager);
                    var height = manager.FindViewByPosition(manager.FindLastVisibleItemPosition()).MeasuredHeight;

                    if (e.Direction == Direction.Up)
                    {
                        manager.ScrollToPositionWithOffset(10, 0);
                        mRecyclerView.SmoothScrollBy(0, -height * 3, new AnticipateOvershootInterpolator(), 1000);
                    }
                    else if (e.Direction == Direction.Down)
                    {
                        mRecyclerView.SmoothScrollBy(0, height * 3, new AnticipateOvershootInterpolator(), 1000);
                    }
                }
            });
        }

        private class RecyclerLinearSmoothScroller : LinearSmoothScroller
        {
            protected override int VerticalSnapPreference => SnapToAny;

            public RecyclerLinearSmoothScroller(Context context) : base(context)
            {
            }

            protected override float CalculateSpeedPerPixel(DisplayMetrics displayMetrics)
            {
                return 300f / (int)displayMetrics.DensityDpi;
            }
        }

        private void MSwipeRefreshLayout_Refresh(object sender, SwipyRefreshLayout.RefreshEventArgs e)
        {
            if (e.P0 == SwipyRefreshLayoutDirection.Top)
            {
                mManager.LoadPrevious();
            }
            else if (e.P0 == SwipyRefreshLayoutDirection.Bottom)
            {
                mManager.LoadLater();
            }
        }

        private void MTrainConnAdapter_ItemClick(object sender, ConnectionsAdapterClickEventArgs e)
        {
            TrainConnectionListItem item = mTrainConnData[e.Position];

            var intent = new Intent(this, typeof(DetailsActivity));
            intent.PutExtra("train_conn", JsonConvert.SerializeObject(item.Connection));
            intent.PutExtra("selected_station", JsonConvert.SerializeObject(mSelectedStation));
            intent.PutExtra("transition", ViewCompat.GetTransitionName(e.View));

            ActivityOptionsCompat options = ActivityOptionsCompat.MakeSceneTransitionAnimation(
                    this,
                    e.View,
                    ViewCompat.GetTransitionName(e.View));

            StartActivity(intent, options.ToBundle());
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_connections, menu);
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

        //  Time picker

        private void MButtonPickTime_Click(object sender, EventArgs e)
        {
            TimePickerDialog timePickerDialog = new TimePickerDialog(this, this, mManager.Time.Hour, mManager.Time.Minute, true);
            timePickerDialog.Show();
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            DateTime selectedTime = new DateTime(mManager.Time.Year, mManager.Time.Month, mManager.Time.Day, hourOfDay, minute, 0);
            mButtonPickTime.Text = selectedTime.ToShortTimeString();
            mManager.Time = selectedTime;
            UpdateAdapterData();
        }

        //  Date picker

        private void MButtonPickDate_Click(object sender, EventArgs e)
        {
            DatePickerDialog datePickerDialog = new DatePickerDialog(this, this, mManager.Time.Year, mManager.Time.Month - 1, mManager.Time.Day);
            datePickerDialog.Show();
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            DateTime selectedTime = new DateTime(year, month + 1, dayOfMonth, mManager.Time.Hour, mManager.Time.Minute, 0);
            mButtonPickDate.Text = selectedTime.ToString("d MMMM yyyy");
            mManager.Time = selectedTime;
            UpdateAdapterData();
        }
    }
}