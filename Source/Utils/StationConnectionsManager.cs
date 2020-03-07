using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace WozAlboPrzewoz
{
    public class StationConnectionsManager
    {
        public Station Station { get; private set; }
        public DateTime Time { get; private set; }
        public List<TrainConnectionListItem> Connections { get; private set; }

        public event EventHandler<ConnectionsEventArgs> UpdateEnd;
        public event EventHandler<WebExceptionEventArgs> HttpError;

        public StationConnectionsManager(Station station, DateTime time)
        {
            Station = station;
            Time = time;
            Connections = new List<TrainConnectionListItem>();
        }

        #region Public Methods

        public void Update()
        {
            Connections.Clear();
            LoadPrimary();
        }

        public void SetTime(DateTime time)
        {
            Time = time;
        }

        #endregion

        private void LoadPrimary()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    var connections = PKPAPI.GetStationTimetable(Station.Id, Time);

                    DateTime lastDate = DateTime.Now;

                    for (int i = 0; i < connections.Length; i++)
                    {
                        var conn = connections[i];
                        var item = new TrainConnectionListItem(conn);
                        var date = DateTime.FromOADate(conn.TimeDeparture).Date;

                        lastDate = date;
                        Connections.Add(item);
                    }

                    CleanUpData();
                    UpdateEnd?.Invoke(this, new ConnectionsEventArgs(Connections));
                }
                catch (WebException ex)
                {
                    HttpError?.Invoke(this, new WebExceptionEventArgs(ex));
                }
            }).Start();
        }

        public void LoadPrevious()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    var firstConn = Connections.First().Connection;
                    var connections = PKPAPI.GetStationTimetable(Station.Id, DateTime.FromOADate(firstConn.TimeDeparture), 0, 10);

                    var list = new List<TrainConnectionListItem>();

                    foreach (var conn in connections)
                    {
                        list.Add(new TrainConnectionListItem(conn));
                    }

                    Connections.InsertRange(0, list);
                    CleanUpData();
                    UpdateEnd?.Invoke(this, new ConnectionsEventArgs(Connections, Direction.Up));
                }
                catch (WebException ex)
                {
                    HttpError?.Invoke(this, new WebExceptionEventArgs(ex));
                }
            }).Start();

        }

        public void LoadLater()
        {
            new System.Threading.Thread(() =>
            {
                try
                {
                    var lastConn = Connections.Last().Connection;
                    var connections = PKPAPI.GetStationTimetable(Station.Id, DateTime.FromOADate(lastConn.TimeDeparture), 2, 10);
                    var previousLastIndex = Connections.Count - 1;

                    foreach (var conn in connections)
                    {
                        if (lastConn.Sknnt == conn.Sknnt && lastConn.Spnnt == conn.Spnnt && lastConn.TrainNumber == conn.TrainNumber) continue;

                        Connections.Add(new TrainConnectionListItem(conn));
                    }

                    CleanUpData();
                    UpdateEnd?.Invoke(this, new ConnectionsEventArgs(Connections, Direction.Down));
                }
                catch (WebException ex)
                {
                    HttpError?.Invoke(this, new WebExceptionEventArgs(ex));
                }
            }).Start();
        }

        private void CleanUpData()
        {
            DateTime lastDate = DateTime.Now;

            Connections.Sort((a, b) =>
            {
                return System.Math.Sign(TimeUtils.DiscardSeconds(DateTime.FromOADate(a.Connection.TimeDeparture))
                    .AddMinutes(System.Math.Max(0, a.Connection.DelayStart))
                    .Subtract(DateTime.FromOADate(b.Connection.TimeDeparture)
                    .AddMinutes(System.Math.Max(0, b.Connection.DelayStart))).TotalMinutes);
            });

            foreach (var item in Connections)
            {
                var date = DateTime.FromOADate(item.Connection.TimeDeparture).Date;

                if (lastDate < date)
                {
                    item.HasHeader = true;
                    item.HeaderText = date.ToLongDateString();
                }

                lastDate = date;
            }
        }
    }
}