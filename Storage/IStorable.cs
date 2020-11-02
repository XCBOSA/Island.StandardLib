using System;
using System.Collections.Generic;
using System.Text;

namespace Island.StandardLib.Storage
{
    /// <summary>
    /// 表示类实现数据序列化
    /// </summary>
    public interface IStorable
    {
        /// <summary>
        /// 当需要存储数据时被调用，在此处应进行写入容器的操作
        /// </summary>
        /// <param name="data">序列化数据容器</param>
        void WriteToData(DataStorage data);

        /// <summary>
        /// 当需要解析数据时被调用，在此处应进行从容器读取内容的操作
        /// </summary>
        /// <param name="data">序列化数据容器</param>
        void ReadFromData(DataStorage data);
    }
}
