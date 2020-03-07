using Android.Graphics.Drawables;
using Android.Widget;
using System;

namespace WozAlboPrzewoz
{
    public class RouteProgressHelper
    {
        public static void SetType(FrameLayout layout, RouteProgressType type)
        {
            var background = (ImageView)layout.FindViewById(Resource.Id.imageViewProgressBackground);
            var foreground = (ImageView)layout.FindViewById(Resource.Id.imageViewProgressForeground);

            switch (type)
            {
                case RouteProgressType.Normal:
                    background.SetImageResource(Resource.Drawable.route_progress_background);
                    foreground.SetImageResource(Resource.Drawable.route_progress_clip);
                    break;

                case RouteProgressType.Start:
                    background.SetImageResource(Resource.Drawable.route_progress_background_start);
                    foreground.SetImageResource(Resource.Drawable.route_progress_clip_start);
                    break;

                case RouteProgressType.End:
                    background.SetImageResource(Resource.Drawable.route_progress_background_end);
                    foreground.SetImageResource(Resource.Drawable.route_progress_clip_end);
                    break;
            }
        }

        public static ClipDrawable GetDrawable(FrameLayout layout)
        {
            return (ClipDrawable)((ImageView)layout.FindViewById(Resource.Id.imageViewProgressForeground)).Drawable;
        }

        public static void SetProgress(ClipDrawable drawable, double progress)
        {
            drawable.SetLevel(Math.Max(0, Math.Min(10000, (int)(progress * 100))));
        }
    }

    public enum RouteProgressType
    {
        Normal,
        Start,
        End
    }
}