using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Island.StandardLib.Storage
{
    public static class DataStorageManager
    {
        static readonly byte[] METAINF = new byte[64] { 33, 88, 67, 78, 66, 33, 32, 80, 104, 121, 83, 105, 109, 32, 83, 116, 111, 114, 97, 103, 101, 32, 70, 105, 108, 101, 32, 68, 79, 32, 78, 79, 84, 32, 69, 68, 73, 84, 32, 84, 72, 73, 83, 32, 70, 73, 76, 69, 32, 85, 83, 69, 32, 84, 69, 88, 84, 32, 69, 68, 73, 84, 79, 82 };

        /// <summary>
        /// 按照指定可序列化类型序列化此内存
        /// </summary>
        /// <typeparam name="T">可序列化类型</typeparam>
        public static T ReadData<T>(this byte[] bytes) where T : IStorable, new()
        {
            T instance = new T();
            DataStorage ds = new DataStorage(bytes);
            instance.ReadFromData(ds);
            return instance;
        }

        public static byte[] GetBytes(this IStorable data)
        {
            DataStorage ds = new DataStorage();
            data.WriteToData(ds);
            return ds.Bytes;
        }

        /// <summary>
        /// 将可序列化类型序列化并存入文件
        /// </summary>
        /// <param name="data">可序列化类型</param>
        /// <param name="writeTo">文件路径</param>
        public static void WriteFile(IStorable data, string writeTo)
        {
            DataStorage ds = new DataStorage();
            data.WriteToData(ds);
            if (File.Exists(writeTo)) File.Delete(writeTo);
            FileStream writer = new FileStream(writeTo, FileMode.Create, FileAccess.Write);
            writer.Write(ds.Bytes, 0, ds.Size);
            writer.Flush();
            writer.Close();
        }

        public static void WriteFileWithMd(IStorable data, IStorable metadata, string writeTo)
        {
            byte[] buff_md = metadata.GetBytes(), buff_dat = data.GetBytes();
            int pmetadata = METAINF.Length, pbody = pmetadata + buff_md.Length;
            FileStream writer = new FileStream(writeTo, FileMode.Create, FileAccess.Write);
            writer.Write(BitConverter.GetBytes(pmetadata + 8), 0, 4);
            writer.Write(BitConverter.GetBytes(pbody + 8), 0, 4);
            writer.Write(METAINF, 0, METAINF.Length);
            writer.Flush();
            writer.Write(buff_md, 0, buff_md.Length);
            writer.Flush();
            writer.Write(buff_dat, 0, buff_dat.Length);
            writer.Flush();
            writer.Close();
        }

        public static string WriteToString(IStorable data) => Convert.ToBase64String(data.GetBytes());
        public static T ReadFromString<T>(string str) where T : IStorable, new() => Convert.FromBase64String(str).ReadData<T>();
        public static string WriteToString(IStorable data, string keypass32) => Convert.ToBase64String(AESEncrypt(data.GetBytes(), keypass32));
        public static T ReadFromString<T>(string str, string keypass32) where T : IStorable, new() => AESDecrypt(Convert.FromBase64String(str), keypass32).ReadData<T>();

        static byte[] AESEncrypt(byte[] plainBytes, string Key)
        {
            MemoryStream mStream = new MemoryStream();
            RijndaelManaged aes = new RijndaelManaged();
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = bKey;
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            try
            {
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();
                return mStream.ToArray();
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
        }

        static byte[] AESDecrypt(byte[] encryptedBytes, string Key)
        {
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            MemoryStream mStream = new MemoryStream(encryptedBytes);
            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = bKey;
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            try
            {
                byte[] tmp = new byte[encryptedBytes.Length + 32];
                int len = cryptoStream.Read(tmp, 0, encryptedBytes.Length + 32);
                byte[] ret = new byte[len];
                Array.Copy(tmp, 0, ret, 0, len);
                return ret;
            }
            finally
            {
                cryptoStream.Close();
                mStream.Close();
                aes.Clear();
            }
        }

        public static MdType ReadFileWithMd_Md<MdType>(string readFrom) where MdType : IStorable, new()
        {
            FileStream reader = new FileStream(readFrom, FileMode.Open, FileAccess.Read);
            byte[] buff_md_p = new byte[4], buff_dat_p = new byte[4];
            reader.Read(buff_md_p, 0, 4);
            reader.Read(buff_dat_p, 0, 4);
            int md_p = BitConverter.ToInt32(buff_md_p, 0), dat_p = BitConverter.ToInt32(buff_dat_p, 0);
            byte[] buff_md = new byte[dat_p - md_p];
            reader.Position = md_p;
            reader.Read(buff_md, 0, buff_md.Length);
            reader.Close();
            return buff_md.ReadData<MdType>();
        }

        public static DataType ReadFileWithMd_Data<DataType>(string readFrom) where DataType : IStorable, new()
        {
            FileStream reader = new FileStream(readFrom, FileMode.Open, FileAccess.Read);
            byte[] buff_dat_p = new byte[4];
            reader.Position = 4;
            reader.Read(buff_dat_p, 0, 4);
            int dat_p = BitConverter.ToInt32(buff_dat_p, 0);
            reader.Position = dat_p;
            byte[] buff_data = new byte[reader.Length - dat_p];
            reader.Read(buff_data, 0, buff_data.Length);
            reader.Close();
            return buff_data.ReadData<DataType>();
        }

        /// <summary>
        /// 将此可序列化类型序列化并存入文件
        /// </summary>
        /// <param name="writeTo">文件路径</param>
        public static void WriteToFile(this IStorable data, string writeTo)
        {
            WriteFile(data, writeTo);
        }

        /// <summary>
        /// 从文件中读取并创建可序列化类型实例
        /// </summary>
        /// <param name="readFrom">文件路径</param>
        public static T ReadFile<T>(string readFrom) where T : IStorable, new()
        {
            if (!File.Exists(readFrom)) throw new FileNotFoundException();
            FileStream reader = new FileStream(readFrom, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[reader.Length];
            reader.Read(data, 0, data.Length);
            reader.Close();
            DataStorage ds = new DataStorage(data);
            T instance = new T();
            instance.ReadFromData(ds);
            return instance;
        }
    }
}
