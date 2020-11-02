using Island.StandardLib.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Island.StandardLib
{
    //public class SocketEx : Socket
    //{
    //    MultiSizeData SendPool, RecvPool;
    //    Thread SendLoopThread, RecvLoopThread;

    //    public SocketEx(SocketInformation info) : base(info)
    //    {
    //        SendPool = new MultiSizeData();
    //        RecvPool = new MultiSizeData();
    //    }

    //    public SocketEx(AddressFamily address, SocketType stype, ProtocolType ptype) : base(address, stype, ptype)
    //    {
    //        SendPool = new MultiSizeData();
    //        RecvPool = new MultiSizeData();
    //    }

    //    public void StartLoop()
    //    {
    //        SendLoopThread = new Thread(SendLoop);
    //        SendLoopThread.IsBackground = true;
    //        SendLoopThread.Start();
    //        RecvLoopThread = new Thread(RecvLoop);
    //        RecvLoopThread.IsBackground = true;
    //        RecvLoopThread.Start();
    //    }

    //    void SendLoop()
    //    {
    //        byte[] fbuff = new byte[128];
    //        while (true)
    //        {
    //            try
    //            {
    //                if (SendPool.Size > 127)
    //                {
    //                    SendPool.Receive(fbuff, 0, 128);
    //                    SendPool.FreeBegin();
    //                    Send(fbuff);
    //                }
    //                SendPool.FreeBegin();
    //            }
    //            catch { break; }
    //            Thread.Sleep(2);
    //        }
    //    }

    //    void RecvLoop()
    //    {
    //        byte[] fbuff = new byte[128];
    //        while (true)
    //        {
    //            try
    //            {
    //                Receive(fbuff, fbuff.Length, SocketFlags.None);
    //                SendPool.Send(fbuff);
    //            }
    //            catch { break; }
    //            Thread.Sleep(2);
    //        }
    //    }

    //    public void ReceiveEx(byte[] buffer, int size)
    //    {
    //        RecvPool.Receive(buffer, 0, size);
    //        RecvPool.FreeBegin();
    //    }

    //    public void SendEx(byte[] buffer)
    //    {
    //        SendPool.Send(buffer);
    //    }

    //    protected override void Dispose(bool disposing)
    //    {
    //        base.Dispose(disposing);
    //        SendLoopThread.Stop();
    //        RecvLoopThread.Stop();
    //    }
    //}

    //public class SocketExS
    //{
    //    public const int NativePoolSize = 4;

    //    MultiSizeData SendPool, RecvPool;
    //    Thread SendLoopThread, RecvLoopThread;
    //    public Socket Socket;

    //    public SocketExS(Socket socket)
    //    {
    //        SendPool = new MultiSizeData();
    //        RecvPool = new MultiSizeData();
    //        Socket = socket;
    //        StartLoop();
    //    }

    //    public void StartLoop()
    //    {
    //        SendLoopThread = new Thread(SendLoop);
    //        SendLoopThread.IsBackground = true;
    //        SendLoopThread.Start();
    //        RecvLoopThread = new Thread(RecvLoop);
    //        RecvLoopThread.IsBackground = true;
    //        RecvLoopThread.Start();
    //    }

    //    void SendLoop()
    //    {
    //        byte[] fbuff = new byte[NativePoolSize];
    //        while (true)
    //        {
    //            try
    //            {
    //                if (SendPool.Size >= NativePoolSize)
    //                {
    //                    SendPool.Read(fbuff, 0, NativePoolSize);
    //                    Socket.Send(fbuff);
    //                }
    //            }
    //            catch (Exception e)
    //            {
    //                Debug.WriteLine(e);
    //                break;
    //            }
    //            Thread.Sleep(2);
    //        }
    //    }

    //    void RecvLoop()
    //    {
    //        byte[] fbuff = new byte[NativePoolSize];
    //        while (true)
    //        {
    //            try
    //            {
    //                Socket.Receive(fbuff, fbuff.Length, SocketFlags.None);
    //                RecvPool.Write(fbuff);
    //            }
    //            catch { break; }
    //            Thread.Sleep(2);
    //        }
    //    }

    //    public void ReceiveEx(byte[] buffer, int size)
    //    {
    //        RecvPool.Read(buffer, 0, size);
    //        RecvPool.FreeUnused();
    //    }

    //    public void SendEx(byte[] buffer)
    //    {
    //        SendPool.Write(buffer);
    //    }

    //    public void Close()
    //    {
    //        Socket.Close();
    //    }

    //    public void Dispose()
    //    {
    //        SendLoopThread.Stop();
    //        RecvLoopThread.Stop();
    //        Socket.Dispose();
    //    }
    //}
}
