using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace dmuka2.CS.Deploy
{
    /// <summary>
    /// This class is to manage to get logs from processes.
    /// <para></para>
    /// You can manage this situations and send emails to your service when you get any error from processes.
    /// </summary>
    public static class AgentHelper
    {
        #region Variables
        /// <summary>
        /// To manage the multiple processes via one thread.
        /// </summary>
        static Queue<Action> __queue = new Queue<Action>();
        static bool __exit = false;
        #endregion

        #region Methods
        /// <summary>
        /// To start the queue thread to consume the queue.
        /// </summary>
        public static void StartTheQueue()
        {
            new Thread(() =>
            {
                while (true)
                {
                    Action _queueAction = null;
                    lock (__queue)
                    {
                        if (__queue.Count != 0)
                            _queueAction = __queue.Dequeue();
                        else if (__exit == true)
                            break;
                    }

                    if (_queueAction != null)
                    {
                        try
                        {
                            _queueAction();
                        }
                        catch { }
                    }
                    else
                        Thread.Sleep(1);
                }
            }).Start();
        }

        /// <summary>
        /// To stop the queue thread.
        /// </summary>
        public static void StopTheQueue()
        {
            __exit = true;
        }

        /// <summary>
        /// When any event is triggered, this event is called.
        /// </summary>
        /// <param name="action"></param>
        public static void AddToQueue(Action action)
        {
            lock (__queue)
                __queue.Enqueue(action);
        }

        /// <summary>
        /// This method calculates to cpu usage and how much does it use the ram as byte.
        /// <para></para>
        /// Parameter will be null when the process isn't working.
        /// <para></para>
        /// This process will take 2 second to calculate the datas.
        /// </summary>
        /// <param name="projectName">Which project in config.json?</param>
        /// <returns></returns>
        public static (double? cpuPercent, long? ramUsage) GetProjectUsage(string projectName)
        {
            double? cpuPercent = null;
            long? ramUsage = null;

            double? totalMillisecond = null;
            DateTime? calcDate = null;
            Process process = null;
            try
            {
                process = Process.GetProcessById(Convert.ToInt32(ProcessSaveHelper.Get(projectName)));
                if (process.HasExited == false)
                {
                    totalMillisecond = process.TotalProcessorTime.TotalMilliseconds;
                    calcDate = DateTime.Now;
                }
                else
                    process = null;
            }
            catch { }

            if (process != null)
            {
                cpuPercent = ((process.TotalProcessorTime.TotalMilliseconds - totalMillisecond.Value) / (DateTime.Now - calcDate.Value).TotalMilliseconds) * 100;
                ramUsage = process.WorkingSet64;
            }

            return (cpuPercent: cpuPercent, ramUsage: ramUsage);
        }

        /// <summary>
        /// When any process starts, but it doesn't mean that process just has opened.
        /// <para></para>
        /// It may take many times to build.
        /// </summary>
        /// <param name="projectName">Which project in config.json?</param>
        public static void OnProcessStart(string projectName)
        {
            new Thread(() =>
            {
                while (true)
                {
                    // This is only the example to learn.
                    var projectUsage = GetProjectUsage(projectName);
                    if (projectUsage.cpuPercent != null)
                    {
                        File.AppendAllText(
                            Path.Combine(Directory.GetCurrentDirectory(), "agent-log.txt"),
                            string.Format("CPU : {0}%, RAM : {1} MB", projectUsage.cpuPercent.Value.ToString("N2"), (projectUsage.ramUsage.Value / 1024/*KB*/ / 1024/*MB*/).ToString("N2")) + Environment.NewLine
                            );
                    }

                    // Every 5 minute.
                    Thread.Sleep(1000 * 60 * 5);
                }
            });
        }

        /// <summary>
        /// When any process throw the log.
        /// </summary>
        /// <param name="isError">Is the log error?</param>
        /// <param name="text">What is the log?</param>
        public static void OnLog(string projectName, bool isError, string text)
        {

        }
        #endregion
    }
}
