using Island.StandardLib.Exceptions;
using Island.StandardLib.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Island.StandardLib
{
    /// <summary>
    /// 可继承此类，封装服务器操作
    /// </summary>
    /// <typeparam name="TPlayer">为每个连接者提供一个唯一的TPlayer，此类需继承自<see cref="ConnectionPlayerBase"/>并提供无参数构造函数</typeparam>
    public abstract class ConnectionServer<TPlayer, LoginOrRegisterRequestType>
        where TPlayer : ConnectionPlayerBase, new()
        where LoginOrRegisterRequestType : LoginOrRegisterRequest, IStorable, new()
    {
        public int ServerPort { get; private set; }
        public string ServerAddress { get; private set; }
        public int MaxBitSize { get; private set; }
        public uint ServerVersion { get; private set; }

        public List<TPlayer> OnlinePlayers;

        public ConnectionServer(string addr, int port, uint version = 1, int maxbitsz = 524288)
        {
            Logger.InitLoggerOnce();

            ConsoleColor bkup = Console.ForegroundColor;
            Console.Write("Island.StandardLib.");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("ServerConnection");
            Console.ForegroundColor = bkup;
            Console.WriteLine("()");

            Console.WriteLine(" __  ______   _   _ ____  \n \\ \\/ / ___| | \\ | | __ ) \n  \\  / |     |  \\| |  _ \\ \n  /  \\ |___  | |\\  | |_) |\n /_/\\_\\____| |_| \\_|____/\n");
            Console.Write("-------------------- ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("StandardLib ");
            Console.ForegroundColor = bkup;
            Console.WriteLine("Version 20Y0823 --------------------\n");

            OnlinePlayers = new List<TPlayer>();
            ServerAddress = addr;
            MaxBitSize = maxbitsz;
            ServerPort = port;
            ServerVersion = version;
            IPAddress serverip = IPAddress.Parse(addr);
            Socket serverSocket = new Socket(serverip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(serverip, ServerPort));
            serverSocket.Listen(1024);
            Thread acceptThread = new Thread(AcceptLoop);
            acceptThread.IsBackground = true;
            acceptThread.Start(serverSocket);
            Logger.WriteLine(LogLevel.Default, $"Server start at {addr}:{ServerPort}");
        }

        void AcceptLoop(object obj)
        {
            Socket serverSocket = (Socket)obj;
            OnConnectionBegin();
            Exception exitWith;
            while (true)
            {
                try
                {
                    Socket client = serverSocket.Accept();
                    Logger.WriteLine(LogLevel.Info, "{0}:{1} Connected.", ((IPEndPoint)client.RemoteEndPoint).Address.ToString(),
                        ((IPEndPoint)client.RemoteEndPoint).Port.ToString());
                    Thread loginThread = new Thread(PassLogin);
                    loginThread.IsBackground = true;
                    loginThread.Start(client);
                }
                catch (Exception e)
                {
                    exitWith = e;
                    Logger.LogError(e);
                    break;
                }
            }
            OnConnectionBreaked(exitWith);
        }

        protected abstract LoginResult PassLogin(LoginOrRegisterRequestType request);
        protected abstract RegisterResult PassReg(LoginOrRegisterRequestType request);
        protected virtual void OnConnectionBegin() { }
        protected virtual void OnConnectionBreaked(Exception reason) { }

        void InternalDestroy(ConnectionPlayerBase player) => OnlinePlayers.Remove((TPlayer)player);

        void PassLogin(object sock)
        {
            Socket client = (Socket)sock;
            try
            {
                LoginOrRegisterRequestType request = client.ReceiveOnce<LoginOrRegisterRequestType>(512);
                bool cont = false;
                byte[] callbackd = null;
                if (request.IsLogin)
                {
                    LoginCallback callback = new LoginCallback();
                    if (request.ClientVersion != ServerVersion)
                        callback.Code = request.ClientVersion < ServerVersion ? LoginResult.VersionLower : LoginResult.VersionHigher;
                    else callback.Code = PassLogin(request);
                    if (callback.Code == LoginResult.Success)
                        cont = true;
                    callbackd = callback.GetBytes();
                }
                else
                {
                    RegisterCallback callback = new RegisterCallback();
                    if (request.ClientVersion != ServerVersion)
                        callback.Username = (int)(request.ClientVersion < ServerVersion ? LoginResult.VersionLower : LoginResult.VersionHigher);
                    else callback.Username = (int)PassReg(request);
                    callbackd = callback.GetBytes();
                }
                client.SendOnce(callbackd);
                if (cont)
                {
                    TPlayer isp = new TPlayer();
                    isp.Init(client, request.Username, MaxBitSize, InternalDestroy);
                    OnlinePlayers.Add(isp);
                    Logger.WriteLine(LogLevel.Info, "{0} Joined.", isp.Id);
                }
                else
                {
                    Logger.WriteLine(LogLevel.Info, "{0}:{1} Disconnected.", ((IPEndPoint)client.RemoteEndPoint).Address.ToString(),
                        ((IPEndPoint)client.RemoteEndPoint).Port.ToString());
                    client.Close();
                    client.Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, ((IPEndPoint)client.RemoteEndPoint).Address.ToString() + ":");
                Logger.LogError(e);
                client.Close();
                client.Dispose();
            }
        }
    }

    public abstract class ConnectionPlayerBase
    {
        int max_bitsize;

        public int Id { get; private set; }
        public bool IsOnline { get; private set; }

        public ConnectObjectFromServer CommandSendPool;
        public Thread SocketLoopThread;

        public ConnectionPlayerBase() { }

        Action<ConnectionPlayerBase> selfDestroy;

        internal void Init(Socket vaildPlayerSocket, int uid, int mbsz, Action<ConnectionPlayerBase> selfDest)
        {
            CommandSendPool = new ConnectObjectFromServer();
            Id = uid;
            SocketLoopThread = new Thread(SocketLoop);
            SocketLoopThread.IsBackground = true;
            SocketLoopThread.Start(vaildPlayerSocket);
            max_bitsize = mbsz;
            selfDestroy = selfDest;
        }

        Socket socket;

        public void SocketLoop(object objVaildPlayerSocket)
        {
            IsOnline = true;
            socket = (Socket)objVaildPlayerSocket;
            bool isFirstSend = true;
            OnConnectionBegin();
            Exception breakWith;
            while (true)
            {
                try
                {
                    ConnectObjectFromClient clientData = null;
                    if (isFirstSend)
                    {
                        clientData = new ConnectObjectFromClient();
                        isFirstSend = false;
                    }
                    else clientData = socket.ReceiveOnce<ConnectObjectFromClient>(max_bitsize, Id.ToString());
                    lock (CommandSendPool)
                    {
                        InnerCommandPass(clientData);
                        socket.SendOnce(CommandSendPool);
                        CommandSendPool.ClearCommands();
                    }
                }
                catch (PlayerSocketFatalException e) { breakWith = e; Logger.LogError(e); break; }
                catch (SocketException e) { breakWith = e; Logger.Log(LogLevel.Info, "Player {0}: Stopped Connection", Id); break; }
                catch (Exception e) { breakWith = e; Logger.Log(LogLevel.Warning, "Player {0}: Exception: {1}", Id, e); break; }
            }
            try
            {
                socket.Close();
                socket.Dispose();
            }
            catch { }
            OnConnectionBreaked(breakWith);
            selfDestroy(this);
        }

        void InnerCommandPass(ConnectObjectFromClient clientData)
        {
            for (int i = 0; i < clientData.Commands.Length; i++)
            {
                ConnectCommand command = clientData.Commands[i];
                PassCommand(command);
            }
        }

        public void ForceEndConnect()
        {
            socket.Close();
            socket.Dispose();
        }

        protected abstract void PassCommand(ConnectCommand command);
        protected virtual void OnConnectionBegin() { }
        protected virtual void OnConnectionBreaked(Exception reason) { }
    }
}
