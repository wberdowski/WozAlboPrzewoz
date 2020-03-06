using System;
using System.Collections.Generic;

namespace WozAlboPrzewoz
{
    public class ConnectionsEventArgs : EventArgs
    {
        public List<TrainConnectionListItem> Connections { get; set; }
        public Direction Direction { get; set; }

        public ConnectionsEventArgs()
        {

        }

        public ConnectionsEventArgs(List<TrainConnectionListItem> connections)
        {
            Connections = connections;
        }

        public ConnectionsEventArgs(List<TrainConnectionListItem> connections, Direction direction)
        {
            Connections = connections;
            Direction = direction;
        }
    }
}