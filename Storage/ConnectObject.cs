using Island.StandardLib.Math;
using Island.StandardLib.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Island.StandardLib.Storage
{
    /// <summary>
    /// 表示一次客户端报文的所有内容
    /// </summary>
    public abstract class ConnectObject
    {
        /// <summary>
        /// 指令列表
        /// </summary>
        public StorableFixedArray<ConnectCommand> Commands;

        protected void ReadCommands(DataStorage data) => data.Read(out Commands);
        protected void WriteCommands(DataStorage data) => data.Write(Commands);
        protected void InitCommands() => Commands = new StorableFixedArray<ConnectCommand>();

        /// <summary>
        /// 检查是否包含具有指定名称的指令
        /// </summary>
        /// <param name="commandName">指令名称</param>
        public bool HasCommand(int commandName)
        {
            for (int i = 0; i < Commands.Length; i++)
            {
                if (Commands[i].Name == commandName)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="command">指令名称</param>
        /// <param name="replaceSameCommand">是否替换现有相同名称的指令</param>
        public void AddCommand(ConnectCommand command, bool replaceSameCommand = false)
        {
            lock (this)
            {
                if (replaceSameCommand)
                {
                    for (int i = 0; i < Commands.Length; i++)
                    {
                        if (Commands[i].Name == command.Name)
                        {
                            Commands[i].Args = command.Args;
                            return;
                        }
                    }
                }
                Commands.Add(command);
            }
        }

        /// <summary>
        /// 清除指令
        /// </summary>
        public void ClearCommands()
        {
            Commands.Clear();
        }
    }

    /// <summary>
    /// 表示一次客户端报文的所有内容
    /// </summary>
    public class ConnectObjectFromClient : ConnectObject, IStorable
    {
        public ConnectObjectFromClient() => InitCommands();

        public void ReadFromData(DataStorage data)
        {
            ReadCommands(data);
        }

        public void WriteToData(DataStorage data)
        {
            WriteCommands(data);
        }
    }

    /// <summary>
    /// 表示一次服务器报文的所有内容
    /// </summary>
    public class ConnectObjectFromServer : ConnectObject, IStorable
    {
        public ConnectObjectFromServer()
        {
            InitCommands();
        }

        public void ReadFromData(DataStorage data)
        {
            ReadCommands(data);
        }

        public void WriteToData(DataStorage data)
        {
            WriteCommands(data);
        }
    }

    /// <summary>
    /// 表示一个指令
    /// </summary>
    public class ConnectCommand : IStorable
    {
        /// <summary>
        /// 指令名称
        /// </summary>
        public int Name;
        /// <summary>
        /// 指令参数列表
        /// </summary>
        public StorableMultArray Args;

        public ConnectCommand() => Args = new StorableMultArray();

        public ConnectCommand(int command, params MultData[] args)
        {
            Args = new StorableMultArray();
            Name = command;
            for (int i = 0; i < args.Length; i++) Args.Add(args[i]);
        }

        public void ReadFromData(DataStorage data)
        {
            data.Read(out Name);
            data.Read(out Args);
        }

        public void WriteToData(DataStorage data)
        {
            data.Write(Name);
            data.Write(Args);
        }

        /// <summary>
        /// 获取某个参数
        /// </summary>
        /// <param name="index">参数位置</param>
        public MultData this[int index] => Args[index];
    }
}
