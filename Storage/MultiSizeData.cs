using System;
using System.Threading;

namespace Island.StandardLib.Storage
{
    /// <summary>
    /// 提供一个线程安全的、可动态扩展、可垃圾回收且支持序列化的缓存区域
    /// </summary>
    public class MultiSizeData : IStorable
    {
        byte[] Data;

        /// <summary>
        /// 读取位置
        /// </summary>
        public int ReadPosition { get; set; }

        /// <summary>
        /// 未读取内容长度
        /// </summary>
        public int ReadRemainEnd => Data.Length - ReadPosition;

        /// <summary>
        /// 读取内容和缓存起点的距离
        /// </summary>
        public int ReadRemainBegin => ReadPosition;

        /// <summary>
        /// 写入位置
        /// </summary>
        public int WritePosition { get; set; }

        /// <summary>
        /// 未写入内容长度，此值在默认应用下应为0
        /// </summary>
        public int WriteRemainEnd => Data.Length - WritePosition;

        /// <summary>
        /// 写入内容和缓存起点的距离
        /// </summary>
        public int WriteRemainBegin => WritePosition;

        /// <summary>
        /// 从这个值开始，向前的内存区域已经完成写入和读取操作，可被释放
        /// </summary>
        public int FreePtr => ReadPosition > WritePosition ? WritePosition : ReadPosition;

        /// <summary>
        /// 当前缓存长度
        /// </summary>
        public int Size => Data.Length;

        /// <summary>
        /// 初始化缓存区域，初始大小为0
        /// </summary>
        public MultiSizeData()
        {
            Data = new byte[0];
            lck_itio = new object();
            lck_recvier = new object();
        }

        object lck_itio, lck_recvier;

        /// <summary>
        /// 向缓存的任意位置写入数据，若长度不足则拓展缓存
        /// </summary>
        /// <param name="begin">写入起点</param>
        /// <param name="data">写入的内容</param>
        public void WriteAnyWhere(int begin, byte[] data)
        {
            lock (lck_itio)
            {
                if (Data.Length < begin + data.Length)
                {
                    byte[] newData = new byte[begin + data.Length];
                    Array.Copy(Data, newData, Data.Length);
                    Data = newData;
                }
                Array.Copy(data, 0, Data, begin, data.Length);
            }
        }

        /// <summary>
        /// 从缓存区域的任意位置读取数据，若当前不存在指定的区域则引发异常
        /// </summary>
        /// <param name="begin">读取起点</param>
        /// <param name="writeTo">读取到的目标数组</param>
        /// <param name="offset">目标数组偏移量</param>
        /// <param name="size">读取长度</param>
        public void ReadAnyWhere(int begin, byte[] writeTo, int offset, int size)
        {
            lock (lck_itio)
            {
                Array.Copy(Data, begin, writeTo, offset, size);
            }
        }

        /// <summary>
        /// 向缓存区域的最后写入位置追加内容，若长度不足则拓展缓存
        /// </summary>
        /// <param name="data">追加的数据</param>
        public void Write(byte[] data)
        {
            lock (lck_recvier)
            {
                WriteAnyWhere(WritePosition, data);
                WritePosition += data.Length;
            }
        }

        /// <summary>
        /// 从缓存区域的最后读取位置读取指定长度的内容，若当前缓存长度不足则等待
        /// </summary>
        /// <param name="buffer">读取到的目标数组</param>
        /// <param name="offset">目标数组偏移量</param>
        /// <param name="size">读取长度</param>
        public void Read(byte[] buffer, int offset, int size)
        {
            while (ReadRemainEnd < size)
                Thread.Sleep(1);
            lock (lck_recvier)
            {
                ReadAnyWhere(ReadPosition, buffer, offset, size);
                ReadPosition += size;
            }
        }

        /// <summary>
        /// 释放当前已读写的缓存区域，并调整读取位置和写入位置
        /// </summary>
        public void FreeUnused()
        {
            lock (lck_recvier)
            {
                lock (lck_itio)
                {
                    int downSize = FreePtr;
                    if (downSize == 0) return;
                    byte[] newData = new byte[Size - downSize];
                    Array.Copy(Data, downSize, newData, 0, Size - downSize);
                    ReadPosition -= downSize;
                    WritePosition -= downSize;
                    Data = newData;
                }
            }
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out int read); ReadPosition = read;
            data.Read(out int write); WritePosition = write;
            Data = data.Read();
        }

        public void WriteToData(DataStorage data)
        {
            lock (lck_itio)
            {
                data.Write(ReadPosition);
                data.Write(WritePosition);
                data.Write(Data);
            }
        }
    }
}
