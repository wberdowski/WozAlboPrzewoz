using System.IO;
using System.Net;

namespace WozAlboPrzewoz
{
    public class HttpUtils
    {
        public static string SendGETRequest(string url, out HttpStatusCode status)
        {
            HttpWebRequest req = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            status = response.StatusCode;

            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
