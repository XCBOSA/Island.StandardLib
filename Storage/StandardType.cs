using System;
using System.Collections;
using System.Collections.Generic;

namespace Island.StandardLib.Storage
{
    /// <summary>
    /// 表示一个实现序列化的 32 位带符号整数
    /// </summary>
    [Serializable]
    public struct SInt : IStorable, IComparable<SInt>, IComparable<int>, IComparable, IEquatable<SInt>, IEquatable<int>
    {
        public int Value;
        public SInt(int val) => Value = val;
        public void ReadFromData(DataStorage data) => data.Read(out Value);
        public void WriteToData(DataStorage data) => data.Write(Value);
        public static implicit operator int(SInt val) => val.Value;
        public static implicit operator SInt(int Int) => new SInt(Int);
        public override int GetHashCode() => Value.GetHashCode();
        public int CompareTo(SInt other) => Value.CompareTo(other.Value);
        public int CompareTo(int other) => Value.CompareTo(other);
        public int CompareTo(object obj) => Value.CompareTo(obj);
        public bool Equals(SInt other) => Value.Equals(other.Value);
        public bool Equals(int other) => Value.Equals(other);
        public override string ToString() => Value.ToString();
        public override bool Equals(object obj) => obj is int ? (int)obj == Value : obj is SInt ? ((SInt)obj).Value == Value : false;
        public static bool operator ==(SInt a, SInt b) => a.Equals(b);
        public static bool operator !=(SInt a, SInt b) => !a.Equals(b);
        public static bool operator ==(SInt a, int b) => a.Equals(b);
        public static bool operator !=(SInt a, int b) => !a.Equals(b);
        public static bool operator ==(int a, SInt b) => b.Equals(a);
        public static bool operator !=(int a, SInt b) => !b.Equals(a);
    }

    /// <summary>
    /// 表示一个实现序列化的布尔（<see cref="true"/> 或 <see cref="false"/>）值
    /// </summary>
    [Serializable]
    public struct SBool : IStorable, IComparable, IComparable<bool>, IComparable<SBool>, IEquatable<bool>, IEquatable<SBool>
    {
        public bool Value;
        public SBool(bool val) => Value = val;
        public void ReadFromData(DataStorage data) => data.Read(out Value);
        public void WriteToData(DataStorage data) => data.Write(Value);
        public static implicit operator bool(SBool val) => val.Value;
        public static implicit operator SBool(bool boo) => new SBool(boo);
        public override int GetHashCode() => Value.GetHashCode();
        public int CompareTo(object obj) => Value.CompareTo(obj);
        public int CompareTo(bool other) => Value.CompareTo(other);
        public int CompareTo(SBool other) => Value.CompareTo(other.Value);
        public bool Equals(bool other) => Value.Equals(other);
        public bool Equals(SBool other) => Value.Equals(other.Value);
        public override string ToString() => Value.ToString();
        public override bool Equals(object obj) => obj is bool ? (bool)obj == Value : obj is SBool ? ((SBool)obj).Value == Value : false;
        public static bool operator ==(SBool a, SBool b) => a.Equals(b);
        public static bool operator !=(SBool a, SBool b) => !a.Equals(b);
        public static bool operator ==(SBool a, bool b) => a.Equals(b);
        public static bool operator !=(SBool a, bool b) => !a.Equals(b);
        public static bool operator ==(bool a, SBool b) => b.Equals(a);
        public static bool operator !=(bool a, SBool b) => !b.Equals(a);
    }

    /// <summary>
    /// 表示一个实现序列化的 UTF-16 文本单元序列
    /// </summary>
    [Serializable]
    public class SString : IStorable, IComparable, ICloneable, IComparable<string>, IComparable<SString>, IEnumerable<char>, IEnumerable, IEquatable<string>, IEquatable<SString>
    {
        public string Value;
        public SString() { Value = ""; }
        public SString(string val) => Value = val;
        public void ReadFromData(DataStorage data) => data.Read(out Value);
        public void WriteToData(DataStorage data) => data.Write(Value);
        public static implicit operator string(SString val) => val.Value;
        public static implicit operator SString(string str) => new SString(str);
        public override int GetHashCode() => Value.GetHashCode();
        public int CompareTo(object obj) => Value.CompareTo(obj);
        public object Clone() => Value.Clone();
        public int CompareTo(string other) => Value.CompareTo(other);
        public int CompareTo(SString other) => Value.CompareTo(other.Value);
        public IEnumerator<char> GetEnumerator() => Value.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Value.GetEnumerator();
        public bool Equals(string other) => Value.Equals(other);
        public bool Equals(SString other) => Value.Equals(other.Value);
        public char this[int index] => Value[index];
        public override string ToString() => Value.ToString();
        public override bool Equals(object obj) => obj is string ? (string)obj == Value : obj is SString ? ((SString)obj).Value == Value : false;
        public static bool operator ==(SString a, SString b) => a.Equals(b);
        public static bool operator !=(SString a, SString b) => !a.Equals(b);
        public static bool operator ==(SString a, string b) => a.Equals(b);
        public static bool operator !=(SString a, string b) => !a.Equals(b);
        public static bool operator ==(string a, SString b) => b.Equals(a);
        public static bool operator !=(string a, SString b) => !b.Equals(a);
        public int Length => Value.Length;
    }
}
