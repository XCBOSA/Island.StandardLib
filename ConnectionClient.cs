using Island.StandardLib.Storage;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Island.StandardLib
{
    public abstract class ConnectionClient
    {
        public string ServerAddress { get; private set; }
        public int ServerPort { get; private set; }
        public int MaxBitSize { get; private set; }
        public bool DebugMode { get; set; }
        public uint ClientVersion { get; private set; }

        public ConnectionClient(string addr, int port, uint version = 1, bool debug = false, int maxbitsz = 524288)
        {
            ServerAddress = addr;
            ServerPort = port;
            ClientVersion = version;
            MaxBitSize = maxbitsz;
            DebugMode = debug;
        }

        public ConnectObjectFromClient CommandSendPool;
        public Socket ClientSocket;

        public bool IsConnected { get; private set; }

        public LoginResult ConnectAsLogin<RequestType>(RequestType request)
            where RequestType : LoginOrRegisterRequest, IStorable, new()
        {
            try
            {
                request.ClientVersion = ClientVersion;
                request.IsLogin = true;
                IPAddress[] addr = Dns.GetHostAddresses(ServerAddress);
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ClientSocket.Connect(addr, ServerPort);
                ClientSocket.SendOnce(request);
                LoginCallback lcb = ClientSocket.ReceiveOnce<LoginCallback>(512);
                if (lcb.Success)
                {
                    Thread clientLoop = new Thread(ClientLoop);
                    clientLoop.IsBackground = true;
                    clientLoop.Start();
                }
                else
                {
                    try
                    {
                        ClientSocket.Close();
                        ClientSocket.Dispose();
                    }
                    catch { }
                }
                return lcb.Code;
            }
            catch (Exception e)
            {
                try
                {
                    ClientSocket?.Close();
                    ClientSocket?.Dispose();
                }
                catch { }
                return LoginResult.ConnectionError;
            }
        }

        public RegisterData ConnectAsRegister<RequestType>(RequestType request)
            where RequestType : LoginOrRegisterRequest, IStorable, new()
        {
            try
            {
                request.IsRegister = true;
                IPAddress[] addr = Dns.GetHostAddresses(ServerAddress);
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(addr, ServerPort);
                sock.SendOnce(request);
                RegisterCallback rcb = sock.ReceiveOnce<RegisterCallback>(512);
                try
                {
                    sock.Close();
                    sock.Dispose();
                }
                catch { }
                if (rcb.Success) return new RegisterData(RegisterResult.Success, rcb.Username);
                else return new RegisterData((RegisterResult)rcb.Username, -1);
            }
            catch
            {
                return new RegisterData(RegisterResult.ConnectionError, -1);
            }
        }

        public LoginResult ConnectAsLogin(int userName, string password)
        {
            try
            {
                LoginOrRegisterRequest request = new LoginOrRegisterRequest(userName, password, ClientVersion);
                request.IsLogin = true;
                IPAddress[] addr = Dns.GetHostAddresses(ServerAddress);
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ClientSocket.Connect(addr, ServerPort);
                ClientSocket.SendOnce(request);
                LoginCallback lcb = ClientSocket.ReceiveOnce<LoginCallback>(512);
                if (lcb.Success)
                {
                    Thread clientLoop = new Thread(ClientLoop);
                    clientLoop.IsBackground = true;
                    clientLoop.Start();
                }
                else
                {
                    try
                    {
                        ClientSocket.Close();
                        ClientSocket.Dispose();
                    }
                    catch { }
                }
                return lcb.Code;
            }
            catch (Exception e)
            {
                try
                {
                    ClientSocket?.Close();
                    ClientSocket?.Dispose();
                }
                catch { }
                return LoginResult.ConnectionError;
            }
        }

        public RegisterData ConnectAsRegister(string nickname, string password)
        {
            try
            {
                LoginOrRegisterRequest request = new LoginOrRegisterRequest(0, password, ClientVersion);
                request.IsRegister = true;
                request.Nickname = nickname;
                IPAddress[] addr = Dns.GetHostAddresses(ServerAddress);
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.Connect(addr, ServerPort);
                sock.SendOnce(request);
                RegisterCallback rcb = sock.ReceiveOnce<RegisterCallback>(512);
                try
                {
                    sock.Close();
                    sock.Dispose();
                }
                catch { }
                if (rcb.Success) return new RegisterData(RegisterResult.Success, rcb.Username);
                else return new RegisterData((RegisterResult)rcb.Username, -1);
            }
            catch
            {
                return new RegisterData(RegisterResult.ConnectionError, -1);
            }
        }

        void ClientLoop()
        {
            Exception endWith;
            CommandSendPool = new ConnectObjectFromClient();
            IsConnected = true;
            OnConnectionBegin();
            while (true)
            {
                try
                {
                    ConnectObjectFromServer serverData = ClientSocket.ReceiveOnce<ConnectObjectFromServer>(MaxBitSize);
                    bool recvCmds, sendCmds;
                    lock (CommandSendPool)
                    {
                        InnerCommandPass(serverData);
                        ClientSocket.SendOnce(CommandSendPool);
                        recvCmds = serverData.Commands.Count != 0;
                        sendCmds = CommandSendPool.Commands.Count != 0;
                        CommandSendPool.ClearCommands();
                    }
                    if (DebugMode)
                    {
                        if (recvCmds && sendCmds)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.Write("x");
                        }
                        else if (recvCmds)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                            Console.Write("-");
                        }
                        else if (sendCmds)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkGreen;
                            Console.Write("+");
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Write("=");
                        }
                    }
                }
                catch (Exception e)
                {
                    endWith = e;
                    break;
                }
            }
            try
            {
                ClientSocket.Close();
                ClientSocket.Dispose();
            }
            catch { }
            IsConnected = false;
            OnConnectionBreaked(endWith);
        }

        void InnerCommandPass(ConnectObjectFromServer serverData)
        {
            for (int i = 0; i < serverData.Commands.Length; i++)
            {
                ConnectCommand command = serverData.Commands[i];
                PassCommand(command);
            }
        }

        protected abstract void PassCommand(ConnectCommand command);
        protected virtual void OnConnectionBegin() { }
        protected virtual void OnConnectionBreaked(Exception reason) { }
    }

    public struct RegisterData
    {
        public RegisterData(RegisterResult r, int u)
        {
            result = r;
            uid = u;
        }

        public RegisterResult result;
        public int uid;
    }
}
