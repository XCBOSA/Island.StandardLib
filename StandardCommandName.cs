using Island.StandardLib.Storage;

namespace Island.StandardLib
{
    public static class StandardCommandName
    {
        #region DescClasses
        /// <summary>
        /// 这是一个 客户端发给服务器的 指令
        /// </summary>
        public sealed class ClientToServerCommand { }

        /// <summary>
        /// 这是一个 服务器发给客户端的 指令
        /// </summary>
        public sealed class ServerToClientCommand { }
        #endregion

        /// <summary>
        /// 表示和聊天相关的指令
        /// </summary>
        public const int Command_Module_Chat = 0xBA;

        /// <summary>
        /// [<see cref="ClientToServerCommand"/>] 发送消息指令，包含一个 <see cref="string"/> 参数作为消息内容
        /// </summary>
        public const int Command_Chat_Send = 0xBA0;

        /// <summary>
        /// [<see cref="ServerToClientCommand"/>] 接收消息指令，包含一个 <see cref="string"/> 参数作为消息内容
        /// </summary>
        public const int Command_Chat_Recv = 0xBA1;

        /// <summary>
        /// [<see cref="ServerToClientCommand"/>] 消息发送失败指令，包含一个 <see cref="int"/> 参数作为原因
        /// </summary>
        public const int Command_Chat_Reject = 0xBA2;
        public const int Command_Chat_Reject_ByTooQuickly = 0xBA200;
        public const int Command_Chat_Reject_ByBanned = 0xBA201;

        /// <summary>
        /// 表示和比赛房间相关的指令
        /// </summary>
        public const int Command_Module_Room = 0xCA;

        /// <summary>
        /// [<see cref="ServerToClientCommand"/>] 比赛已匹配成功的通知指令
        /// </summary>
        public const int Command_Room_Founded = 0xCA0;

        /// <summary>
        /// [<see cref="ServerToClientCommand"/>] 比赛已结束的通知指令，包含一个 <see cref="RoomEndData"/> 参数返回比赛结果
        /// </summary>
        public const int Command_Room_End = 0xCA1;

        /// <summary>
        /// [<see cref="ClientToServerCommand"/>] 开始匹配指令
        /// </summary>
        public const int Command_Room_JoinRequest = 0xCA2;

        /// <summary>
        /// [<see cref="ServerToClientCommand"/>] 服务器已处理寻找房间指令（匹配中）
        /// </summary>
        public const int Command_Room_RecvRequest = 0xCA3;

        /// <summary>
        /// [<see cref="ServerToClientCommand"/>] 服务器拒绝处理寻找房间指令（匹配失败），包含一个 <see cref="int"/> 参数作为原因
        /// </summary>
        public const int Command_Room_RejectRequest = 0xCA4;
        public const int Command_Room_RejectRequest_ByGaming = 0xCA400;
        public const int Command_Room_RejectRequest_ByBanned = 0xCA401;

    }
}
