using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using System.Collections.Generic;
using Android.Text;
using Android.Provider;
using Android.Database;
using Android.Content;
using Android.Support.V7.Widget;
using System;
using System.Linq;
using Android.Support.Design.Widget;

namespace WozAlboPrzewoz
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Android.Support.V7.Widget.SearchView mSearchView;
        private SearchCursorAdapter mSearchSuggestionsAdapter;
        private RecyclerView mRecyclerViewFavorites;
        private List<Station> mFavoritesData;
        private FavoritesAdapter mFavoritesAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            StationsCache.LoadStations();
            FavoritesManager.Load();
            FavoritesManager.FavoritesChanged += FavoritesManager_FavoritesChanged;

            //
            //  Favorites recycler
            //

            mRecyclerViewFavorites = (RecyclerView)FindViewById(Resource.Id.recyclerViewFavorites);

            LinearLayoutManager mLayoutManager = new LinearLayoutManager(this);
            mRecyclerViewFavorites.SetLayoutManager(mLayoutManager);

            mFavoritesData = FavoritesManager.favorites;

            mFavoritesAdapter = new FavoritesAdapter(mFavoritesData);
            mFavoritesAdapter.ItemClick += MFavoritesAdapter_ItemClick;
            mFavoritesAdapter.ItemLongClick += MFavoritesAdapter_ItemLongClick;
            mRecyclerViewFavorites.SetAdapter(mFavoritesAdapter);

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
        }

        private void MFavoritesAdapter_ItemLongClick(object sender, RecyclerAdapterClickEventArgs e)
        {
            Station s = FavoritesManager.favorites[e.Position];
            int idx = FavoritesManager.RemoveFavorite(s);
            mFavoritesAdapter.NotifyItemRemoved(idx);
            Snackbar.Make(mSearchView, Application.Resources.GetString(Resource.String.action_favorite_removed, s.nazwa), Snackbar.LengthLong)
                .SetAction(Resource.String.action_undo, (v) =>
                {
                    FavoritesManager.AddFavorite(s);
                })
                .Show();
        }

        private void MFavoritesAdapter_ItemClick(object sender, RecyclerAdapterClickEventArgs e)
        {
            Station s = FavoritesManager.favorites[e.Position];
            for (int i = 0; i < StationsCache.Stations.Length; i++)
            {
                if (StationsCache.Stations[i].id == s.id)
                {
                    OpenStationActivity(i);
                }
            }
        }

        private void FavoritesManager_FavoritesChanged(object sender, System.EventArgs e)
        {
            mFavoritesAdapter.NotifyDataSetChanged();
        }

        private void OpenStationActivity(int sid)
        {
            Intent startActivityIntent = new Intent(this, typeof(TrainConnectionsActivity));
            startActivityIntent.PutExtra("id", sid);
            StartActivity(startActivityIntent);
        }

        private void MSearchView_QueryTextSubmit(object sender, Android.Support.V7.Widget.SearchView.QueryTextSubmitEventArgs e)
        {
            string text = ReplaceNonEnglishCharacters(e.NewText.ToLower());
            if (text.Length > 1)
            {
                MatrixCursor cursor = mSearchSuggestionsAdapter.GetItem(0) as MatrixCursor;
                int sid = cursor.GetInt(0);

                OpenStationActivity(sid);

                mSearchView.SetQuery(string.Empty, false);

                e.Handled = true;
            }
        }

        private void MSearchView_SuggestionClick(object sender, Android.Support.V7.Widget.SearchView.SuggestionClickEventArgs e)
        {
            MatrixCursor cursor = mSearchSuggestionsAdapter.GetItem(e.Position) as MatrixCursor;
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
                MatrixCursor c = new MatrixCursor(new string[] { BaseColumns.Id, "nazwa" });

                for (int i = 0; i < StationsCache.Stations.Length; i++)
                {
                    Station s = StationsCache.Stations[i];

                    if (ReplaceNonEnglishCharacters(s.nazwa.ToLower()).Contains(text))
                    {
                        c.AddRow(new Java.Lang.Object[] { i, s.nazwa });
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

        private string ReplaceNonEnglishCharacters(string str)
        {
            char[] chars = str.ToCharArray();

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
    }
}