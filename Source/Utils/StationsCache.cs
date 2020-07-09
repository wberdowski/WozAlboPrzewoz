using Newtonsoft.Json;
using System.IO;

namespace WozAlboPrzewoz
{
    public class StationsCache
    {
        public static Station[] Stations { get; private set; }

        public static void Load()
        {
            //
            //  Load stations dictionary
            //

            //Cache stations list
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            string settingsPath = Path.Combine(path, "stations.json");

            if (!File.Exists(settingsPath))
            {
                Stations = PKPAPI.GetStations();
                StreamWriter stream = File.CreateText(settingsPath);
                stream.Write(JsonConvert.SerializeObject(Stations));
                stream.Close();
            }
            else
            {
                string text = File.ReadAllText(settingsPath);
                Stations = JsonConvert.DeserializeObject<Station[]>(text);
            }
        }
    }
}