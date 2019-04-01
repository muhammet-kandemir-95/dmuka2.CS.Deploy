using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dmuka2.CS.Deploy
{
    public static class LogHelper
    {
        #region Variables
        static object __lockObject = false;
        #endregion

        #region Methods
        /// <summary>
        /// This method is for write logs to correct path.
        /// </summary>
        /// <param name="projectName">Which is project?</param>
        /// <param name="text">What will be written?</param>
        public static void Write(string projectName, string text)
        {
            lock (__lockObject)
            {
                var directoryName = Path.Combine(Directory.GetCurrentDirectory(), "Log", projectName);
                if (Directory.Exists(directoryName) == false)
                    Directory.CreateDirectory(directoryName);

                File.AppendAllText(
                    Path.Combine(directoryName, DateTime.Now.ToString("yyyy-MM-dd") + ".txt"),
                    text + Environment.NewLine
                    );
            }
        }
        #endregion
    }
}
