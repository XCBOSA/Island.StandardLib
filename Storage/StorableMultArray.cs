using System;
using System.Collections.Generic;
using System.Text;

namespace Island.StandardLib.Storage
{
    /// <summary>
    /// 表示一个每项长度不固定的可序列化变长数组（不定项长可变数组）
    /// </summary>
    public class StorableMultArray : IStorable
    {
        List<MultData> array;

        /// <summary>
        /// 初始化不定项长可变数组
        /// </summary>
        public StorableMultArray()
        {
            array = new List<MultData>();
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out int size);
            for (int i = 0; i < size; i++)
                array.Add(new MultData(data.Read()));
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(array.Count);
            for (int i = 0; i < array.Count; i++)
                data.Write(array[i].Data);
        }

        public void Add(IStorable data) => array.Add(new MultData(data));

        public void Add(MultData data) => array.Add(data);
        public void Add(byte[] data) => array.Add(new MultData(data));
        public void RemoveAt(int index) => array.RemoveAt(index);
        public void Clear() => array.Clear();
        public int Count => array.Count;
        public int Size => array.Count;
        public int Length => array.Count;
        public MultData this[int index] => array[index];
    }

    /// <summary>
    /// 表示一个不定项长可变数组的项
    /// </summary>
    public class MultData
    {
        public byte[] Data;

        public MultData(byte[] data) => Data = data;
        public MultData(int data) => Data = BitConverter.GetBytes(data);
        public MultData(char data) => Data = BitConverter.GetBytes(data);
        public MultData(bool data) => Data = BitConverter.GetBytes(data);
        public MultData(float data) => Data = BitConverter.GetBytes(data);
        public MultData(string data) => Data = Encoding.UTF8.GetBytes(data);
        public MultData(IStorable data)
        {
            DataStorage ds = new DataStorage();
            data.WriteToData(ds);
            Data = ds.Bytes;
        }

        public int AsInt() => BitConverter.ToInt32(Data, 0);
        public char AsChar() => BitConverter.ToChar(Data, 0);
        public bool AsBool() => BitConverter.ToBoolean(Data, 0);
        public float AsFloat() => BitConverter.ToSingle(Data, 0);
        public string AsString() => Encoding.UTF8.GetString(Data);
        public T As<T>() where T : IStorable, new() => Data.ReadData<T>();
    }
}
