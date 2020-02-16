namespace WozAlboPrzewoz
{
    public class TrainConnectionListItem
    {
        public TrainConnection Connection { get; set; }
        public bool HasHeader { get; set; }
        public string HeaderText { get; set; }

        public TrainConnectionListItem()
        {

        }

        public TrainConnectionListItem(TrainConnection connection)
        {
            Connection = connection;
        }

        public TrainConnectionListItem(TrainConnection connection, bool hasHeader, string headerText)
        {
            Connection = connection;
            HasHeader = hasHeader;
            HeaderText = headerText;
        }
    }
}