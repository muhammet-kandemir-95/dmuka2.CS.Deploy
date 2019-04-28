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
		/// <summary>
		/// We are checking to exist a new log file.
		/// <para></para>
		/// Because if we need a new log file, it means that we won't use previous file.
		/// <para></para>
		/// Thus, we can compress it.
		/// <para></para>
		/// All of this checking on this variable.
		/// </summary>
		static Dictionary<string, string> __projectLastLogFileName = new Dictionary<string, string>();
		#endregion

		#region Methods
		/// <summary>
		/// This method is for write logs to correct path.
		/// <para></para>
		/// This method also compresses the previous log file.
		/// </summary>
		/// <param name="projectName">Which is project?</param>
		/// <param name="text">What will be written?</param>
		public static void Write(string projectName, string text)
		{
			lock (__lockObject)
			{
				var directoryName = Path.Combine(Program.CurrentDirectory, "Log", projectName);
				if (Directory.Exists(directoryName) == false)
					Directory.CreateDirectory(directoryName);

				var logFilePath = Path.Combine(directoryName, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");
				if (__projectLastLogFileName.ContainsKey(projectName) == true)
				{
					var previousLogPath = __projectLastLogFileName[projectName];
					if (previousLogPath != logFilePath)
					{
						var previousLogFileName = Path.GetFileName(previousLogPath);
						ShellHelper.Run(directoryName, "tar -czvf \"" + previousLogFileName + ".tar.gz\" \"" + previousLogFileName + "\" && rm \"" + previousLogFileName + "\"", false, true);
						__projectLastLogFileName[projectName] = logFilePath;
					}
				}
				else
					__projectLastLogFileName.Add(projectName, logFilePath);

				File.AppendAllText(
					logFilePath,
					text + Environment.NewLine
					);
			}
		}
		#endregion
	}
}
