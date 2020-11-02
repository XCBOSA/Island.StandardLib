using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Island.StandardLib.Storage.Encryption
{
    public abstract class EncrypterBase
    {
        public string DefaultKey;
        public EncrypterBase(string defaultKey) => DefaultKey = defaultKey;
        public byte[] Encrypt(byte[] plainData) => Encrypt(plainData, DefaultKey);
        public byte[] Decrypt(byte[] encryptedData) => Decrypt(encryptedData, DefaultKey);
        public abstract byte[] Encrypt(byte[] plainData, string key);
        public abstract byte[] Decrypt(byte[] encryptedData, string key);
    }

    public class RijndaelEncrypter : EncrypterBase
    {
        public RijndaelEncrypter(string defaultKey) : base(defaultKey) { }

        public override byte[] Decrypt(byte[] encryptedData, string key)
        {
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            MemoryStream mStream = new MemoryStream(encryptedData);
            RijndaelManaged aes = new RijndaelManaged();
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = bKey;
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            try
            {
                byte[] tmp = new byte[encryptedData.Length + 32];
                int len = cryptoStream.Read(tmp, 0, encryptedData.Length + 32);
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

        public override byte[] Encrypt(byte[] plainData, string key)
        {
            MemoryStream mStream = new MemoryStream();
            RijndaelManaged aes = new RijndaelManaged();
            byte[] bKey = new byte[32];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.KeySize = 128;
            aes.Key = bKey;
            CryptoStream cryptoStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            try
            {
                cryptoStream.Write(plainData, 0, plainData.Length);
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
    }
}
