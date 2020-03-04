using System;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.Content;
using AndroidX.RecyclerView.Widget;
using AndroidX.CardView.Widget;

namespace WozAlboPrzewoz
{
    public class ConnectionsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ConnectionsAdapterClickEventArgs> ItemClick;
        public event EventHandler<ConnectionsAdapterClickEventArgs> ItemLongClick;
        List<TrainConnectionListItem> items;
        Context context;

        public ConnectionsAdapter(Context ctx, List<TrainConnectionListItem> data)
        {
            context = ctx;
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            Android.Views.View itemView = null;
            var id = Resource.Layout.recycler_row_connection;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new ConnectionsAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as ConnectionsAdapterViewHolder;

            ConnectionItemHelper.SetViewHolderContent(context, item, holder);
        }

        public override int ItemCount => items.Count;

        void OnClick(ConnectionsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ConnectionsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class ConnectionsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public LinearLayout linearLayoutRow { get; set; }
        public TextView textViewHeader { get; set; }
        public TextView textViewTime { get; set; }
        public TextView textViewMin { get; set; }
        public TextView textViewStatus { get; set; }
        public TextView textViewDestination { get; set; }
        public ImageView ImageViewTrain { get; set; }
        public TextView textViewLine { get; set; }
        public TextView statusIndicator { get; set; }
        public TextView textViewTime1 { get; set; }
        public TextView textViewTime2 { get; set; }
        public TextView textViewInfo { get; set; }

        public ConnectionsAdapterViewHolder(Android.Views.View itemView, Action<ConnectionsAdapterClickEventArgs> clickListener,
                            Action<ConnectionsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            linearLayoutRow = (LinearLayout)itemView.FindViewById(Resource.Id.linearLayoutRow);
            textViewHeader = (TextView)itemView.FindViewById(Resource.Id.textViewHeader);
            textViewTime = (TextView)itemView.FindViewById(Resource.Id.textViewTime);
            textViewMin = (TextView)itemView.FindViewById(Resource.Id.textViewMin);
            textViewStatus = (TextView)itemView.FindViewById(Resource.Id.textViewStatus);
            textViewDestination = (TextView)itemView.FindViewById(Resource.Id.textViewDestination);
            ImageViewTrain = (ImageView)itemView.FindViewById(Resource.Id.imageViewTrain);
            textViewLine = (TextView)itemView.FindViewById(Resource.Id.textViewLine);
            statusIndicator = (TextView)itemView.FindViewById(Resource.Id.statusIndicator);
            textViewTime1 = (TextView)itemView.FindViewById(Resource.Id.textViewTime1);
            textViewTime2 = (TextView)itemView.FindViewById(Resource.Id.textViewTime2);
            textViewInfo = (TextView)itemView.FindViewById(Resource.Id.textViewInfo);

            //TextView = v;
            linearLayoutRow.Click += (sender, e) => clickListener(new ConnectionsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            linearLayoutRow.LongClick += (sender, e) => longClickListener(new ConnectionsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class ConnectionsAdapterClickEventArgs : EventArgs
    {
        public Android.Views.View View { get; set; }
        public int Position { get; set; }
    }
}