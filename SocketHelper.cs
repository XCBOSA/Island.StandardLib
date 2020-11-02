using Island.StandardLib.Exceptions;
using Island.StandardLib.Storage;
using System;
using System.Net.Sockets;
using System.Threading;

namespace Island.StandardLib
{
    public static class SocketHelper
    {
        public static bool EnableHashCheck { get; set; } = true;
        public static int DefaultMaxBitSize { get; set; } = 1024 * 1024 * 1024;
        public static int MaxRecvBuffSize { get; set; } = 1024;

        public static int SendBufferSizeEx { get; set; } = 0;
        public static int RecvBufferSizeEx { get; set; } = 0;

        static int Claim(int val, int min, int max)
        {
            if (val < min) return min;
            if (val > max) return max;
            return val;
        }

        public static void ReceiveEx(this Socket sock, byte[] buff_out, int size)
        {
            int recved = 0;
            while (recved < size) recved += sock.Receive(buff_out, recved, size - recved > MaxRecvBuffSize ? MaxRecvBuffSize : size - recved, SocketFlags.None);
        }

        public static void SendEx(this Socket sock, byte[] buff_in)
        {
            int send = 0;
            while (send < buff_in.Length) send += sock.Send(buff_in, send, buff_in.Length - send > MaxRecvBuffSize ? MaxRecvBuffSize : buff_in.Length - send, SocketFlags.None);
        }

        public static T ReceiveOnce<T>(this Socket socket, int maxBitSize, string playerName = null)
           where T : IStorable, new()
        {
            maxBitSize = maxBitSize == 0 ? DefaultMaxBitSize : maxBitSize;
            byte[] buf_len = new byte[4], buf_hash = new byte[16], buf_data;
            socket.ReceiveEx(buf_len, 4);
            int length = BitConverter.ToInt32(buf_len, 0);
            if (length > maxBitSize)
                throw new PlayerSocketFatalException((playerName ?? "") + "(" + length + ", " + maxBitSize + ")", PlayerSocketFatalExceptionType.RecvBufferTooLong);
            socket.ReceiveEx(buf_hash, 16);
            buf_data = new byte[length];
            socket.ReceiveEx(buf_data, length);
            if (EnableHashCheck)
            {
                if (!buf_data.Hash16().ByteEquals(buf_hash))
                    throw new PlayerSocketFatalException(playerName ?? "", PlayerSocketFatalExceptionType.HashFailException);
            }
            return buf_data.ReadData<T>();
        }

        public static void SendOnce(this Socket socket, byte[] buf_data)
        {
            byte[] buf_len = BitConverter.GetBytes(buf_data.Length), buf_hash = buf_data.Hash16();
            socket.SendEx(buf_len);
            socket.SendEx(buf_hash);
            socket.SendEx(buf_data);
        }

        public static void SendOnce<T>(this Socket socket, T data)
            where T : IStorable, new()
        {
            byte[] buf_data = data.GetBytes(),
                buf_len = BitConverter.GetBytes(buf_data.Length),
                buf_hash = buf_data.Hash16();
            socket.SendEx(buf_len);
            socket.SendEx(buf_hash);
            socket.SendEx(buf_data);
        }
    }
}
