using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace dmuka2.CS.Deploy
{
	public static class ShellHelper
	{
		#region Methods
		/// <summary>
		/// This method will run a shell command.
		/// <para></para>
		/// You can also get output which log is given by the new process.
		/// <para></para>
		/// You only need to fill "callbacks" and set "log" parameter to true and set "wait" parameter to true.
		/// <para></para>
		/// </summary>
		/// <param name="workingDirectory">Which directory will it work on?</param>
		/// <param name="command">Shell command.</param>
		/// <param name="log">Is log active?</param>
		/// <param name="wait">Is wait active?</param>
		/// <param name="callbackOutput">Callback output event.</param>
		/// <param name="callbackError">Callback error event.</param>
		/// <param name="useShell">If this value is false, command will be divided according to first space and then find file name and arguments and then it will work without shell.</param>
		public static Process Run(string workingDirectory, string command, bool log, bool wait, Action<Process, string> callbackOutput = null, Action<Process, string> callbackError = null, bool useShell = true, Action<Process> callbackStarted = null)
		{
			callbackOutput = callbackOutput ?? ((a, b) => { });
			callbackError = callbackError ?? ((a, b) => { });

			string filePath = "sh";
			string args = "-c \"" + command.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";

			if (useShell == false)
			{
				int spaceIndex = command.IndexOf(' ');
				if (spaceIndex == -1)
				{
					filePath = command;
					args = "";
				}
				else
				{
					if (spaceIndex + 1 == command.Length)
					{
						filePath = command.Substring(0, command.Length - 1);
						args = "";
					}
					else
					{
						filePath = command.Substring(0, spaceIndex);
						args = command.Substring(spaceIndex);
					}
				}
			}

			var process = new Process()
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = filePath,
					WorkingDirectory = Path.GetFullPath(Path.Combine(Program.CurrentDirectory, workingDirectory)),
					Arguments = args,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardInput = true,
					RedirectStandardError = true,
					ErrorDialog = false
				}
			};

			process.OutputDataReceived += (a, b) => callbackOutput(process, b.Data ?? "");
			process.ErrorDataReceived += (a, b) => callbackError(process, b.Data ?? "");
			process.Start();

			if (callbackStarted != null)
				callbackStarted(process);

			if (log == true)
			{
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
			}

			if (wait == true)
				process.WaitForExit();

			return process;
		}
		#endregion
	}
}
