using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Preference;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Launcher", MainLauncher = true, Icon = "@mipmap/ic_launcher")]
    public class MainActivity : AppCompatActivity
    {
        private Android.Support.V7.Widget.SearchView mSearchView;
        private SearchCursorAdapter mSearchSuggestionsAdapter;
        private RecyclerView mRecyclerViewFavorites;
        private List<Station> mFavoritesData;
        private FavoritesAdapter mFavoritesAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(Resource.Style.AppTheme);
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            AppCompatDelegate.DefaultNightMode = (prefs.GetBoolean("dark_mode", false) ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo);

            StationsCache.Load();
            TranslationsCache.Load();

            FavoritesManager.Load();
            FavoritesManager.FavoritesChanged += FavoritesManager_FavoritesChanged;

            var mToolbar = (AndroidX.AppCompat.Widget.Toolbar)FindViewById(Resource.Id.toolbar);
            SetSupportActionBar(mToolbar);

            SupportActionBar.SetDisplayShowTitleEnabled(false);

            //
            //  Favorites Recycler
            //

            mRecyclerViewFavorites = (RecyclerView)FindViewById(Resource.Id.recyclerViewFavorites);

            var mLayoutManager = new LinearLayoutManager(this);
            mRecyclerViewFavorites.SetLayoutManager(mLayoutManager);

            mFavoritesData = FavoritesManager.favorites;

            mFavoritesAdapter = new FavoritesAdapter(mFavoritesData);
            mFavoritesAdapter.ItemClick += MFavoritesAdapter_ItemClick;
            mRecyclerViewFavorites.SetAdapter(mFavoritesAdapter);

            ItemTouchHelper touchHelper = new ItemTouchHelper(new ItemTouchHelperCallback(this));
            touchHelper.AttachToRecyclerView(mRecyclerViewFavorites);

            //
            //  SearchView
            //

            mSearchView = (Android.Support.V7.Widget.SearchView)FindViewById(Resource.Id.editTextSearch);
            mSearchView.QueryTextChange += SearchView_QueryTextChange;
            mSearchView.QueryTextSubmit += MSearchView_QueryTextSubmit;
            mSearchView.SuggestionClick += MSearchView_SuggestionClick;
            mSearchView.InputType = (int)InputTypes.TextFlagCapWords;

            mSearchSuggestionsAdapter = new SearchCursorAdapter(this, null, mSearchView);
            mSearchView.SuggestionsAdapter = mSearchSuggestionsAdapter;

            //
            //  No favorites container
            //

            mNoFavoritesContainer = (Android.Widget.LinearLayout)FindViewById(Resource.Id.noFavoritesContainer);
            UpdateNoFavoritesContainerVisibility();
        }

        private class ItemTouchHelperCallback : ItemTouchHelper.Callback
        {
            private MainActivity Activity { get; set; }
            private bool mOrderChanged;

            public ItemTouchHelperCallback(MainActivity activity)
            {
                Activity = activity;
            }

            public override int GetMovementFlags(RecyclerView p0, RecyclerView.ViewHolder p1)
            {
                return MakeFlag(ItemTouchHelper.ActionStateDrag, ItemTouchHelper.Down | ItemTouchHelper.Up);
            }

            public override bool OnMove(RecyclerView p0, RecyclerView.ViewHolder p1, RecyclerView.ViewHolder p2)
            {
                Activity.mFavoritesData.Swap(p1.AdapterPosition, p2.AdapterPosition);
                Activity.mFavoritesAdapter.NotifyItemMoved(p1.AdapterPosition, p2.AdapterPosition);
                mOrderChanged = true;
                return true;
            }

            public override void OnSwiped(RecyclerView.ViewHolder p0, int p1)
            {

            }

            public override void OnSelectedChanged(RecyclerView.ViewHolder viewHolder, int actionState)
            {
                base.OnSelectedChanged(viewHolder, actionState);

                if (actionState == ItemTouchHelper.ActionStateIdle && mOrderChanged)
                {
                    FavoritesManager.Commit(false);
                    mOrderChanged = false;
                }
            }
        }

        //private void MFavoritesAdapter_ItemLongClick(object sender, ConnectionsAdapterClickEventArgs e)
        //{
        //var station = FavoritesManager.favorites[e.Position];
        //int idx = FavoritesManager.RemoveFavorite(station);
        //mFavoritesAdapter.NotifyItemRemoved(idx);
        //SnackbarUtils.MakeWithMargins(mSearchView, Application.Resources.GetString(Resource.String.action_favorite_removed, station.Name), Snackbar.LengthLong)
        //    .SetAction(Resource.String.action_undo, (v) =>
        //    {
        //        FavoritesManager.AddFavoriteAt(station, idx);
        //    })
        //    .Show();
        //}

        private void MFavoritesAdapter_ItemClick(object sender, ConnectionsAdapterClickEventArgs e)
        {
            var station = FavoritesManager.favorites[e.Position];
            for (int i = 0; i < StationsCache.Stations.Length; i++)
            {
                if (StationsCache.Stations[i].Id == station.Id)
                {
                    OpenStationActivity(i);
                }
            }
        }

        private void FavoritesManager_FavoritesChanged(object sender, System.EventArgs e)
        {
            mFavoritesAdapter.NotifyDataSetChanged();
            UpdateNoFavoritesContainerVisibility();
        }

        private void UpdateNoFavoritesContainerVisibility()
        {
            if (FavoritesManager.favorites.Count > 0)
            {
                mNoFavoritesContainer.Visibility = ViewStates.Gone;
            }
            else
            {
                mNoFavoritesContainer.Visibility = ViewStates.Visible;
            }
        }

        private void OpenStationActivity(int sid)
        {
            var startActivityIntent = new Intent(this, typeof(ConnectionsActivity));
            startActivityIntent.PutExtra("id", sid);
            StartActivity(startActivityIntent);
        }

        private void MSearchView_QueryTextSubmit(object sender, Android.Support.V7.Widget.SearchView.QueryTextSubmitEventArgs e)
        {
            string text = ReplaceNonEnglishCharacters(e.NewText.ToLower());
            if (text.Length > 1)
            {
                if (mSearchSuggestionsAdapter.Count > 0)
                {
                    var cursor = mSearchSuggestionsAdapter.GetItem(0) as MatrixCursor;
                    int sid = cursor.GetInt(0);

                    OpenStationActivity(sid);

                    mSearchView.SetQuery(string.Empty, false);

                    e.Handled = true;
                }
            }
        }

        private void MSearchView_SuggestionClick(object sender, Android.Support.V7.Widget.SearchView.SuggestionClickEventArgs e)
        {
            var cursor = mSearchSuggestionsAdapter.GetItem(e.Position) as MatrixCursor;
            int sid = cursor.GetInt(0);

            OpenStationActivity(sid);

            mSearchView.SetQuery(string.Empty, false);

            e.Handled = true;
        }

        private void SearchView_QueryTextChange(object sender, Android.Support.V7.Widget.SearchView.QueryTextChangeEventArgs e)
        {
            string text = ReplaceNonEnglishCharacters(e.NewText.ToLower());
            if (text.Length > 1)
            {
                var c = new MatrixCursor(new string[] { BaseColumns.Id, "nazwa" });

                for (int i = 0; i < StationsCache.Stations.Length; i++)
                {
                    var station = StationsCache.Stations[i];

                    if (ReplaceNonEnglishCharacters(station.Name.ToLower()).Contains(text))
                    {
                        c.AddRow(new Java.Lang.Object[] { i, station.Name });
                    }
                }

                mSearchSuggestionsAdapter.ChangeCursor(c);
                e.Handled = true;
            }
        }

        private IDictionary<char, char> mDictionary = new Dictionary<char, char>
        {
            {'ą', 'a'},
            {'ć', 'c'},
            {'ę', 'e'},
            {'ł', 'l'},
            {'ń', 'n'},
            {'ó', 'o'},
            {'ś', 's'},
            {'ź', 'z'},
            {'ż', 'z'},
            {'-',' '}
        };
        private LinearLayout mNoFavoritesContainer;

        private string ReplaceNonEnglishCharacters(string str)
        {
            var chars = str.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                char replacement;
                if (mDictionary.TryGetValue(chars[i], out replacement))
                {
                    chars[i] = replacement;
                }
            }

            return new string(chars);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);

            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (item.ItemId == Resource.Id.action_settings)
            {
                StartActivity(typeof(SettingsActivity));
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}