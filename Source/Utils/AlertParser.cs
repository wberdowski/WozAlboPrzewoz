using System.Text.RegularExpressions;

namespace WozAlboPrzewoz
{
    public class AlertParser
    {
        private static Regex regBeginnig = new Regex(@"^\@.*?\#+(.*?)$", RegexOptions.Compiled);
        private static Regex regGuid = new Regex(@"([a-z0-9]{8}\-[a-z0-9]{4}\-[a-z0-9]{4}\-[a-z0-9]{4}\-[a-z0-9]{12})", RegexOptions.Compiled);

        public static string Parse(string text)
        {
            var match = regBeginnig.Match(text);
            var result = match.Groups[1].ToString();

            var result2 = regGuid.Replace(result, new MatchEvaluator(Evaluate));
            result2 = result2.Replace("#", "");
            return result2;
        }

        private static string Evaluate(Match match)
        {
            var guid = match.Groups[1].ToString();
            if (TranslationsCache.TryGetByGuid(guid, out TranslationPhrase phrase))
            {
                return phrase.T;
            }

            return string.Empty;
        }
    }
}