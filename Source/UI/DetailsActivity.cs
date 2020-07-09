using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Android.Material.BottomSheet;
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
        private List<StationSchedule> mStations;
        private DetailsAdapter mDetailsAdapter;
        private TrainConnection mTrainConnection;
        private SwipeRefreshLayout mSwipeRefreshLayout;
        private TickReceiver mTickReceiver;
        private ConnectionsAdapterViewHolder vh;
        private Station mSelectedStation;
        private System.Timers.Timer mProgressTimer;
        private BottomSheetBehavior mBottomSheetBehavior;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_details);

            var mToolbar = (AndroidX.AppCompat.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);

            SupportActionBar.SetDisplayShowTitleEnabled(false);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            mTrainConnection = JsonConvert.DeserializeObject<TrainConnection>(Intent.GetStringExtra("train_conn"));
            mSelectedStation = JsonConvert.DeserializeObject<Station>(Intent.GetStringExtra("selected_station"));
            Bundle extras = Intent.Extras;

            //
            //  Toolbar
            //

            var mToolbarText = (TextView)mToolbar.FindViewById(Resource.Id.toolbar_title);
            mToolbarText.Text = $"{mTrainConnection.TrainNumber} {mTrainConnection.StationEnd}";
            mToolbarText.Selected = true;

            //
            //  SwipeRefreshLayout
            //

            mSwipeRefreshLayout = (SwipeRefreshLayout)FindViewById(Resource.Id.swipeRefreshLayoutDetails);
            mSwipeRefreshLayout.SetColorSchemeResources(Resource.Color.colorAccent_Light);
            mSwipeRefreshLayout.Refresh += MSwipeRefreshLayout_Refresh;

            //
            //  Recycler details
            //

            mRecyclerDetails = (RecyclerView)FindViewById(Resource.Id.recyclerViewDetails);

            var mLayoutManager = new LinearLayoutManager(this);
            mRecyclerDetails.SetLayoutManager(mLayoutManager);

            mStations = new List<StationSchedule>();

            mDetailsAdapter = new DetailsAdapter(mTrainConnection, mSelectedStation, mStations);
            mDetailsAdapter.ItemClick += MDetailsAdapter_ItemClick;
            mRecyclerDetails.SetAdapter(mDetailsAdapter);

            //
            //  Include
            //

            var view = FindViewById(Resource.Id.include1);
            //view.Elevation = 4 * Resources.DisplayMetrics.Density + 0.5f;
            view.SetBackgroundResource(Resource.Drawable.recycler_row_bg_full);
            view.OutlineProvider = ViewOutlineProvider.Background;

            var linearLayoutRow = (LinearLayout)view.FindViewById(Resource.Id.linearLayoutRow);
            linearLayoutRow.SetBackgroundResource(0);

            vh = new ConnectionsAdapterViewHolder(view, (e) =>
            {
                if (mBottomSheetBehavior.State == BottomSheetBehavior.StateExpanded)
                {
                    mBottomSheetBehavior.State = BottomSheetBehavior.StateCollapsed;
                }
                else
                {
                    mBottomSheetBehavior.State = BottomSheetBehavior.StateExpanded;
                }
            }, (e) =>
            {

            });

            //
            //  Bottom sheet
            //

            LinearLayout llBottomSheet = (LinearLayout)FindViewById(Resource.Id.bottom_sheet);

            mBottomSheetBehavior = BottomSheetBehavior.From(llBottomSheet);

            //
            //  Info
            //

            var mTextViewDifficulties = (TextView)FindViewById(Resource.Id.textViewDifficulties);
            var difficultiesText = AlertParser.Parse(mTrainConnection.Up);
            mTextViewDifficulties.Text = (difficultiesText.Length > 0 ? difficultiesText : GetString(Resource.String.no_difficulties));

            //
            //  Transition
            //

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                view.TransitionName = extras.GetString("transition");
                SupportStartPostponedEnterTransition();
            }

            //
            //  Init
            //

            RegisterTickReceiver();

            UpdateViewHolder();
            UpdateAll(onRefreshed: () =>
             {
                 int idx = mStations.FindIndex((x) =>
                 {
                     return x.Name == mSelectedStation.Name;
                 });

                 (mRecyclerDetails.GetLayoutManager() as LinearLayoutManager).ScrollToPositionWithOffset(idx, 0);
             });

            mProgressTimer = new System.Timers.Timer(1000 / 10f);
            mProgressTimer.Elapsed += Timer_Elapsed;
            mProgressTimer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                mDetailsAdapter.UpdateProgress();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void MSwipeRefreshLayout_Refresh(object sender, EventArgs e)
        {
            UpdateAll();
        }

        private void UpdateAll(bool showRefresh = true, Action onRefreshed = null)
        {
            UpdateAdapterData(showRefresh, () =>
             {
                 UpdateViewHolder();
                 onRefreshed?.Invoke();
             });
        }

        private void UpdateViewHolder()
        {
            var item = new TrainConnectionListItem(mTrainConnection);
            if (mStations.Count > 0)
            {
                item.Connection.DelayStart = mStations.Where(x => x.Name == mSelectedStation.Name).FirstOrDefault().DelayDeparture;
                item.Connection.DelayEnd = mStations.Last().DelayDeparture;
            }
            ConnectionItemHelper.SetViewHolderContent(this, item, vh);
        }

        private void UpdateAdapterData(bool showRefresh = true, Action onFinished = null)
        {
            if (showRefresh)
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

                    var details = PKPAPI.GetConnectionRoute(req, true);

                    RunOnUiThread(() =>
                    {
                        var dt = new ConnectionDetails()
                        .FromJson(details);

                        mStations.Clear();
                        mStations.AddRange(dt.Stations);

                        mDetailsAdapter.NotifyDataSetChanged();

                        mSwipeRefreshLayout.Refreshing = false;

                        if (onFinished != null)
                            onFinished?.Invoke();
                    });
                }
                catch (WebException ex)
                {
                    mSwipeRefreshLayout.Refreshing = false;
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
            var item = mStations[e.Position];

            Dialog dialog = new Dialog(this);
            dialog.SetContentView(Resource.Layout.dialog_station_details);

            var title = (TextView)dialog.FindViewById(Resource.Id.textViewDialogTitle);
            title.Text = item.Name;

            var arrivalTime = (TextView)dialog.FindViewById(Resource.Id.textViewArrivalTime);
            arrivalTime.Text = item.ArrivalTime.ToShortTimeString() + (item.DelayArrival > 0 ? $" (+{item.DelayArrival})" : "");

            var departureTime = (TextView)dialog.FindViewById(Resource.Id.textViewDepartureTime);
            departureTime.Text = item.DepartureTime.ToShortTimeString() + (item.DelayDeparture > 0 ? $" (+{item.DelayDeparture})" : "");

            var platformTrack = (TextView)dialog.FindViewById(Resource.Id.textViewPlatformTrack);
            platformTrack.Text = $"{item.Platform} / {item.Track}";

            var layoutParams = dialog.Window.Attributes;
            layoutParams.Width = ViewGroup.LayoutParams.MatchParent;

            dialog.Show();
        }

        public override bool OnSupportNavigateUp()
        {
            FinishAfterTransition();
            return true;
        }

        protected override void OnResume()
        {
            RegisterTickReceiver();
            UpdateViewHolder();
            base.OnResume();
        }

        private void RegisterTickReceiver()
        {
            IntentFilter intentFilter = new IntentFilter();
            intentFilter.AddAction(Intent.ActionTimeTick);
            mTickReceiver = new TickReceiver(() =>
            {
                // UpdateAll(false);
                UpdateViewHolder();
            });
            RegisterReceiver(mTickReceiver, intentFilter);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            try
            {
                mProgressTimer.Stop();
                mProgressTimer.Dispose();
                UnregisterReceiver(mTickReceiver);
            }
            catch (IllegalArgumentException ex)
            {
                ex.PrintStackTrace();
            }
        }

        private class TickReceiver : BroadcastReceiver
        {
            private Action Action { get; set; }

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