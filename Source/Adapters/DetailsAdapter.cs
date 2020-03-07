using System;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using AndroidX.RecyclerView.Widget;
using Android.Graphics;

namespace WozAlboPrzewoz
{
    public class DetailsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<DetailsAdapterClickEventArgs> ItemClick;
        public event EventHandler<DetailsAdapterClickEventArgs> ItemLongClick;
        TrainConnection connection;
        List<StationSchedule> items;
        Station selectedStation;
        public Dictionary<int, Action> actions = new Dictionary<int, Action>();

        public DetailsAdapter(TrainConnection connection, Station selectedStation, List<StationSchedule> items)
        {
            this.connection = connection;
            this.selectedStation = selectedStation;
            this.items = items;
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

            if (item.Name == selectedStation.Name)
            {
                holder.textViewStationName.SetTextColor(new Color(context.GetColor(Resource.Color.colorRouteProgressForeground)));
            }

            DateTime arrivalTime, departureTime;
            int delay;

            if (position == 0)
            {
                departureTime = item.DepartureTime;
                delay = Math.Max(0, connection.DelayStart);
            }
            else if (position == ItemCount - 1)
            {
                departureTime = item.ArrivalTime;
                delay = Math.Max(0, item.DelayDeparture);
            }
            else
            {
                arrivalTime = item.ArrivalTime;
                departureTime = item.DepartureTime;
                delay = Math.Max(0, item.DelayDeparture);
            }

            //
            //  Time
            //

            holder.textViewTime.Text = departureTime.ToShortTimeString();

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

            if (position == 0)
                RouteProgressHelper.SetType(holder.routeProgress, RouteProgressType.Start);
            else if (ItemCount - 1 == position)
                RouteProgressHelper.SetType(holder.routeProgress, RouteProgressType.End);
            else
                RouteProgressHelper.SetType(holder.routeProgress, RouteProgressType.Normal);

            var drawable = RouteProgressHelper.GetDrawable(holder.routeProgress);

            StationSchedule previousStation, nextStation;

            if (position - 1 < 0)
            {
                previousStation = items[0];
                previousStation.ArrivalTime = previousStation.DepartureTime.AddMinutes(previousStation.DelayDeparture);
            }
            else
            {
                previousStation = items[position - 1];
            }

            var previousStationRealDepartureTime = previousStation.DepartureTime.AddMinutes(previousStation.DelayDeparture);

            if (position + 1 > ItemCount - 1)
            {
                nextStation = items[ItemCount - 1];
                nextStation.DepartureTime = nextStation.ArrivalTime.AddMinutes(nextStation.DelayArrival);
            }
            else
            {
                nextStation = items[position + 1];
            }

            var nextStationRealArrivalTime = nextStation.ArrivalTime.AddMinutes(nextStation.DelayArrival);

            var currentStationRealArrivalTime = item.ArrivalTime.AddMinutes(item.DelayArrival);
            var currentStationRealDepartureTime = item.DepartureTime.AddMinutes(item.DelayDeparture);

            Action a = () =>
            {
                //
                //  Top
                //
                var totalTop = currentStationRealArrivalTime.Subtract(previousStationRealDepartureTime).TotalMilliseconds;
                var positionTop = totalTop - currentStationRealArrivalTime.Subtract(DateTime.Now).TotalMilliseconds;

                //
                //  Bottom
                //
                var totalBottom = nextStationRealArrivalTime.Subtract(currentStationRealDepartureTime).TotalMilliseconds;
                var positionBottom = totalBottom - nextStationRealArrivalTime.Subtract(DateTime.Now).TotalMilliseconds;

                var top = (positionTop / (float)totalTop * 100).Clamp(50, 100) - 50;
                var bottom = (positionBottom / (float)totalBottom * 100).Clamp(0, 50);

                RouteProgressHelper.SetProgress(drawable, top + bottom);
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
        public FrameLayout routeProgress { get; set; }

        public DetailsAdapterViewHolder(Android.Views.View itemView, Action<DetailsAdapterClickEventArgs> clickListener,
                            Action<DetailsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            textViewStationName = (TextView)itemView.FindViewById(Resource.Id.textViewStationName);
            textViewTime = (TextView)itemView.FindViewById(Resource.Id.textViewTime);
            textViewTrack = (TextView)itemView.FindViewById(Resource.Id.textViewTrack);
            textViewStatus = (TextView)itemView.FindViewById(Resource.Id.textViewStatus);
            textViewCount = (TextView)itemView.FindViewById(Resource.Id.textViewCount);
            routeProgress = (FrameLayout)itemView.FindViewById(Resource.Id.routeProgress);
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