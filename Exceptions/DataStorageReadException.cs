using System;
using System.Collections.Generic;
using System.Text;

namespace Island.StandardLib.Exceptions
{
    public class DataStorageReadException : Exception
    {
        public byte[] ErrorBits { get; private set; }
        public int ReadSize { get; private set; }

        public DataStorageReadException(int readSize, byte[] errorBits)
        {
            ReadSize = readSize;
            ErrorBits = errorBits;
        }

        public override string Message => $"DataStorageReadException: ReadAs: {ReadSize}Bits, SourceBits: {BitConverter.ToString(ErrorBits)}";
        public override string ToString() => Message;
    }

    public class DataStorageAutoException : Exception
    {
        public object ErrorObject { get; private set; }
        public Operation Operator;

        public enum Operation
        {
            ReadAuto, WriteAuto
        }

        public DataStorageAutoException(Operation opr, object obj)
        {
            ErrorObject = obj;
            Operator = opr;
        }

        public override string Message => $"DataStorageAutoException: {Operator}(object value) expect IStorable or StandardType, but got: {ErrorObject}";
        public override string ToString() => Message;
    }
}
