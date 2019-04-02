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
        public static void Run(string workingDirectory, string command, bool log, bool wait, Action<Process, string> callbackOutput = null, Action<Process, string> callbackError = null)
        {
            callbackOutput = callbackOutput ?? ((a, b) => { });
            callbackError = callbackError ?? ((a, b) => { });

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "sh",
                    WorkingDirectory = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), workingDirectory)),
                    Arguments = "-c \"" + command.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"",
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

            if (log == true)
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }

            if (wait == true)
                process.WaitForExit();
        }
        #endregion
    }
}
