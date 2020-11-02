using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Storage
{
    public class StorImage : IStorable
    {
        public byte[] Data;

        public Image Image
        {
            get
            {
                MemoryStream stream = new MemoryStream(Data);
                Image img = Image.FromStream(stream);
                stream.Close();
                return img;
            }
            set
            {
                MemoryStream _strm = new MemoryStream();
                value.Save(_strm, ImageFormat.Jpeg);
                Data = new byte[_strm.Length];
                _strm.Position = 0;
                _strm.Read(Data, 0, Data.Length);
                _strm.Close();
            }
        }

        public StorImage() { }

        public StorImage(Image img)
        {
            Image = img;
        }

        public void ReadFromData(DataStorage data)
        {
            Data = data.Read();
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(Data);
        }
    }
}
