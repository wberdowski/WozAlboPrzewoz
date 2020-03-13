using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace WozAlboPrzewoz
{
    public class TranslationsCache
    {
        public static TranslationPhrase[] Phrases { get; private set; }

        public static void Load()
        {
            //
            //  Load stations dictionary
            //

            //Cache stations list
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            string settingsPath = Path.Combine(path, "translations.json");

            if (!File.Exists(settingsPath))
            {
                Phrases = PKPAPI.GetTranslations();
                StreamWriter stream = File.CreateText(settingsPath);
                stream.Write(JsonConvert.SerializeObject(Phrases));
                stream.Close();
            }
            else
            {
                string text = File.ReadAllText(settingsPath);
                Phrases = JsonConvert.DeserializeObject<TranslationPhrase[]>(text);
            }
        }

        public static bool TryGetByGuid(string guid, out TranslationPhrase phrase)
        {
            phrase = Phrases.Where(x => x.N.ToLower() == guid).DefaultIfEmpty(null).FirstOrDefault();
            return phrase != null;
        }
    }
}