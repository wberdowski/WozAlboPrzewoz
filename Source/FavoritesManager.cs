using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WozAlboPrzewoz
{
    public class FavoritesManager
    {
        public static List<Station> favorites = new List<Station>();
        public static event EventHandler<EventArgs> FavoritesChanged;

        public static int AddFavorite(Station station)
        {
            if (!HasFavorite(station))
            {
                favorites.Add(station);
                Commit();
                return favorites.Count - 1;
            }
            return -1;
        }

        public static int RemoveFavorite(Station station)
        {
            if (HasFavorite(station))
            {
                int idx = favorites.FindIndex(x => x.id == station.id);
                favorites.RemoveAt(idx);
                Commit();
                return idx;
            }
            return -1;
        }

        private static void Commit()
        {
            FavoritesChanged?.Invoke(null, null);
            List<int> arr = new List<int>();

            foreach (Station s in favorites)
            {
                arr.Add(s.id);
            }

            var serialized = JsonConvert.SerializeObject(arr);
            File.WriteAllText(GetFilePath(), serialized);
        }

        public static bool HasFavorite(Station station)
        {
            return favorites.Where(x => x.id == station.id).Count() > 0;
        }

        public static void Load()
        {
            if (!File.Exists(GetFilePath()))
            {
                File.Create(GetFilePath());
            }

            var arr = JsonConvert.DeserializeObject<List<int>>(File.ReadAllText(GetFilePath()));
            if (arr != null)
            {
                foreach (int sid in arr)
                {
                    favorites.Add(StationsCache.Stations.Where(x => x.id == sid).FirstOrDefault());
                }
            }
        }

        private static string GetFilePath()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(path, "favorites.json");
        }
    }
}