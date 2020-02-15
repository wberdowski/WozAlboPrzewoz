using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Database;

namespace WozAlboPrzewoz
{
    public class SearchCursorAdapter : Android.Support.V4.Widget.CursorAdapter
    {
        private LayoutInflater mLayoutInflater;
        private Context mContext;
        private Android.Support.V7.Widget.SearchView searchView;

        public SearchCursorAdapter(Context context, ICursor cursor, Android.Support.V7.Widget.SearchView sv) : base(context, cursor, false)
        {
            mContext = context;
            searchView = sv;
            mLayoutInflater = LayoutInflater.From(context);
        }

        public override void BindView(View view, Context context, ICursor cursor)
        {
            string stationName = cursor.GetString(cursor.GetColumnIndexOrThrow("nazwa"));

            TextView textViewSearchResult = (TextView)view.FindViewById(Resource.Id.textViewSearchResult);
            textViewSearchResult.Text = stationName;
        }

        public override View NewView(Context context, ICursor cursor, ViewGroup parent)
        {
            View v = mLayoutInflater.Inflate(Resource.Layout.recycler_row_search, parent, false);
            return v;
        }
    }
}