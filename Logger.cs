using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Island.StandardLib
{
    /// <summary>
    /// 表示输出内容的等级
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// 提示
        /// </summary>
        Info,
        /// <summary>
        /// 正常
        /// </summary>
        Default,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }

    internal class LogLine
    {
        internal string str;
        internal LogLevel lvl;
    }

    public interface ILogger
    {
        void Write(string data);
        void WriteLine(string data);
        void SetForegroundColor(Color color);
        void GetForegroundColor(ref Color color);
        void ReadLine(ref string str);
    }

    public static class Logger
    {
        public static bool ShowInfo = true;
        public static ILogger __Logger__;
        public static bool ShowDefault = true;
        public static bool ShowWarning = true;
        static bool mainThread = false;
        public static bool RunInMainThread
        {
            get
            {
                return mainThread;
            }
            set
            {
                mainThread = value;
                if (value)
                    WriteLine(LogLevel.Warning, "已强制Logger使用单线程，这将影响服务器执行效率。");
                else WriteLine(LogLevel.Warning, "已关闭强制Logger使用单线程。");
            }
        }
        static StreamWriter writer;
        static ConsoleColor srcColor;
        static Color srcColor2;
        static List<LogLine> lines;

        public static void InitLoggerOnce(bool saveToFile = true)
        {
            if (saveToFile)
            {
                if (writer != null) return;
                string file = "log.txt";
                writer = new StreamWriter(file, true, Encoding.UTF8);
            }
            if (__Logger__ == null)
                srcColor = Console.ForegroundColor;
            else __Logger__.GetForegroundColor(ref srcColor2);
            lines = new List<LogLine>();
            Thread thread = new Thread(Loop);
            thread.Start();
        }

        static void Loop()
        {
            while (true)
            {
                lock (lines)
                {
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (i < 0) continue;
                        if (lines[i] != null)
                            writeInternal(lines[i].lvl, lines[i].str);
                        lines.RemoveAt(i);
                        i--;
                    }
                }
                Thread.Sleep(1);
            }
        }

        static void writeInternal(LogLevel level, string str)
        {
            string timeStr = "[" + DateTime.Now.ToString() + "] ";
            string logStr = timeStr;
            switch (level)
            {
                case LogLevel.Default:
                    logStr += "[Default] ";
                    break;
                case LogLevel.Error:
                    logStr += "[Error] ";
                    break;
                case LogLevel.Info:
                    logStr += "[Info] ";
                    break;
                case LogLevel.Warning:
                    logStr += "[Warning] ";
                    break;
            }
            logStr += str;
            writer?.WriteLine(logStr);
            writer?.Flush();
            if (__Logger__ == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(timeStr);
            }
            else
            {
                __Logger__.SetForegroundColor(Color.DarkGray);
                __Logger__.Write(timeStr);
            }
            string writeStr = str;
            switch (level)
            {
                case LogLevel.Default:
                    if (!ShowDefault) return;
                    if (__Logger__ == null)
                        Console.ForegroundColor = ConsoleColor.White;
                    else
                        __Logger__.SetForegroundColor(Color.White);
                    break;
                case LogLevel.Error:
                    if (__Logger__ == null)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        __Logger__.SetForegroundColor(Color.Red);
                    break;
                case LogLevel.Info:
                    if (!ShowInfo) return;
                    if (__Logger__ == null)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    else
                        __Logger__.SetForegroundColor(Color.DarkGray);
                    break;
                case LogLevel.Warning:
                    if (!ShowWarning) return;
                    if (__Logger__ == null)
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        __Logger__.SetForegroundColor(Color.Yellow);
                    break;
            }
            if (__Logger__ == null)
            {
                Console.WriteLine(writeStr);
                Console.ForegroundColor = srcColor;
            }
            else
            {
                __Logger__.WriteLine(writeStr);
                __Logger__.SetForegroundColor(srcColor2);
            }
        }

        public static void Log(LogLevel level, string str)
        {
            WriteLine(level, str);
        }

        public static void Log(string str)
        {
            WriteLine(str);
        }

        public static void Log(string str, params object[] cs)
        {
            WriteLine(str, cs);
        }

        public static void Log(LogLevel level, string str, params object[] cs)
        {
            WriteLine(level, str, cs);
        }

        public static void WriteLine(LogLevel level, string str)
        {
            if (mainThread)
            {
                writeInternal(level, str);
            }
            else
            {
                LogLine ll = new LogLine();
                ll.lvl = level;
                ll.str = str;
                lines.Add(ll);
            }
        }

        public static void WriteLine(string str)
        {
            WriteLine(LogLevel.Default, str);
        }

        public static void WriteLine(string str, params object[] cs)
        {
            string c = str;
            for (int i = 0; i < cs.Length; i++)
                c = c.Replace("{" + i + "}", cs[i].ToString());
            WriteLine(c);
        }

        public static void WriteLine(LogLevel level, string str, params object[] cs)
        {
            string c = str;
            for (int i = 0; i < cs.Length; i++)
                c = c.Replace("{" + i + "}", cs[i].ToString());
            WriteLine(level, c);
        }

        public static void LogError(Exception e)
        {
            WriteLine(LogLevel.Error, e.ToString());
        }

        public static string ReadLine()
        {
            string cmd = null;
            if (__Logger__ == null)
                cmd = Console.ReadLine();
            else __Logger__.ReadLine(ref cmd);
            return cmd;
        }

        public static string ReadData(string helpText, string defValue = null)
        {
            string txt = helpText + (defValue == null ? "" : $"({defValue})") + "> ";
            if (__Logger__ == null)
                Console.Write(txt);
            else __Logger__.Write(txt);
            string dat = ReadLine();
            return dat == "" ? defValue : dat;
        }

        public static T ReadData<T>(string helpText, Func<string, T> parseFunc)
        {
            while (true)
            {
                string data = ReadData(helpText);
                try
                {
                    return parseFunc(data);
                }
                catch
                {
                    if (__Logger__ == null)
                        Console.WriteLine("Parse Error, Retry.");
                    else __Logger__.WriteLine("Parse Error, Retry.");
                }
            }
        }

        public static T ReadData<T>(string helpText, Func<string, T> parseFunc, string defValue)
        {
            while (true)
            {
                string data = ReadData(helpText, defValue);
                try
                {
                    return parseFunc(data);
                }
                catch
                {
                    if (__Logger__ == null)
                        Console.WriteLine("Parse Error, Retry.");
                    else __Logger__.WriteLine("Parse Error, Retry.");
                }
            }
        }
    }
}
