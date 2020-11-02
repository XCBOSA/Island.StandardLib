using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Island.StandardLib.Exceptions
{
    public class PlayerItemsException : Exception
    {
        public string PlayerName;
        public int AccountId;
        public string Requirements;

        public PlayerItemsException(string playername, int accid, string reqs)
        {
            PlayerName = playername;
            AccountId = accid;
            Requirements = reqs;
        }

        public override string Message => $"PlayerItemsException: At Player {PlayerName}(ID{AccountId}): Can not pass requirements {Requirements}.";
    }
}
