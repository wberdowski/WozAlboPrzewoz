﻿using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;

namespace WozAlboPrzewoz
{
    public class FavoritesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ConnectionsAdapterClickEventArgs> ItemClick;
        public event EventHandler<ConnectionsAdapterClickEventArgs> ItemLongClick;

        private List<Station> items;

        public FavoritesAdapter(List<Station> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            Android.Views.View itemView = null;
            var id = Resource.Layout.recycler_row_favorites;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new FavoritesAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as FavoritesAdapterViewHolder;

            holder.textViewText.Text = item.Name;
        }

        public override int ItemCount => items.Count;

        private void OnClick(ConnectionsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        private void OnLongClick(ConnectionsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class FavoritesAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView textViewText { get; set; }

        public FavoritesAdapterViewHolder(Android.Views.View itemView, Action<ConnectionsAdapterClickEventArgs> clickListener,
                            Action<ConnectionsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            textViewText = (TextView)itemView.FindViewById(Resource.Id.textViewFavoriteText);
            //TextView = v;
            itemView.Click += (sender, e) => clickListener(new ConnectionsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ConnectionsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class FavoritesAdapterClickEventArgs : EventArgs
    {
        public Android.Views.View View { get; set; }
        public int Position { get; set; }
    }
}