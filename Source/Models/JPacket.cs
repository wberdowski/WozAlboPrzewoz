using Newtonsoft.Json;

namespace WozAlboPrzewoz
{
    public abstract class JPacket
    {
        public virtual string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static T Deserialize<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text);
        }
    }
}
