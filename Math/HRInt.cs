using Island.StandardLib.Storage;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Island.StandardLib.Math
{
    /// <summary>
    /// 表示一个小数点后一位的0-998的小数，它将可以转换为通用的两字节Char
    /// </summary>
    public struct HRInt : IStorable
    {
        char hrValue;

        /// <summary>
        /// 使用数据交换应用的两字节Char初始化
        /// </summary>
        /// <param name="HR"></param>
        public HRInt(char HR)
        {
            hrValue = HR;
        }

        /// <summary>
        /// 使用浮点数初始化
        /// </summary>
        /// <param name="source"></param>
        public HRInt(float source, bool requestStrictMode = true)
        {
            int i = (int)(source * 10);
            if (i > 9998 || i < 0)
            {
                if (requestStrictMode)
                    throw new Exception("Island.Server.Math.HRInt: 无效的转换: " + source);
                else
                {
                    if (i < 0) i = 0;
                    if (i > 9998) i = 9998;
                }
            }
            i++;
            string ci = i.ToString();
            if (i < 1000)
            {
                ci = "0" + ci;
                if (i < 100)
                {
                    ci = "0" + ci;
                    if (i < 10)
                        ci = "0" + ci;
                }
            }
            hrValue = UnicodeToString(ci)[0];
        }

        /// <summary>
        /// 获取浮点数表示
        /// </summary>
        public float Value
        {
            get
            {
                return (int.Parse(StringToUnicode(hrValue + "")) - 1) / 10f;
            }
        }

        /// <summary>
        /// 获取数据交换应用的两字节Char表示
        /// </summary>
        public char HRValue
        {
            get
            {
                return hrValue;
            }
        }

        static string StringToUnicode(string source)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(source);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
                stringBuilder.AppendFormat("{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            return stringBuilder.ToString();
        }

        static string UnicodeToString(string source)
        {
            return new Regex(@"([0-9A-F]{4})", RegexOptions.IgnoreCase).Replace(
                source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out hrValue);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(hrValue);
        }
    }
}
