using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;

namespace WozAlboPrzewoz
{
    public class LicensesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<LicensesAdapterClickEventArgs> ItemClick;
        public event EventHandler<LicensesAdapterClickEventArgs> ItemLongClick;
        List<License> items;

        public LicensesAdapter(List<License> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            Android.Views.View itemView = null;
            var id = Resource.Layout.recycler_row_licenses;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new LicensesAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as LicensesAdapterViewHolder;

            holder.textViewHeader.Text = item.Product;
            holder.textViewContent.Text = item.LicenseText;
        }

        public override int ItemCount => items.Count;

        void OnClick(LicensesAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(LicensesAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class LicensesAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView textViewHeader { get; set; }
        public TextView textViewContent { get; set; }

        public LicensesAdapterViewHolder(Android.Views.View itemView, Action<LicensesAdapterClickEventArgs> clickListener,
                            Action<LicensesAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            textViewHeader = (TextView)itemView.FindViewById(Resource.Id.textViewHeader);
            textViewContent = (TextView)itemView.FindViewById(Resource.Id.textViewContent);
            //TextView = v;
            itemView.Click += (sender, e) => clickListener(new LicensesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new LicensesAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class LicensesAdapterClickEventArgs : EventArgs
    {
        public Android.Views.View View { get; set; }
        public int Position { get; set; }
    }
}