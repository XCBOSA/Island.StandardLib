using Island.StandardLib.Storage.Local;
using System;
using System.Collections.Generic;
using System.Text;

namespace Island.StandardLib.Exceptions
{
    public class PlayerSocketFatalException : Exception
    {
        public string PlayerName { get; private set; }
        public PlayerSocketFatalExceptionType WhatHappend { get; private set; }
        public string ExtraToSay { get; private set; }

        public PlayerSocketFatalException(string playerName, PlayerSocketFatalExceptionType whatHappend)
        {
            PlayerName = playerName;
            WhatHappend = whatHappend;
        }

        public override string Message => $"Player {PlayerName} Disconnected due a fatal error, ErrorCode={WhatHappend}{(ExtraToSay == null ? "" : $", ErrorMessage={ExtraToSay}")}";
        public override string ToString() => Message;
    }

    public enum PlayerSocketFatalExceptionType
    {
        PlayerCloseConnection,
        FatalException,
        RecvBufferTooLong,
        HashFailException
    }
}
