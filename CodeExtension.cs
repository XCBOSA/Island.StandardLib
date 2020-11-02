using Island.StandardLib.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

namespace Island.StandardLib
{
    public static class CodeExtension
    {
        /// <summary>
        /// 获取这个byte[]的16字节特征值
        /// </summary>
        public static byte[] Hash16(this byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            return md5.ComputeHash(bytes);
        }

        public static void Clear(this byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = 0;
        }

        public static bool ByteEquals(this byte[] data, byte[] bytes)
        {
            if (data.Length != bytes.Length) return false;
            for (int i = 0; i < data.Length; i++)
                if (data[i] != bytes[i]) return false;
            return true;
        }

        public static string ToStringEx(this long data)
        {
            string ret = "";
            if (data >= 100000000) ret = System.Math.Round(data / 100000000d, 2) + "亿";
            else if (data >= 10000) ret = System.Math.Round(data / 10000d, 2) + "万";
            else ret = data.ToString();
            return ret;
        }

        public static bool Contain<T>(this T[] tlist, T finding)
        {
            for (int i = 0; i < tlist.Length; i++)
                if (tlist[i].Equals(finding))
                    return true;
            return false;
        }

        public static string ToStringEx(this float data) => data.ToString("P");

        public static MultData ToMultData(this byte[] data) => new MultData(data);
        public static MultData ToMultData(this int data) => new MultData(data);
        public static MultData ToMultData(this char data) => new MultData(data);
        public static MultData ToMultData(this bool data) => new MultData(data);
        public static MultData ToMultData(this float data) => new MultData(data);
        public static MultData ToMultData(this string data) => new MultData(data);
        public static MultData ToMultData<T>(this T data) where T : IStorable => new MultData(data);

        public static ConnectCommand CommandWithArgs(this int command, params object[] args)
        {
            MultData[] datas = new MultData[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is MultData)
                    datas[i] = (MultData)args[i];
                if (args[i] is byte[])
                    datas[i] = new MultData((byte[])args[i]);
                else if (args[i] is int)
                    datas[i] = new MultData((int)args[i]);
                else if (args[i] is char)
                    datas[i] = new MultData((char)args[i]);
                else if (args[i] is bool)
                    datas[i] = new MultData((bool)args[i]);
                else if (args[i] is float)
                    datas[i] = new MultData((float)args[i]);
                else if (args[i] is string)
                    datas[i] = new MultData((string)args[i]);
                else if (args[i] is IStorable)
                    datas[i] = new MultData((IStorable)args[i]);
                else throw new InvalidCastException();
            }
            return new ConnectCommand(command, datas);
        }

        public static void Stop(this Thread thread)
        {
            try
            {
                thread?.Abort();
            }
            catch { }
        }

        public static void Do<CollectionType>(this CollectionType[] collection, Action<CollectionType> func)
        {
            for (int i = 0; i < collection.Length; i++)
                func(collection[i]);
        }

        public static void Do<CollectionType>(this CollectionType[] collection, Action<CollectionType, int> func)
        {
            for (int i = 0; i < collection.Length; i++)
                func(collection[i], i);
        }

        public static RetType[] Do<CollectionType, RetType>(this CollectionType[] collection, Func<CollectionType, RetType> func)
        {
            RetType[] rets = new RetType[collection.Length];
            for (int i = 0; i < collection.Length; i++)
                rets[i] = func(collection[i]);
            return rets;
        }

        public static T[] Sub<T>(this T[] array, int begin, int end)
        {
            T[] newT = new T[end - begin];
            for (int i = begin; i < end; i++)
                newT[i - begin] = array[i];
            return newT;
        }

        public static T[] Add<T>(this T[] array, T[] arr)
        {
            T[] newT = new T[array.Length + arr.Length];
            for (int i = 0; i < array.Length; i++)
                newT[i] = array[i];
            for (int i = 0; i < arr.Length; i++)
                newT[i + array.Length] = arr[i];
            return newT;
        }

        public static void Set<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey name, TValue val)
        {
            if (dict.ContainsKey(name)) dict[name] = val;
            else dict.Add(name, val);
        }

        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey name, TValue defval = default)
        {
            if (dict.ContainsKey(name)) return dict[name];
            return defval;
        }

        public static string PushRandom<TValue>(this Dictionary<string, TValue> dict, TValue objToPush)
        {
            Random rd = new Random();
            string val = "";
            while (true)
            {
                val = (char)rd.Next(char.MinValue, char.MaxValue) + "" + (char)rd.Next(char.MinValue, char.MaxValue);
                if (!dict.ContainsKey(val))
                    break;
            }
            dict[val] = objToPush;
            return val;
        }
    }
}
