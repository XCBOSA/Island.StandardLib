using Island.StandardLib.Storage.Local;

namespace Island.StandardLib.Storage
{
    public class RoomPreparedData : IStorable
    {
        public StorPlayerPublic OtherPlayer;
        public int AfterClock;

        public RoomPreparedData() { }

        public RoomPreparedData(StorPlayerPublic other, int afterClock)
        {
            OtherPlayer = other;
            AfterClock = afterClock;
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out OtherPlayer);
            data.Read(out AfterClock);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(OtherPlayer);
            data.Write(AfterClock);
        }
    }
}
