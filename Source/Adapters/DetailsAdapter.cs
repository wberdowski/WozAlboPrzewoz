using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;

namespace WozAlboPrzewoz
{
    public class DetailsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<DetailsAdapterClickEventArgs> ItemClick;
        public event EventHandler<DetailsAdapterClickEventArgs> ItemLongClick;
        List<StationSchedule> items;

        public DetailsAdapter(List<StationSchedule> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            Android.Views.View itemView = null;
            var id = Resource.Layout.recycler_row_details;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new DetailsAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as DetailsAdapterViewHolder;

            holder.textViewStationName.Text = item.Name;
            if (position == 0)
            {
                holder.textViewTime.Text = item.DepartureTime.ToShortTimeString();
            }
            else
            {
                holder.textViewTime.Text = item.ArrivalTime.ToShortTimeString();
            }
            holder.textViewTrack.Text = $"Peron {item.Platform} Tor {item.Track}";
            holder.textViewDelay.Text = $"+{item.Delay} min";
        }

        public override int ItemCount => items.Count;

        void OnClick(DetailsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(DetailsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class DetailsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView textViewStationName { get; set; }
        public TextView textViewTime { get; set; }
        public TextView textViewTrack { get; set; }
        public TextView textViewDelay { get; set; }

        public DetailsAdapterViewHolder(Android.Views.View itemView, Action<DetailsAdapterClickEventArgs> clickListener,
                            Action<DetailsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            textViewStationName = (TextView)itemView.FindViewById(Resource.Id.textViewStationName);
            textViewTime = (TextView)itemView.FindViewById(Resource.Id.textViewTime);
            textViewTrack = (TextView)itemView.FindViewById(Resource.Id.textViewTrack);
            textViewDelay = (TextView)itemView.FindViewById(Resource.Id.textViewDelay);
            //TextView = v;
            itemView.Click += (sender, e) => clickListener(new DetailsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new DetailsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class DetailsAdapterClickEventArgs : EventArgs
    {
        public Android.Views.View View { get; set; }
        public int Position { get; set; }
    }
}