using Island.StandardLib.Math;
using System;
using System.Collections.Generic;
using System.Text;

namespace Island.StandardLib.Storage.Local
{
    public class StorPlayer : IStorable
    {
        public int Star;
        public string NickName, Password;

        public void ReadFromData(DataStorage data)
        {
            data.Read(out Star);
            data.Read(out NickName);
            data.Read(out Password);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(Star);
            data.Write(NickName);
            data.Write(Password);
        }

        public StorPlayerPublic CreatePublic()
        {
            StorPlayerPublic sp = new StorPlayerPublic();
            sp.NickName = NickName;
            sp.Star = Star;
            return sp;
        }
    }

    public class StorPlayerPublic : IStorable
    {
        public int Star;
        public string NickName;

        public void ReadFromData(DataStorage data)
        {
            data.Read(out Star);
            data.Read(out NickName);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(Star);
            data.Write(NickName);
        }
    }
}
