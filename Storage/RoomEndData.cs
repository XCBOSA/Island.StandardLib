using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Storage
{
    public class RoomEndData : IStorable
    {
        public int YouGetStar;
        public RoomEndReason Reason;

        public RoomEndData() { }

        public RoomEndData(int getStar, RoomEndReason reason)
        {
            YouGetStar = getStar;
            Reason = reason;
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out YouGetStar);
            data.Read(out int d); Reason = (RoomEndReason)d;
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(YouGetStar);
            data.Write((int)Reason);
        }
    }

    public enum RoomEndReason
    {
        PlayerDisconnected,
        ArmyVictory,
        Overthrow,
        PeopleDiedTooMuch
    }
}
