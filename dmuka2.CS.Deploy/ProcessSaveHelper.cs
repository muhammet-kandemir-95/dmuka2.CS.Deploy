using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace dmuka2.CS.Deploy
{
    public static class ProcessSaveHelper
    {
        #region Variables
        static string __processesFilePath = null;
        #endregion

        #region Constructors
        static ProcessSaveHelper()
        {
            __processesFilePath = Path.Combine(Directory.GetCurrentDirectory(), "processes.txt");

            // We are checking that did pc restart?
            if (
                File.Exists(__processesFilePath) == false ||
                (DateTime.UtcNow.AddMilliseconds(-1 * Environment.TickCount) - new DateTime(Convert.ToInt64(File.ReadAllText(__processesFilePath).Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries)[0]))).TotalSeconds > 1
                )
                File.WriteAllText(__processesFilePath, DateTime.UtcNow.AddMilliseconds(-1 * Environment.TickCount).Ticks.ToString());
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get process id from disk.
        /// </summary>
        /// <param name="name">Process id on config.json.</param>
        /// <returns></returns>
        public static string Get(string name)
        {
            var processesList = File.ReadAllText(__processesFilePath).Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < processesList.Length; i++)
            {
                var process = processesList[i];

                string processName = process.Split('/')[0];

                if (processName == name)
                    return process.Split('/')[1];
            }

            return "";
        }

        /// <summary>
        /// Set process id on disk.
        /// </summary>
        /// <param name="name">Process id on config.json.</param>
        /// <param name="processId">New process id.</param>
        public static void Set(string name, string processId)
        {
            var processesList = File.ReadAllText(__processesFilePath).Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var newRow = name + "/" + processId;

            var exists = false;
            for (int i = 1; i < processesList.Count; i++)
            {
                var process = processesList[i];

                string processName = process.Split('/')[0];

                if (processName == name)
                {
                    exists = true;
                    processesList[i] = newRow;
                    break;
                }
            }

            if (exists == false)
                processesList.Add(newRow);

            File.WriteAllText(__processesFilePath, string.Join("~", processesList));
        }
        #endregion
    }
}
