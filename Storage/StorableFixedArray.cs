using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Island.StandardLib.Storage
{
    /// <summary>
    /// 表示一个每项长度固定的可序列化变长数组（固定项长可变数组）
    /// </summary>
    /// <typeparam name="T">定长项的类型</typeparam>
    [Serializable]
    public class StorableFixedArray<T> : IStorable, IEnumerable<T> where T : IStorable, new()
    {
        List<T> array;

        /// <summary>
        /// 初始化固定项长可变数组
        /// </summary>
        public StorableFixedArray()
        {
            array = new List<T>();
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out int size);
            for (int i = 0; i < size; i++)
                array.Add(data.Read<T>());
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(array.Count);
            for (int i = 0; i < array.Count; i++)
                data.Write(array[i]);
        }

        public void Add(T item) => array.Add(item);
        public void Remove(T item) => array.Remove(item);
        public void RemoveAt(int index) => array.RemoveAt(index);
        public void Clear() => array.Clear();
        public IEnumerator<T> GetEnumerator() => array.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => array.GetEnumerator();
        public int Count => array.Count;
        public int Size => array.Count;
        public int Length => array.Count;
        public T this[int index] => array[index];
    }
}
