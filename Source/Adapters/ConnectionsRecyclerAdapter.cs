using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Content;

namespace WozAlboPrzewoz
{
    public class ConnectionsRecyclerAdapter : RecyclerView.Adapter
    {
        public event EventHandler<RecyclerAdapterClickEventArgs> ItemClick;
        public event EventHandler<RecyclerAdapterClickEventArgs> ItemLongClick;
        List<TrainConnectionListItem> items;
        Context context;

        public ConnectionsRecyclerAdapter(Context ctx, List<TrainConnectionListItem> data)
        {
            context = ctx;
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
            var connection = item.Connection;
            var holder = viewHolder as RecyclerAdapterViewHolder;

            var departureTime = NormalizeTime(DateTime.FromOADate(connection.timeDeparture));
            var delayNormalized = Math.Max(0, connection.delay);
            var minutesLeft = (int)Math.Ceiling(departureTime.Subtract(DateTime.Now).TotalMinutes) + delayNormalized;

            //
            //  Header
            //

            if (item.HasHeader)
            {
                holder.textViewHeader.Text = item.HeaderText;
                holder.textViewHeader.Visibility = ViewStates.Visible;
            }
            else
            {
                holder.textViewHeader.Visibility = ViewStates.Gone;
            }

            //
            //  Status
            //

            if (connection.delay > 0)
            {
                holder.textViewStatus.Text = context.GetString(Resource.String.delay, connection.delay);
                holder.textViewStatus.SetTextColor(new Android.Graphics.Color(context.GetColor(Resource.Color.colorStatusBadDark)));
            }
            else
            {
                if (connection.up.Length > 0)
                {
                    holder.textViewStatus.Text = context.GetString(Resource.String.difficulties);
                    holder.textViewStatus.SetTextColor(new Android.Graphics.Color(context.GetColor(Resource.Color.colorStatusBadDark)));
                }
                else
                {
                    holder.textViewStatus.Text = context.GetString(Resource.String.on_time);
                    holder.textViewStatus.SetTextColor(new Android.Graphics.Color(context.GetColor(Resource.Color.colorStatusGoodDark)));
                }
            }

            //
            //  Status indicator
            //

            if (connection.up.Length > 0)
            {
                holder.statusIndicator.SetBackgroundResource(Resource.Color.colorStatusBad);
            }
            else
            {
                if (minutesLeft < 1)
                {
                    holder.statusIndicator.SetBackgroundResource(Resource.Color.colorStatusDim);
                }
                else
                {
                    holder.statusIndicator.SetBackgroundResource(Resource.Color.colorStatusGood);
                }
            }

            //
            //  Time or minutes left
            //

            if (minutesLeft < 60)
            {
                holder.textViewTime.Text = Math.Abs(minutesLeft).ToString();
                holder.textViewMin.Visibility = ViewStates.Visible;
            }
            else
            {
                holder.textViewTime.Text = departureTime.AddMinutes(delayNormalized).ToShortTimeString();
                holder.textViewMin.Visibility = ViewStates.Gone;
            }

            holder.textViewDestination.Text = connection.stationEnd;
            holder.textViewLine.Text = connection.line;
            holder.textViewCarrier.Text = connection.carrier;

            holder.textViewTime1.Text = departureTime.ToShortTimeString();
            holder.textViewTime2.Text = DateTime.FromOADate(connection.timeArrivalEnd).ToShortTimeString();
        }

        private DateTime NormalizeTime(DateTime time)
        {
            return time.AddSeconds(-time.Second);
        }

        public override int ItemCount => items.Count;

        void OnClick(RecyclerAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(RecyclerAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class RecyclerAdapterViewHolder : RecyclerView.ViewHolder
    {
        public CardView cardViewRow { get; set; }
        public TextView textViewHeader { get; set; }
        public TextView textViewTime { get; set; }
        public TextView textViewMin { get; set; }
        public TextView textViewStatus { get; set; }
        public TextView textViewDestination { get; set; }
        public TextView textViewCarrier { get; set; }
        public TextView textViewLine { get; set; }
        public LinearLayout statusIndicator { get; set; }
        public TextView textViewTime1 { get; set; }
        public TextView textViewTime2 { get; set; }

        public RecyclerAdapterViewHolder(Android.Views.View itemView, Action<RecyclerAdapterClickEventArgs> clickListener,
                            Action<RecyclerAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            cardViewRow = (CardView)itemView.FindViewById(Resource.Id.cardViewRow);
            textViewHeader = (TextView)itemView.FindViewById(Resource.Id.textViewHeader);
            textViewTime = (TextView)itemView.FindViewById(Resource.Id.textViewTime);
            textViewMin = (TextView)itemView.FindViewById(Resource.Id.textViewMin);
            textViewStatus = (TextView)itemView.FindViewById(Resource.Id.textViewStatus);
            textViewDestination = (TextView)itemView.FindViewById(Resource.Id.textViewDestination);
            textViewCarrier = (TextView)itemView.FindViewById(Resource.Id.textViewCarrier);
            textViewLine = (TextView)itemView.FindViewById(Resource.Id.textViewLine);
            statusIndicator = (LinearLayout)itemView.FindViewById(Resource.Id.statusIndicator);
            textViewTime1 = (TextView)itemView.FindViewById(Resource.Id.textViewTime1);
            textViewTime2 = (TextView)itemView.FindViewById(Resource.Id.textViewTime2);

            //TextView = v;
            cardViewRow.Click += (sender, e) => clickListener(new RecyclerAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            cardViewRow.LongClick += (sender, e) => longClickListener(new RecyclerAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class RecyclerAdapterClickEventArgs : EventArgs
    {
        public Android.Views.View View { get; set; }
        public int Position { get; set; }
    }
}