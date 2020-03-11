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

        public static void AddFavorite(Station station)
        {
            if (!HasFavorite(station))
            {
                favorites.Add(station);
                Commit();
            }
        }

        public static void AddFavoriteAt(Station station, int index)
        {
            if (!HasFavorite(station))
            {
                favorites.Insert(index, station);
                Commit();
            }
        }

        public static int RemoveFavorite(Station station)
        {
            if (HasFavorite(station))
            {
                int idx = favorites.FindIndex(x => x.Id == station.Id);
                favorites.RemoveAt(idx);
                Commit();
                return idx;
            }
            return -1;
        }

        public static void Commit(bool notify = true)
        {
            if (notify)
                FavoritesChanged?.Invoke(null, null);

            List<int> arr = new List<int>();

            foreach (Station s in favorites)
            {
                arr.Add(s.Id);
            }

            var serialized = JsonConvert.SerializeObject(arr);
            File.WriteAllText(GetFilePath(), serialized);
        }

        public static bool HasFavorite(Station station)
        {
            return favorites.Where(x => x.Id == station.Id).Count() > 0;
        }

        public static void Load()
        {
            using (var reader = new StreamReader(File.Open(GetFilePath(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
            {
                var arr = JsonConvert.DeserializeObject<List<int>>(reader.ReadToEnd());
                if (arr != null)
                {
                    favorites.Clear();

                    foreach (int sid in arr)
                    {
                        favorites.Add(StationsCache.Stations.Where(x => x.Id == sid).FirstOrDefault());
                    }
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