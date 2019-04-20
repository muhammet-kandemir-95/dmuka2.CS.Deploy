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
            __processesFilePath = Path.Combine(Program.CurrentDirectory, "Processes");
			if (Directory.Exists(__processesFilePath) == false)
				Directory.CreateDirectory(__processesFilePath);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get process id from disk.
        /// </summary>
        /// <param name="name">Project name on config.json.</param>
        /// <returns></returns>
        public static string Get(string name)
        {
            var processFilePathByName = Path.Combine(__processesFilePath, name + ".txt");
            if (File.Exists(processFilePathByName) == false)
                return "";

            checkPCRestart(processFilePathByName);
            var processesList = File.ReadAllText(processFilePathByName).Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries);

            if (processesList.Length == 2)
                return processesList[1];

            return "";
        }

        /// <summary>
        /// Set process id on disk.
        /// </summary>
        /// <param name="name">Project name on config.json.</param>
        /// <param name="processId">New process id.</param>
        public static void Set(string name, string processId)
        {
            var processFilePathByName = Path.Combine(__processesFilePath, name + ".txt");
            checkPCRestart(processFilePathByName);

            var processesList = File.ReadAllText(processFilePathByName).Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var newRow = processId;

            if (processesList.Count == 2)
                processesList[1] = newRow;
            else
                processesList.Add(newRow);

            File.WriteAllText(processFilePathByName, string.Join("~", processesList));
        }

        private static void checkPCRestart(string processFilePathByName)
        {
            // We are checking that did pc restart?
            if (
                File.Exists(processFilePathByName) == false ||
                (DateTime.UtcNow.AddMilliseconds(-1 * Environment.TickCount) - new DateTime(Convert.ToInt64(File.ReadAllText(processFilePathByName).Split(new char[] { '~' }, StringSplitOptions.RemoveEmptyEntries)[0]))).TotalSeconds > 1
                )
                File.WriteAllText(processFilePathByName, DateTime.UtcNow.AddMilliseconds(-1 * Environment.TickCount).Ticks.ToString());
        }
        #endregion
    }
}
