﻿using System;
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
        public static string AgentLogDirectory { get; private set; }
        #endregion

        #region Constructors
        static AgentHelper()
        {
            var logDirectoryPath = Path.Combine(Program.CurrentDirectory, "Log");
            if (Directory.Exists(logDirectoryPath) == false)
                Directory.CreateDirectory(logDirectoryPath);

            AgentLogDirectory = Path.Combine(logDirectoryPath, "Agent");
            if (Directory.Exists(AgentLogDirectory) == false)
                Directory.CreateDirectory(AgentLogDirectory);
        }
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
                        Thread.Sleep(1000);
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
        /// Return the agent log path of project.
        /// </summary>
        /// <param name="projectName">Which project in config.json?</param>
        /// <returns></returns>
        public static string GetAgentLogFilePath(string projectName)
        {
            return Path.Combine(AgentLogDirectory, projectName + "__agent_log.txt");
        }

        /// <summary>
        /// This method provides short name of byte value.
        /// <para></para>
        /// For example, if value is 1024 byte, result will be "1 KB".
        /// </summary>
        /// <param name="value">Byte value.</param>
        /// <returns></returns>
        public static string ByteShort(long value)
        {
            if (value < 1024)
                return value + " B";
            else if (value < 1024 * 1024)
                return (value / 1024/*KB*/).ToString("N2") + " KB";
            else if (value < 1024 * 1024 * 1024)
                return (value / 1024/*KB*/ / 1024/*MB*/).ToString("N2") + " MB";
            else if (value < 1024 * 1024 * 1024)
                return (value / 1024/*KB*/ / 1024/*MB*/ / 1024/*GB*/).ToString("N2") + " GB";

            return (value / 1024/*KB*/ / 1024/*MB*/ / 1024/*GB*/ / 1024/*TB*/).ToString("N2") + " TB";
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

            double? totalTicks = null;
            DateTime? calcDate = null;
            Process process = null;
            try
            {
                process = Process.GetProcessById(Convert.ToInt32(ProcessSaveHelper.Get(projectName)));
                if (process.HasExited == false)
                {
                    totalTicks = process.TotalProcessorTime.Ticks;
                    calcDate = DateTime.Now;
                }
                else
                    process = null;
            }
            catch { }

            if (process != null)
            {
                Thread.Sleep(300);
                cpuPercent = ((process.TotalProcessorTime.Ticks - totalTicks.Value) / (DateTime.Now - calcDate.Value).Ticks) * 100;
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
                while (__exit == false)
                {
                    try
                    {
                        var projectUsage = GetProjectUsage(projectName);
                        if (projectUsage.cpuPercent != null)
                        {
                            File.AppendAllText(
                                GetAgentLogFilePath(projectName),
                                string.Format("CPU : {0}%, RAM : {1}, TIME : {2}", projectUsage.cpuPercent.Value.ToString("N2"), ByteShort(projectUsage.ramUsage.Value), DateTime.Now.ToString("dd.MM.yyyy HH:mm")) + Environment.NewLine
                                );
                        }

                    }
                    catch { }

                    // Every 30 minute.
                    Thread.Sleep(1000 * 60 * 30);
                }
            }).Start();
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
