namespace WozAlboPrzewoz
{
    public interface IJsonModel<T>
    {
        T FromJson(string str);
    }
}