using System;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using Android.Graphics.Drawables;
using Android.OS;
using Java.Lang;

namespace WozAlboPrzewoz
{
    public class DetailsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<DetailsAdapterClickEventArgs> ItemClick;
        public event EventHandler<DetailsAdapterClickEventArgs> ItemLongClick;
        TrainConnection connection;
        List<StationSchedule> items;
        public Dictionary<int, Action> actions = new Dictionary<int, Action>();

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

            //if (position == 0)
            //{
            //    time = item.DepartureTime;
            //    delay = Math.Max(0, connection.DelayStart);
            //}
            //else
            {
                time = item.ArrivalTime;
                delay = System.Math.Max(0, item.Delay);
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
                holder.textViewStatus.SetTextAppearance(Resource.Style.StatusTextBad);
            }
            else
            {
                holder.textViewStatus.Text = context.GetString(Resource.String.on_time);
                holder.textViewStatus.SetTextAppearance(Resource.Style.StatusTextGood);
            }

            //
            //  Progress
            //

            var mImageDrawable = (ClipDrawable)holder.imageViewProgressForeground.Drawable;

            Action a = () =>
            {
                if (items.Count > position + 1)
                {
                    var nextStation = items[position + 1];
                    var nextStationArrivalTime = nextStation.ArrivalTime.AddMinutes(nextStation.Delay);
                    var currentStationDepartureTime = item.DepartureTime.AddMinutes(item.Delay);
                    var totalSeconds = nextStationArrivalTime.Subtract(currentStationDepartureTime).TotalSeconds;
                    var secondsLeft = nextStationArrivalTime.Subtract(DateTime.Now).TotalSeconds;

                    mImageDrawable.SetLevel(10000 - (int)((secondsLeft / (float)totalSeconds) * 10000));
                }
            };
            a.Invoke();
            actions[position] = a;

            //
            //  Description
            //

            holder.textViewTrack.Text = $"Peron {item.Platform} Tor {item.Track}";

            holder.textViewCount.Text = position.ToString();
        }

        public void UpdateProgress()
        {
            foreach (var a in actions)
            {
                a.Value.Invoke();
            }
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
        public ImageView imageViewProgressForeground { get; set; }

        public DetailsAdapterViewHolder(Android.Views.View itemView, Action<DetailsAdapterClickEventArgs> clickListener,
                            Action<DetailsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            textViewStationName = (TextView)itemView.FindViewById(Resource.Id.textViewStationName);
            textViewTime = (TextView)itemView.FindViewById(Resource.Id.textViewTime);
            textViewTrack = (TextView)itemView.FindViewById(Resource.Id.textViewTrack);
            textViewStatus = (TextView)itemView.FindViewById(Resource.Id.textViewStatus);
            textViewCount = (TextView)itemView.FindViewById(Resource.Id.textViewCount);
            imageViewProgressForeground = (ImageView)itemView.FindViewById(Resource.Id.imageViewProgressForeground);
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