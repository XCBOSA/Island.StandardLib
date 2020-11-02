using Island.StandardLib.Storage.Local;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#pragma warning disable IDE0032

namespace Island.StandardLib.Storage
{
    public class LoginOrRegisterRequest : IStorable
    {
        bool isLogin, isLobbyLogin;
        public int Username;
        public string Password;
        public string Nickname;
        public uint ClientVersion;

        /// <summary>
        /// 反序列化时使用的构造函数
        /// </summary>
        public LoginOrRegisterRequest() { }

        /// <summary>
        /// 创建登录或注册请求
        /// </summary>
        /// <param name="userName">ID（注册时可不填）</param>
        /// <param name="password">密码</param>
        /// <param name="version">当前客户端版本</param>
        public LoginOrRegisterRequest(int userName, string password, uint version)
        {
            Username = userName;
            Password = password;
            ClientVersion = version;
        }

        public bool IsLogin
        {
            get => isLogin;
            set => isLogin = value;
        }

        public bool IsRegister
        {
            get => !isLogin;
            set => isLogin = !value;
        }

        public bool IsLobbyLogin
        {
            get => isLobbyLogin;
            set
            {
                isLobbyLogin = value;
                if (value) isLogin = true;
            }
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out isLogin);
            data.Read(out isLobbyLogin);
            data.Read(out Username);
            data.Read(out Password);
            data.Read(out Nickname);
            data.Read(out ClientVersion);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(isLogin);
            data.Write(isLobbyLogin);
            data.Write(Username);
            data.Write(Password);
            data.Write(Nickname);
            data.Write(ClientVersion);
        }
    }

    public class RegisterCallback : IStorable
    {
        public int Username;
        public bool Success => Username > 0;

        public void ReadFromData(DataStorage data)
        {
            data.Read(out Username);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(Username);
        }
    }

    public class LoginCallback : IStorable
    {
        public LoginResult Code;
        public bool Success => Code == 0;

        public void ReadFromData(DataStorage data)
        {
            data.Read(out int code);
            Code = (LoginResult)code;
        }

        public void WriteToData(DataStorage data)
        {
            data.Write((int)Code);
        }
    }

    public class LobbyLoginCallback : IStorable
    {
        public LoginResult Code;
        public bool Success => Code == 0;
        public string UserNickName;
        public bool CanLogin;
        public int CurrentMapSeed;
        public int CurrentDifficulty;

        public void ReadFromData(DataStorage data)
        {
            data.Read(out int code); Code = (LoginResult)code;
            data.Read(out UserNickName);
            data.Read(out CanLogin);
            data.Read(out CurrentMapSeed);
            data.Read(out CurrentDifficulty);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write((int)Code);
            data.Write(UserNickName);
            data.Write(CanLogin);
            data.Write(CurrentMapSeed);
            data.Write(CurrentDifficulty);
        }
    }

    public enum LoginResult
    {
        Success = 0,
        NoAccountOrPasswordError = 1,
        AlreadyOnline = 2,
        VersionLower = 3,
        VersionHigher = 4,
        ConnectionError = 5
    }

    public enum RegisterResult
    {
        Success = 0,
        NickOrPasswordError = -1,
        VersionLower = -3,
        VersionHigher = -4,
        ConnectionError = -5
    }
}
