using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Graphics;
using System.Threading;
using Xamarin.Forms;

namespace WozAlboPrzewoz
{
    public class ConnectionsRecyclerAdapter : RecyclerView.Adapter
    {
        public event EventHandler<RecyclerAdapterClickEventArgs> ItemClick;
        public event EventHandler<RecyclerAdapterClickEventArgs> ItemLongClick;
        List<TrainConnection> items;

        public ConnectionsRecyclerAdapter(List<TrainConnection> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            Android.Views.View itemView = null;
            var id = Resource.Layout.recycler_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new RecyclerAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as RecyclerAdapterViewHolder;

            var departureTime = DateTime.FromOADate(item.timeDeparture);
            var minutesLeft = Math.Ceiling(departureTime.AddMinutes(item.delay).Subtract(DateTime.Now).TotalMinutes);

            if (item.delay > 0)
            {
                holder.textViewStatus.Text = $"+{item.delay} min";
                holder.textViewStatus.SetTextColor(new Android.Graphics.Color(200, 0, 0));
            }
            else
            {
                holder.textViewStatus.Text = $"Na czas";
                holder.textViewStatus.SetTextColor(new Android.Graphics.Color(0, 170, 0));
            }

            if (minutesLeft < 0)
            {
                holder.textViewStatus.Text = $"Odjechał";
                holder.textViewStatus.SetTextColor(new Android.Graphics.Color(170, 170, 170));
            }
            else if (minutesLeft < 1)
            {
                holder.textViewStatus.Text = $"Odjeżdża";
                holder.textViewStatus.SetTextColor(new Android.Graphics.Color(170, 170, 170));
            }

            if (minutesLeft < 60)
            {
                holder.textViewTime.Text = $"{Math.Abs(minutesLeft)}";
                holder.textViewMin.Visibility = ViewStates.Visible;
            }
            else
            {
                holder.textViewTime.Text = departureTime.AddMinutes(item.delay).ToShortTimeString();
                holder.textViewMin.Visibility = ViewStates.Gone;
            }

            if (minutesLeft == 1)
            {
                holder.textViewTime.Text = $"<1";
            }

            holder.textViewDestination.Text = item.stationEnd;
            holder.textViewLine.Text = item.line;
            holder.textViewCarrier.Text = item.carrier;
        }

        public override int ItemCount => items.Count;

        void OnClick(RecyclerAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(RecyclerAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class RecyclerAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView textViewTime { get; set; }
        public TextView textViewMin { get; set; }
        public TextView textViewStatus { get; set; }
        public TextView textViewDestination { get; set; }
        public TextView textViewCarrier { get; set; }
        public TextView textViewLine { get; set; }

        public RecyclerAdapterViewHolder(Android.Views.View itemView, Action<RecyclerAdapterClickEventArgs> clickListener,
                            Action<RecyclerAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            textViewTime = (TextView)itemView.FindViewById(Resource.Id.textViewTime);
            textViewMin = (TextView)itemView.FindViewById(Resource.Id.textViewMin);
            textViewStatus = (TextView)itemView.FindViewById(Resource.Id.textViewStatus);
            textViewDestination = (TextView)itemView.FindViewById(Resource.Id.textViewDestination);
            textViewCarrier = (TextView)itemView.FindViewById(Resource.Id.textViewCarrier);
            textViewLine = (TextView)itemView.FindViewById(Resource.Id.textViewLine);
            //TextView = v;
            itemView.Click += (sender, e) => clickListener(new RecyclerAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RecyclerAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class RecyclerAdapterClickEventArgs : EventArgs
    {
        public Android.Views.View View { get; set; }
        public int Position { get; set; }
    }
}