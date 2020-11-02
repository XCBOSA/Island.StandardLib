using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Island.StandardLib.Storage.Encryption
{
    public class EncryptedData<DataType> : IStorable where DataType : IStorable, new()
    {
        public byte[] Encrypted;

        public EncryptedData(EncrypterBase encrypter, DataType data, string key) => SetData(encrypter, data, key);
        public EncryptedData() => Encrypted = new byte[0];

        public DataType GetData(EncrypterBase encrypter, string key)
        {
            byte[] plain = encrypter.Decrypt(Encrypted, key);
            DataStorage ds = new DataStorage(plain);
            return ds.Read<DataType>();
        }

        public void SetData(EncrypterBase encrypter, DataType data, string key)
        {
            byte[] plain = data.GetBytes();
            Encrypted = encrypter.Encrypt(plain, key);
        }

        public void ReadFromData(DataStorage data)
        {
            data.ReadUncheck(out int size);
            data.ReadInternal(Encrypted, size);
        }

        public void WriteToData(DataStorage data)
        {
            data.WriteUncheck(Encrypted.Length);
            data.WriteInternal(Encrypted);
        }
    }
}
