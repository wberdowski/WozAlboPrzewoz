using System;
using Android.Content;
using Android.Views;

namespace WozAlboPrzewoz
{
    public class ConnectionItemHelper
    {
        public static void BindViewHolder(Context context, TrainConnectionListItem item, ConnectionsAdapterViewHolder holder)
        {
            var connection = item.Connection;
            var departureTime = TimeUtils.DiscardSeconds(DateTime.FromOADate(connection.TimeDeparture));
            var delayNormalized = Math.Max(0, connection.DelayStart);
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
            if (minutesLeft < 0)
            {
                holder.textViewStatus.Text = context.GetString(Resource.String.train_left);
                holder.textViewStatus.SetTextAppearance(Resource.Style.StatusText);
            }
            else
            {
                if (connection.DelayStart > 0)
                {
                    holder.textViewStatus.Text = context.GetString(Resource.String.delay, connection.DelayStart);
                    holder.textViewStatus.SetTextAppearance(Resource.Style.StatusTextBad);
                }
                else
                {
                    if (connection.Up.Length > 0)
                    {
                        holder.textViewStatus.Text = context.GetString(Resource.String.difficulties);
                        holder.textViewStatus.SetTextAppearance(Resource.Style.StatusTextBad);
                    }
                    else
                    {
                        holder.textViewStatus.Text = context.GetString(Resource.String.on_time);
                        holder.textViewStatus.SetTextAppearance(Resource.Style.StatusTextGood);
                    }
                }
            }

            //
            //  Status indicator
            //
            if (minutesLeft < 0)
            {
                holder.statusIndicator.SetBackgroundColor(AttrValueUtils.GetColor(Resource.Attribute.colorStatusIndicatorFaint));
            }
            else
            {
                if (connection.Up.Length > 0)
                {
                    holder.statusIndicator.SetBackgroundColor(AttrValueUtils.GetColor(Resource.Attribute.colorStatusIndicatorBad));
                }
                else
                {
                    holder.statusIndicator.SetBackgroundColor(AttrValueUtils.GetColor(Resource.Attribute.colorStatusIndicatorGood));
                }
            }

            //
            //  Time or minutes left
            //

            if (Math.Abs(minutesLeft) < 60)
            {
                if (minutesLeft == 0)
                {
                    holder.textViewTime.Text = context.GetString(Resource.String.now);
                    holder.textViewMin.Visibility = ViewStates.Gone;
                }
                else
                {
                    holder.textViewTime.Text = Math.Abs(minutesLeft).ToString();
                    holder.textViewMin.Visibility = ViewStates.Visible;
                }
            }
            else
            {
                holder.textViewTime.Text = departureTime.AddMinutes(delayNormalized).ToShortTimeString();
                holder.textViewMin.Visibility = ViewStates.Gone;
            }

            if (minutesLeft < 0)
            {
                holder.textViewTime.SetTextAppearance(Resource.Style.TimeFaint);
                holder.textViewMin.SetTextAppearance(Resource.Style.MinFaint);
            }
            else
            {
                holder.textViewTime.SetTextAppearance(Resource.Style.TimeNormal);
                holder.textViewMin.SetTextAppearance(Resource.Style.MinNormal);
            }

            holder.textViewDestination.Text = connection.StationEnd;
            holder.textViewLine.Text = connection.Line;
            holder.textViewTime1.Text = departureTime.ToShortTimeString();
            holder.textViewTime2.Text = DateTime.FromOADate(connection.TimeArrivalEnd).ToShortTimeString();
            holder.textViewInfo.Text = TimeUtils.DiscardSeconds(DateTime.FromOADate(connection.TimeArrivalEnd)).Subtract(departureTime).ToString(@"hh\:mm");

            if (connection.DelayStart > 0)
            {
                holder.textViewTime1.SetTextAppearance(Resource.Style.TimeBadgeBad);
                holder.textViewTime1.SetBackgroundResource(Resource.Drawable.time_badge_bad);
            }
            else
            {
                holder.textViewTime1.SetTextAppearance(Resource.Style.TimeBadgeGood);
                holder.textViewTime1.SetBackgroundResource(Resource.Drawable.time_badge_good);
            }

            if (connection.DelayEnd > 0)
            {
                holder.textViewTime2.SetTextAppearance(Resource.Style.TimeBadgeBad);
                holder.textViewTime2.SetBackgroundResource(Resource.Drawable.time_badge_bad);
            }
            else
            {
                holder.textViewTime2.SetTextAppearance(Resource.Style.TimeBadgeGood);
                holder.textViewTime2.SetBackgroundResource(Resource.Drawable.time_badge_good);
            }
        }
    }
}