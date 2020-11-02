using System;
using System.Collections;
using System.Collections.Generic;

namespace Island.StandardLib.Storage
{
    [Serializable]
    public class StorableDictionary<TKey, TValue> : IStorable, IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IStorable, new() where TValue : IStorable, new()
    {
        Dictionary<TKey, TValue> baseDict;

        public StorableDictionary() => baseDict = new Dictionary<TKey, TValue>();
        public TValue this[TKey key] => baseDict[key];
        public void Add(TKey key, TValue val) => baseDict.Add(key, val);
        public void Remove(TKey key) => baseDict.Remove(key);
        public int Count => baseDict.Count;
        public bool ContainsKey(TKey key) => baseDict.ContainsKey(key);
        public bool ContainsValue(TValue val) => baseDict.ContainsValue(val);
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => baseDict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => baseDict.GetEnumerator();

        public void ReadFromData(DataStorage data)
        {
            data.Read(out int len);
            for (int i = 0; i < len; i++)
            {
                TKey key = data.Read<TKey>();
                data.Read(out bool hasVal);
                TValue val = default;
                if (hasVal) val = data.Read<TValue>();
                baseDict.Add(key, val);
            }
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(baseDict.Count);
            foreach (var tpair in baseDict)
            {
                data.Write(tpair.Key);
                if (tpair.Value == null) data.Write(false);
                else
                {
                    data.Write(true);
                    data.Write(tpair.Value);
                }
            }
        }
    }
}
