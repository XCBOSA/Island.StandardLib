using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Island.StandardLib.MapGenerator.ThreadTask
{
    public static class OrderedThreadQueue
    {
        static SortedList<float, Action> queueThreads;
        static int runningThreadNum;
        static Random rd;
        static object lck;

        public static int runningCount => runningThreadNum;
        public static int waitingCount => queueThreads.Count;
        public static int totalCount => runningCount + waitingCount;

        public static int MaxUseableThreadCount;

        public static void Init()
        {
            lck = new object();
            lock (lck)
            {
                queueThreads = new SortedList<float, Action>();
                runningThreadNum = 0;
                rd = new Random();
                MaxUseableThreadCount = Environment.ProcessorCount - 2;
                if (MaxUseableThreadCount <= 0) MaxUseableThreadCount = 1;
            }
        }

        public static void QueueTask(Action threadAction, float order)
        {
            if (queueThreads == null) Init();
            lock (lck)
            {
                while (queueThreads.ContainsKey(order))
                    order += (float)rd.NextDouble() - 0.5f;
                queueThreads.Add(order, threadAction);
                if (runningThreadNum < MaxUseableThreadCount)
                {
                    runningThreadNum++;
                    Thread bgThread = new Thread(() =>
                    {
                        while (queueThreads.Count > 0)
                        {
                            Thread.Sleep(1);
                            try
                            {
                                Action act;
                                lock (lck)
                                {
                                    act = queueThreads.First().Value;
                                    queueThreads.RemoveAt(0);
                                }
                                act();
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                        lock (lck)
                            runningThreadNum--;
                    })
                    {
                        IsBackground = true
                    };
                    bgThread.Start();
                }
            }
        }
    }
}
