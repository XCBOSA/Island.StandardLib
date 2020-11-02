using Island.StandardLib.Storage;
using System;

namespace Island.StandardLib.Math
{
    /// <summary>
    /// 表示一个总和为1的平衡容器
    /// </summary>
    public class Percentage : IStorable
    {
        StorableFixedArray<Key> percentages;

        public int KeyLength => percentages.Length;

        public Percentage() { }

        /// <summary>
        /// 初始化容器
        /// </summary>
        /// <param name="sourceValue">初始数据</param>
        public Percentage(params float[] sourceValue)
        {
            percentages = new StorableFixedArray<Key>();
            if (sourceValue.Length == 0) throw new Exception();
            float val = 0;
            for (int i = 0; i < sourceValue.Length; i++)
            {
                val += sourceValue[i];
                percentages.Add(new Key(sourceValue[i]));
            }
            if (val != 1f)
                AdjustBalance();
        }

        /// <summary>
        /// 获取和设置数据。其中，设置数据后会自动按比例平衡容器，保证容器总和为1
        /// </summary>
        /// <param name="index">数据序号</param>
        /// <returns></returns>
        public float this[int index]
        {
            get => percentages[index].Value;
            set => AdjustKeep(index, value);
        }

        public long Total(int index, long totalValue)
        {
            return (long)(this[index] * totalValue);
        }

        void AdjustBalance()
        {
            float total = 0f;
            for (int i = 0; i < KeyLength; i++)
                total += this[i];
            float px = 1 / total;
            for (int i = 0; i < KeyLength; i++)
                percentages[i].Value *= px;
        }

        void AdjustKeep(int keepIndex, float newValue)
        {
            AdjustBalance();
            float distance = this[keepIndex] - newValue;
            percentages[keepIndex].Value = newValue;
            float[] gains = new float[KeyLength - 1];
            int g = 0;
            for (int i = 0; i < KeyLength; i++)
            {
                if (keepIndex == i)
                    continue;
                gains[g] = this[i]; g++;
            }
            Percentage p = new Percentage(gains);
            g = 0;
            for (int i = 0; i < KeyLength; i++)
            {
                if (keepIndex == i)
                    continue;
                percentages[i].Value += p[g] * distance; g++;
            }
        }

        public override string ToString()
        {
            string s = "Percentage [";
            float total = 0f;
            for (int i = 0; i < KeyLength; i++)
            {
                total += this[i];
                if (i == KeyLength - 1)
                    s += this[i].ToString("P");
                else s += this[i].ToString("P") + ", ";
            }
            return s + "]";
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out percentages);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(percentages);
        }

        class Key : IStorable
        {
            public float Value;
            public Key() => Value = 0f;
            public Key(float sourceValue) => Value = sourceValue;
            public void ReadFromData(DataStorage data) => data.Read(out Value);
            public void WriteToData(DataStorage data) => data.Write(Value);
        }
    }
}
