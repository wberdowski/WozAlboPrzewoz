using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using AndroidX.Core.Content;

namespace WozAlboPrzewoz
{
    public class DetailsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<DetailsAdapterClickEventArgs> ItemClick;
        public event EventHandler<DetailsAdapterClickEventArgs> ItemLongClick;
        TrainConnection connection;
        List<StationSchedule> items;

        public DetailsAdapter(TrainConnection connection, List<StationSchedule> data)
        {
            this.connection = connection;
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
            var context = holder.textViewStatus.Context;

            holder.textViewStationName.Text = item.Name;

            DateTime time;
            int delay;

            if (position == 0)
            {
                time = item.DepartureTime;
                delay = Math.Max(0, connection.Delay);
            }
            else
            {
                time = item.ArrivalTime;
                delay = Math.Max(0, item.Delay);
            }

            //
            //  Time
            //

            holder.textViewTime.Text = time.ToShortTimeString();

            //
            //  Status
            //

            if (delay > 0)
            {
                holder.textViewStatus.Text = context.GetString(Resource.String.delay, delay);
                holder.textViewStatus.SetTextColor(new Android.Graphics.Color(context.GetColor(Resource.Color.colorStatusBadDark)));
            }
            else
            {
                holder.textViewStatus.Text = context.GetString(Resource.String.on_time);
                holder.textViewStatus.SetTextColor(new Android.Graphics.Color(context.GetColor(Resource.Color.colorStatusGoodDark)));
            }

            //
            //  Description
            //

            holder.textViewTrack.Text = $"Peron {item.Platform} Tor {item.Track}";

            holder.textViewCount.Text = position.ToString();
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
        public TextView textViewStatus { get; set; }
        public TextView textViewCount { get; set; }

        public DetailsAdapterViewHolder(Android.Views.View itemView, Action<DetailsAdapterClickEventArgs> clickListener,
                            Action<DetailsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            textViewStationName = (TextView)itemView.FindViewById(Resource.Id.textViewStationName);
            textViewTime = (TextView)itemView.FindViewById(Resource.Id.textViewTime);
            textViewTrack = (TextView)itemView.FindViewById(Resource.Id.textViewTrack);
            textViewStatus = (TextView)itemView.FindViewById(Resource.Id.textViewStatus);
            textViewCount = (TextView)itemView.FindViewById(Resource.Id.textViewCount);
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