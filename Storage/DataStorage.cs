using Island.StandardLib.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Island.StandardLib.Storage
{
    /// <summary>
    /// 表示一个提供读写操作的序列化数据容器
    /// </summary>
    public class DataStorage
    {
        List<byte> Data;
        public int Position { get; private set; }

        public DataStorage()
        {
            Data = new List<byte>();
        }

        public DataStorage(byte[] data)
        {
            Data = new List<byte>(data);
        }

        public byte[] Bytes => Data.ToArray();
        public int Size => Data.Count;

        public void ReadInternal(byte[] data, int size)
        {
            for (int i = 0; i < size; i++)
                data[i] = Data[i + Position];
            Position += size;
        }

        public void WriteInternal(byte[] data)
        {
            for (int i = 0; i < data.Length; i++)
                Data.Add(data[i]);
        }

        void WriteInternal(List<byte> data)
        {
            for (int i = 0; i < data.Count; i++)
                Data.Add(data[i]);
        }

        public byte[] Read()
        {
            byte[] buf_size = new byte[4];
            ReadInternal(buf_size, 4);
            int size = BitConverter.ToInt32(buf_size, 0);
            byte[] buff = new byte[size];
            ReadInternal(buff, size);
            return buff;
        }

        public void Write<T>(T value)
            where T : IStorable
        {
            DataStorage typeInstance = new DataStorage();
            value.WriteToData(typeInstance);
            int size = typeInstance.Size;
            WriteInternal(BitConverter.GetBytes(size));
            WriteInternal(typeInstance.Data);
        }

        public void Write(byte[] bytes)
        {
            WriteInternal(BitConverter.GetBytes(bytes.Length));
            WriteInternal(bytes);
        }

        public void Write(int value)
        {
            WriteInternal(new byte[4] { 4, 0, 0, 0 });
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void Write(uint value)
        {
            WriteInternal(new byte[4] { 4, 0, 0, 0 });
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void Write(long value)
        {
            WriteInternal(new byte[4] { 8, 0, 0, 0 });
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void Write(char value)
        {
            WriteInternal(new byte[4] { 2, 0, 0, 0 });
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void Write(bool value)
        {
            WriteInternal(new byte[4] { 1, 0, 0, 0 });
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void Write(float value)
        {
            WriteInternal(new byte[4] { 4, 0, 0, 0 });
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void Write(double value)
        {
            WriteInternal(new byte[4] { 8, 0, 0, 0 });
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void Write(string value)
        {
            if (value == null)
                value = "";
            byte[] data = Encoding.UTF8.GetBytes(value);
            WriteInternal(BitConverter.GetBytes(data.Length));
            WriteInternal(data);
        }

        public void WriteUncheck(int value)
        {
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void WriteUncheck(uint value)
        {
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void WriteUncheck(long value)
        {
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void WriteUncheck(char value)
        {
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void WriteUncheck(bool value)
        {
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void WriteUncheck(float value)
        {
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void WriteUncheck(double value)
        {
            WriteInternal(BitConverter.GetBytes(value));
        }

        public void WriteAuto(object value)
        {
            if (value is IStorable)
                Write((IStorable)value);
            else if (value is int)
                Write((int)value);
            else if (value is uint)
                Write((uint)value);
            else if (value is long)
                Write((long)value);
            else if (value is char)
                Write((char)value);
            else if (value is bool)
                Write((bool)value);
            else if (value is float)
                Write((float)value);
            else if (value is string)
                Write((string)value);
            else throw new DataStorageAutoException(DataStorageAutoException.Operation.WriteAuto, value);
        }

        public T Read<T>() where T : IStorable, new()
        {
            byte[] data = Read();
            DataStorage stor = new DataStorage(data);
            T t = new T();
            t.ReadFromData(stor);
            return t;
        }

        public void Read<T>(out T value) where T : IStorable, new()
        {
            value = Read<T>();
        }

        public void Read(out int value)
        {
            byte[] data = Read();
            if (data.Length != 4)
                throw new DataStorageReadException(4, data);
            value = BitConverter.ToInt32(data, 0);
        }

        public void Read(out uint value)
        {
            byte[] data = Read();
            if (data.Length != 4)
                throw new DataStorageReadException(4, data);
            value = BitConverter.ToUInt32(data, 0);
        }

        public void Read(out long value)
        {
            byte[] data = Read();
            if (data.Length != 8)
                throw new DataStorageReadException(8, data);
            value = BitConverter.ToInt64(data, 0);
        }

        public void Read(out char value)
        {
            byte[] data = Read();
            if (data.Length != 2)
                throw new DataStorageReadException(2, data);
            value = BitConverter.ToChar(data, 0);
        }

        public void Read(out bool value)
        {
            byte[] data = Read();
            if (data.Length != 1)
                throw new DataStorageReadException(1, data);
            value = BitConverter.ToBoolean(data, 0);
        }

        public void Read(out float value)
        {
            byte[] data = Read();
            if (data.Length != 4)
                throw new DataStorageReadException(4, data);
            value = BitConverter.ToSingle(data, 0);
        }

        public void Read(out double value)
        {
            byte[] data = Read();
            if (data.Length != 8)
                throw new DataStorageReadException(8, data);
            value = BitConverter.ToDouble(data, 0);
        }

        public void Read(out string value)
        {
            byte[] data = Read();
            value = Encoding.UTF8.GetString(data);
        }

        public void ReadUncheck(out int value)
        {
            byte[] data = new byte[4];
            ReadInternal(data, 4);
            value = BitConverter.ToInt32(data, 0);
        }

        public void ReadUncheck(out uint value)
        {
            byte[] data = new byte[4];
            ReadInternal(data, 4);
            value = BitConverter.ToUInt32(data, 0);
        }

        public void ReadUncheck(out long value)
        {
            byte[] data = new byte[8];
            ReadInternal(data, 8);
            value = BitConverter.ToInt64(data, 0);
        }

        public void ReadUncheck(out char value)
        {
            byte[] data = new byte[2];
            ReadInternal(data, 2);
            value = BitConverter.ToChar(data, 0);
        }

        public void ReadUncheck(out bool value)
        {
            byte[] data = new byte[1];
            ReadInternal(data, 1);
            value = BitConverter.ToBoolean(data, 0);
        }

        public void ReadUncheck(out float value)
        {
            byte[] data = new byte[4];
            ReadInternal(data, 4);
            value = BitConverter.ToSingle(data, 0);
        }

        public void ReadUncheck(out double value)
        {
            byte[] data = new byte[8];
            ReadInternal(data, 8);
            value = BitConverter.ToDouble(data, 0);
        }
    }
}
