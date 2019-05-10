using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace dmuka2.CS.Deploy
{
	class Program
	{
		#region Variables
		/// <summary>
		/// We sometimes don't need to readline when program gets a line or throws exception.
		/// <para></para>
		/// This variable is for it.
		/// </summary>
		static bool __askDisable = false;
		/// <summary>
		/// Console can't show anything when this variable is true.
		/// </summary>
		static bool __writeDisable = false;

		/// <summary>
		/// We must check that Has bye bye been typed?
		/// </summary>
		static bool __byeByeEnable = true;

		/// <summary>
		/// We must write the logs with orders.
		/// This variable ensure that.
		/// </summary>
		static object __lockWriteLine = true;

		/// <summary>
		/// Current Directory
		/// </summary>
		internal static string CurrentDirectory = "";
		#endregion

		#region Methods
		/// <summary>
		/// To write a text to console with styles. You have to use some schemas below;
		/// <para></para>
		/// [line][&lt;type&gt;] = Write a line by console buffer width. For instance "[line][01]", "[line][02]", ...
		/// <para></para>
		/// [color][&lt;forecolor&gt;,&lt;backcolor&gt;] = Change the console color. For instance "[color][01,02]Hello [color][15,03]World"
		/// </summary>
		/// <param name="text">Console text.</param>
		/// <param name="arguments">String format arguments.</param>
		static void write(string text, params object[] arguments)
		{
			if (__writeDisable == true)
				return;

			lock (__lockWriteLine)
			{
				text = text
						.Replace("[line][01]", "[color][01,--]" + "".PadRight(Console.BufferWidth, '─'))
						.Replace("[line][02]", "[color][01,--]" + "".PadRight(Console.BufferWidth, '═'))
						.Replace("[line][03]", "[color][01,--]" + "".PadRight(Console.BufferWidth, '.'));

				for (int i = 0; i < arguments.Length; i++)
					text = text.Replace("{" + i + "}", arguments[i].ToString());

				var split = text.Split(new string[] { "[color]" }, StringSplitOptions.None);
				var previousForeColor = Console.ForegroundColor;
				var previousBackColor = Console.BackgroundColor;

				for (int i = 0; i < split.Length; i++)
				{
					var item = split[i];
					if (i != 0)
					{
						var colors = item.Substring(0, "[--,--]".Length);
						item = item.Substring("[--,--]".Length);

						var foreColor = colors.Split(',')[0].Replace("[", "");
						var backColor = colors.Split(',')[1].Replace("]", "");
						if (foreColor != "--")
							Console.ForegroundColor = (ConsoleColor)Convert.ToInt32(foreColor);
						if (backColor != "--")
							Console.BackgroundColor = (ConsoleColor)Convert.ToInt32(backColor);
					}
					Console.Write(item);
				}
				Console.ForegroundColor = previousForeColor;
				Console.BackgroundColor = previousBackColor;
			}
		}

		/// <summary>
		/// To write a line to console with styles. You have to use some schemas below;
		/// <para></para>
		/// [line][&lt;type&gt;] = Write a line by console buffer width. For instance "[line][01]", "[line][02]", ...
		/// <para></para>
		/// [color][&lt;forecolor&gt;,&lt;backcolor&gt;] = Change the console color. For instance "[color][01,02]Hello [color][15,03]World"
		/// </summary>
		/// <param name="text">Console text.</param>
		/// <param name="arguments">String format arguments.</param>
		static void writeLine(string text, params object[] arguments)
		{
			write(text + Environment.NewLine, arguments);
		}

		/// <summary>
		/// To write only a blank line.
		/// </summary>
		static void writeLine()
		{
			write(Environment.NewLine);
		}

		/// <summary>
		/// To catch the error which will be thrown by action.
		/// <para></para>
		/// And then ask it to use to see on the current console.
		/// </summary>
		/// <param name="action">What will it do?</param>
		static bool tryCatch(Action action)
		{
			try
			{
				action();

				return true;
			}
			catch (Exception ex)
			{
				if (__askDisable == false)
					write("[color][03,--]We couldn't. Do you want see the error? [color][01,--](y/n) ");

				if (__askDisable == true || Console.ReadLine().ToLower() == "y")
				{
					writeLine("[color][04,--]" + ex.ToString());
					if (__askDisable == false)
					{
						writeLine("[color][07,--]Press a key to continue...");
						Console.ReadKey(true);
					}
				}

				return false;
			}
		}

		/// <summary>
		/// To ask a question which has 2 answer yes or no.
		/// <para></para>
		/// The question is "Are you sure?".
		/// </summary>
		/// <param name="action">If user will say yes for the question, what will it do.</param>
		static void areYouSure(Action action)
		{
			if (__askDisable == true)
			{
				action();
				return;
			}

			write("[color][15,--]Are you sure? [color][01,--](y/n) ");
			if (Console.ReadLine().ToLower() == "y")
				action();
		}

		/// <summary>
		/// Write the successfull message.
		/// </summary>
		static void successful()
		{
			writeLine(@"[color][10,--]Successfull");
		}

		/// <summary>
		/// Write the bye bye message.
		/// </summary>
		static void byeBye()
		{
			if (__byeByeEnable == false)
				return;

			__byeByeEnable = false;

			writeLine();
			writeLine("[color][13,00]" + "".PadRight(Console.BufferWidth, ' '));
			writeLine("[color][13,00]" + "Bye Bye".PadLeft((Console.BufferWidth + 7) / 2, ' ').PadRight(Console.BufferWidth, ' '));
			writeLine("[color][13,00]" + "".PadRight(Console.BufferWidth, ' '));
			Console.ForegroundColor = (ConsoleColor)(-1);
			Console.BackgroundColor = (ConsoleColor)(-1);
		}
		#endregion

		static void Main(string[] args)
		{
			CurrentDirectory = Directory.GetCurrentDirectory();
			ConfigHelper.SetUserName("");

			Func<string> getNewArgLine = () => throw new Exception("Not found enough arguments!");
			Func<string, string> getLine = (msg) =>
			{
				if (__askDisable == true)
				{
					return getNewArgLine();
				}

				write("[color][07,--]" + msg);

				var previousForeColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.DarkYellow;

				var line = Console.ReadLine();

				Console.ForegroundColor = previousForeColor;

				writeLine("[line][03]");

				return line;
			};

			bool exit = false;

			#region Commands
			List<Command> commands = new List<Command>();
			commands.Add(new Command("help", "", () =>
			{
				var maxLengthLeft = commands.Max(o => o.Name.Length);
				var maxLengthRight = commands.Max(o => o.LongName.Length);

				writeLine("[color][03,--]Run a Command/Commands Schema");
				writeLine("[color][01,--]depmk [color][14,--]<cmd1> <cmd2> <cmd3>...");
				writeLine();

				writeLine("[color][03,--]Run a Command/Commands Schema with Parameter");
				writeLine("[color][01,--]depmk [color][14,--]<cmd1> <parameter1> <cmd2> <parameter1> <cmd3> <parameter1>...");
				writeLine();

				writeLine("[color][03,--]Commands List");

				foreach (var command in commands.OrderBy(o => o.Name.Split(' ')[0]))
					if (command.Name != "help")
					{
						if (command.LongName == "")
							writeLine("[color][11,--]" + command.Name.PadRight(maxLengthLeft + (maxLengthRight == 0 ? 0 : 3 + maxLengthRight), ' ') + " [color][08,--]= [color][15,--]" + command.Description);
						else
							writeLine("[color][11,--]" + command.Name.PadRight(maxLengthLeft, ' ') + " [color][15,--]| [color][11,--]" + command.LongName.PadRight(maxLengthRight, ' ') + " [color][08,--]= [color][15,--]" + command.Description);
					}

				writeLine();
				writeLine("[color][03,--]Example 1 - Run a Command");
				writeLine("[color][01,--]depmk [color][14,--]pr -s");
				writeLine();

				writeLine("[color][03,--]Example 2 - Run Multiple Command");
				writeLine("[color][01,--]depmk [color][14,--]pr -s pr -ka");
				writeLine();

				writeLine("[color][03,--]Example 3 - Run a Command with Parameter");
				writeLine("[color][01,--]depmk [color][14,--]pr -r test_consoleapp");
				writeLine();

				writeLine("[color][03,--]Example 4 - Run Multiple Command with Parameter");
				writeLine("[color][01,--]depmk [color][14,--]pr -r test_consoleapp pr -ka");
			}));
			commands.Add(new Command("version", "Show the version of project.", () =>
			{
				writeLine("[line][01]");
				writeLine("[color][15,--]DEPMK Version [color][07,--]= [color][02,--]" + Assembly.GetExecutingAssembly().GetName().Version);
				writeLine("[line][01]");
			}));
			commands.Add(new Command("exit", "Close this application safely.", () =>
			{
				exit = true;
			}));
			commands.Add(new Command("clear", "Clear console.", () =>
			{
				Console.Clear();
			}));
			commands.Add(new Command("cwd", "Write current directory.", () =>
			{
				writeLine("[color][14,--]" + CurrentDirectory);
			}));
			commands.Add(new Command("show -c", "show --config", "Show config file.", () =>
			{
				tryCatch(() =>
				{
					writeLine("[color][14,--]" + ConfigHelper.Config.ToString());

					successful();
				});
			}));
			commands.Add(new Command("set -c", "set --config", "Set config file.", () =>
			{
				tryCatch(() =>
				{
					string path = getLine("Write config file name = ");
					if (path == "")
						return;

					string newConfigPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path));
					ConfigHelper.Config = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(newConfigPath));
					ConfigHelper.Save();

					successful();
				});
			}));
			commands.Add(new Command("add -p", "add --project", "Add a new project to config.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");
					if (projectName == "")
						return;

					if (ConfigHelper.Projects.Any(o => o == projectName))
						throw new Exception("Project already is exist!");

					string commandName = getLine("Write your command name = ");
					if (commandName == "")
						return;
					string commandArgs = getLine("Write your command arguments = ");
					if (commandArgs == "")
						return;
					((JObject)ConfigHelper.Config["project"])[projectName] = JObject.FromObject(new
					{
						commands = new object[] {
							new {
								main =  true,
								name =  commandName,
								arguments =  commandArgs,
								path =  new {
									@default = Directory.GetCurrentDirectory()
								}
							}
						}
					});
					ConfigHelper.Save();

					successful();
				});
			}));
			commands.Add(new Command("del -p", "del --project", "Remove a project from config.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");
					if (projectName == "")
						return;

					if (ConfigHelper.Projects.Any(o => o == projectName) == false)
						throw new Exception("Project was not found!");

					((JObject)ConfigHelper.Config["project"]).Remove(projectName);
					ConfigHelper.Save();

					successful();
				});
			}));
			string deployShFilePath = Path.Combine(CurrentDirectory, "deploy.sh");
			commands.Add(new Command("add -s", "add --startup", "Add deploy.sh to startup by linux user name.", () =>
			{
				tryCatch(() =>
				{
					string linuxUserName = getLine("Write linux user name = ");
					if (linuxUserName == "")
						return;

					string deployStartupCommand =
							Environment.NewLine +
							"@reboot " + linuxUserName + " " + deployShFilePath +
							Environment.NewLine;

					File.WriteAllText(deployShFilePath,
						"dotnet exec \"" + Path.Combine(CurrentDirectory, "bin/Release/netcoreapp2.1/dmuka2.CS.Deploy.dll") + "\" --current-directory \"" + CurrentDirectory + "\"");

					ShellHelper.Run(
						"",
						"chmod +x " + deployShFilePath,
						true,
						true,
						(process, text) =>
						{
							writeLine(text);
						}, callbackError: (process, text) =>
						{
							writeLine(text);
						});
					string crontabContent = File.ReadAllText("/etc/crontab");

					crontabContent = crontabContent.Replace(deployStartupCommand, "");
					crontabContent += deployStartupCommand;

					File.WriteAllText("/etc/crontab", crontabContent);

					successful();
				});
			}));
			commands.Add(new Command("del -s", "del --startup", "Remove deploy.sh from startup by linux user name.", () =>
			{
				tryCatch(() =>
				{
					string linuxUserName = getLine("Write linux user name = ");
					if (linuxUserName == "")
						return;

					string deployStartupCommand =
							Environment.NewLine +
							"@reboot " + linuxUserName + " " + deployShFilePath +
							Environment.NewLine;

					string crontabContent = File.ReadAllText("/etc/crontab");

					crontabContent = crontabContent.Replace(deployStartupCommand, "");

					File.WriteAllText("/etc/crontab", crontabContent);
					successful();
				});
			}));
			string aliasCommand =
					"alias depmk=\"dotnet exec \\\"" + Path.Combine(CurrentDirectory, "bin/Release/netcoreapp2.1/dmuka2.CS.Deploy.dll") + "\\\" --current-directory \\\"" + CurrentDirectory + "\\\"\"";
			string bashrcFilePath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".bashrc");
			commands.Add(new Command("add -a", "add --alias", "Add alias to bash as 'depmk' via current directory.", () =>
			{
				tryCatch(() =>
				{
					ShellHelper.Run(
						"",
						"dotnet build -c Release",
						true,
						true,
						(process, text) =>
						{
							writeLine(text);
						}, callbackError: (process, text) =>
						{
							writeLine(text);
						});
					string bashrc = string.Join(Environment.NewLine, File.ReadAllText(bashrcFilePath).Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(o => o.Contains("alias depmk") == false));

					bashrc += Environment.NewLine + aliasCommand;

					File.WriteAllText(bashrcFilePath, bashrc);

					successful();
				});
			}));
			commands.Add(new Command("del -a", "del --alias", "Remove alias from .bashrc.", () =>
			{
				tryCatch(() =>
				{
					string bashrc = string.Join(Environment.NewLine, File.ReadAllText(bashrcFilePath).Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(o => o.Contains("alias depmk") == false));

					File.WriteAllText(bashrcFilePath, bashrc);

					successful();
				});
			}));
			commands.Add(new Command("sleep -s", "sleep --second", "Thread sleep as second.", () =>
			{
				tryCatch(() =>
				{
					string value = getLine("Write second = ");
					if (value == "")
						return;

					var second = Convert.ToInt32(value);
					writeLine("[color][13,--]Waiting {0} second...", second);
					Thread.Sleep(second * 1000);
					successful();
				});
			}));
			commands.Add(new Command("sleep -m", "sleep --minute", "Thread sleep as minute.", () =>
			{
				tryCatch(() =>
				{
					string value = getLine("Write minute = ");
					if (value == "")
						return;

					var minute = Convert.ToInt32(value);
					writeLine("[color][13,--]Waiting {0} minute...", minute);
					Thread.Sleep(minute * 1000 * 60);
					successful();
				});
			}));
			commands.Add(new Command("sleep -h", "sleep --hour", "Thread sleep as hour.", () =>
			{
				tryCatch(() =>
				{
					string value = getLine("Write hour = ");
					if (value == "")
						return;

					var hour = Convert.ToInt32(value);
					writeLine("[color][13,--]Waiting {0} hour...", hour);
					Thread.Sleep(hour * 1000 * 60 * 60);
					successful();
				});
			}));
			commands.Add(new Command("set -u", "set --user", "Set user name.", () =>
			{
				var userName = getLine("Write user name = ");
				if (userName == "")
					return;

				ConfigHelper.SetUserName(userName);
				successful();
			}));
			commands.Add(new Command("get -u", "get --user", "Get user name.", () =>
			{
				writeLine("[color][13,--]" + ConfigHelper.UserName);
				successful();
			}));
			List<Process> logProcesses = new List<Process>();
			Action killLogProcesses = () =>
			{
				foreach (var logProcess in logProcesses)
				{
					try
					{
						logProcess.Kill();
					}
					catch { }
				}
				logProcesses.Clear();
			};
			Console.CancelKeyPress += (sender, e) =>
			{
				killLogProcesses();
			};
			Action<string[]> runAgentLogProcesses = (projects) =>
			{
				writeLine("[color][15,--]Agent log was started...");
				writeLine();

				var disableLog = false;
				var maxLength = projects.Max(o => o.Length);
				foreach (var projectName in projects)
				{
					var logAgentFilePath = AgentHelper.GetAgentLogFilePath(projectName);

					if (File.Exists(logAgentFilePath) == false)
						continue;

					var shellProcess = ShellHelper.Run(AgentHelper.AgentLogDirectory, "tail -n 100 -f \"" + Path.GetFileName(logAgentFilePath) + "\"", true, false,
						(process, text) =>
						{
							if (exit == true || disableLog == true)
								return;

							writeLine("[color][14,--]" + projectName.PadRight(maxLength + 2, ' ') + " [color][08,--]| [color][15,--]" + text);
						}, (process, text) =>
						{
							if (exit == true || disableLog == true)
								return;

							writeLine("[color][14,--]" + projectName.PadRight(maxLength + 2, ' ') + " [color][08,--]| [color][15,--]" + text);
						}, useShell: false);

					logProcesses.Add(shellProcess);
				}

				Console.ReadKey(true);
				disableLog = true;
				killLogProcesses();

				writeLine();
				writeLine("[color][15,--]Agent log was closed...");
			};
			commands.Add(new Command("alog -sa", "alog --show-all", "Show agent log of all projects.", () =>
			{
				runAgentLogProcesses(ConfigHelper.Projects);
			}));
			commands.Add(new Command("alog -s", "alog --show", "Show agent log of project/projects.", () =>
			{
				tryCatch(() =>
				{
					var logs = getLine("Write project/projects name(You can use ';' for multiple project) = ").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToArray();
					if (logs.Length == 0)
						return;

					if (logs.Length != ConfigHelper.Projects.Where(o => logs.Any(a => a == o)).Count())
						throw new Exception("Not found project!");

					runAgentLogProcesses(logs);
				});
			}));
			Action<string[]> runLogProcesses = (projects) =>
			{
				writeLine("[color][15,--]Log was started...");
				writeLine();

				var disableLog = false;
				var maxLength = projects.Max(o => o.Length);
				foreach (var projectName in projects)
				{
					var logProjectPath = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
					var logProjectDirectoryPath = Path.Combine(CurrentDirectory, "Log", projectName);

					if (File.Exists(Path.Combine(logProjectDirectoryPath, logProjectPath)) == false)
						continue;

					var shellProcess = ShellHelper.Run(logProjectDirectoryPath, "tail -n 100 -f \"" + logProjectPath + "\"", true, false,
						(process, text) =>
						{
							if (exit == true || disableLog == true)
								return;

							writeLine("[color][14,--]" + projectName.PadRight(maxLength + 2, ' ') + " [color][08,--]| [color][15,--]" + text);
						}, (process, text) =>
						{
							if (exit == true || disableLog == true)
								return;

							writeLine("[color][14,--]" + projectName.PadRight(maxLength + 2, ' ') + " [color][08,--]| [color][15,--]" + text);
						}, useShell: false);

					logProcesses.Add(shellProcess);
				}

				Console.ReadKey(true);
				disableLog = true;
				killLogProcesses();

				writeLine();
				writeLine("[color][15,--]Log was closed...");
			};
			commands.Add(new Command("log -sa", "log --show-all", "Show all projects daily logs.", () =>
			{
				runLogProcesses(ConfigHelper.Projects);
			}));
			commands.Add(new Command("log -s", "log --show", "Show daily logs of project/projects.", () =>
			{
				tryCatch(() =>
				{
					var logs = getLine("Write project/projects name(You can use ';' for multiple project) = ").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToArray();
					if (logs.Length == 0)
						return;

					if (logs.Length != ConfigHelper.Projects.Where(o => logs.Any(a => a == o)).Count())
						throw new Exception("Not found project!");

					runLogProcesses(logs);
				});
			}));
			Action<string> deleteLog = (projectName) =>
			{
				var logDirectoryPath = Path.Combine(CurrentDirectory, "Log", projectName);
				if (Directory.Exists(logDirectoryPath))
					Directory.Delete(logDirectoryPath, true);

				var logAgentFilePath = AgentHelper.GetAgentLogFilePath(projectName);
				if (File.Exists(logAgentFilePath))
					File.Delete(logAgentFilePath);
			};
			commands.Add(new Command("log -r", "log --remove", "Remove all logs of a project.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");
					if (projectName == "")
						return;

					deleteLog(projectName);
					successful();
				});
			}));
			commands.Add(new Command("log -ra", "log --remove-all", "Remove logs of all projects.", () =>
			{
				tryCatch(() =>
				{
					areYouSure(() =>
					{
						foreach (var projectName in ConfigHelper.Projects)
							deleteLog(projectName);
						successful();
					});
				});
			}));
			commands.Add(new Command("mon", "monitor", "Open the monitor to watch a project.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");
					if (projectName == "")
						return;

					if (ConfigHelper.Projects.Any(o => o == projectName) == false)
						throw new Exception("Not found project!");

					Console.Clear();
					int cursorPosition = Console.CursorTop;
					bool exitMonitor = false;
					int consoleWidth = Console.BufferWidth - (Console.BufferWidth % 2);
					int consoleHeight = Console.BufferHeight - (2 - (Console.BufferHeight % 2));

					int graphWidth = consoleWidth - 2;
					int graphHeight = consoleHeight - 2;
					var ramY = ((graphHeight - 1) / 2);

					char[,] graph = new char[graphHeight, graphWidth];
					List<string> previousLines = new List<string>();
					for (int i = 0; i < consoleHeight; i++)
						previousLines.Add("");

					for (int y = 0; y < graphHeight; y++)
						for (int x = 0; x < graphWidth; x++)
							graph[y, x] = ' ';
					bool projectEnable = true;

					Action draw = () =>
					{
						string text = "";
						for (int y = 1; y <= consoleHeight; y++)
						{
							string lineText = "";
							for (int x = 1; x <= consoleWidth; x++)
							{
								if (exitMonitor == true)
									return;

								if (x == 1 && y == 1)
									lineText += "[color][01,-1]┌";
								else if (x == consoleWidth && y == consoleHeight)
									lineText += "[color][01,-1]┘";
								else if (x == consoleWidth && y == 1)
									lineText += "[color][01,-1]┐";
								else if (x == 1 && y == consoleHeight)
									lineText += "[color][01,-1]└";
								else if (x == 1 || x == consoleWidth)
									lineText += "[color][01,-1]│";
								else if (y == 1 || y == consoleHeight)
									lineText += "[color][01,-1]─";
								else
								{
									char graphChar = graph[y - 2, x - 2];
									string color = "";
									if (y == 2 || y == ramY + 3)
										color = projectEnable == true ? "[color][14,-1]" : "[color][08,-1]";
									else if (y == 3 || y == ramY + 4)
									{
										if (x < 6)
											color = "[color][07,-1]";
										else
											color = "[color][11,-1]";
									}
									else if (y == ramY + 2)
										color = "[color][01,-1]";
									else if (graphChar != ' ')
										color = "[color][--,15]";
									else if (graphChar == ' ')
										color = "[color][--,-1]";

									lineText += color + graphChar;
								}
							}

							if (previousLines[y - 1] != lineText)
							{
								previousLines[y - 1] = lineText;
								text += lineText;
							}
							text += "[color][--,-1]" + Environment.NewLine;
						}
						write(text);
					};
					draw();

					Thread onDraw = new Thread(() =>
					{
						while (exitMonitor == false)
						{
							Console.SetCursorPosition(0, cursorPosition);
							draw();
							Thread.Sleep(300);
						}
					});
					onDraw.Start();

					Thread onUsage = new Thread(() =>
					{
						List<double> beforeCpus = new List<double>();
						List<long> beforeRAMs = new List<long>();

						var usageGraphYMin = 3;
						var usageGraphYMax = ramY;
						var usageGraphXMax = graphWidth;
						long maxRam = 100;
						while (exitMonitor == false)
						{
							try
							{
								var usage = AgentHelper.GetProjectUsage(projectName);
								var cpu = usage.cpuPercent ?? 0;
								var ram = usage.ramUsage ?? 0;
								projectEnable = usage.cpuPercent != null;

								for (int y = 0; y < graphHeight; y++)
									for (int x = 0; x < graphWidth; x++)
										graph[y, x] = ' ';

								for (int x = 0; x < graphWidth; x++)
									graph[ramY, x] = '─';

								for (int i = 0; i < projectName.Length; i++)
								{
									graph[0, i + 1] = projectName[i];
									graph[ramY + 1, i + 1] = projectName[i];
								}

								graph[1, 1] = 'C';
								graph[1, 2] = 'P';
								graph[1, 3] = 'U';

								graph[ramY + 2, 1] = 'R';
								graph[ramY + 2, 2] = 'A';
								graph[ramY + 2, 3] = 'M';

								beforeCpus.Add(cpu);
								beforeRAMs.Add(ram);
								maxRam = Math.Max(maxRam, ram);

								var cpuStr = cpu.ToString();
								for (int i = 0; i < cpuStr.Length; i++)
									graph[1, 5 + i] = cpuStr[i];
								graph[1, 5 + cpuStr.Length] = '%';

								var ramStr = AgentHelper.ByteShort(ram);
								for (int i = 0; i < ramStr.Length; i++)
									graph[2 + ramY, 5 + i] = ramStr[i];

								if (beforeCpus.Count > usageGraphXMax)
								{
									beforeCpus.RemoveAt(0);
									beforeRAMs.RemoveAt(0);
								}

								for (int i = 0; i < beforeCpus.Count; i++)
									graph[Math.Max(3, Math.Min(usageGraphYMax - 1, usageGraphYMax - (int)((beforeCpus[i] * (usageGraphYMax - usageGraphYMin)) / 100))), i] = '0';

								for (int i = 0; i < beforeRAMs.Count; i++)
									graph[ramY + 1 + Math.Max(0, Math.Min(usageGraphYMax, (usageGraphYMax - (int)((beforeRAMs[i] * (usageGraphYMax - usageGraphYMin)) / maxRam)))), i] = '0';

								if (usage.cpuPercent == null)
									Thread.Sleep(300);
							}
							catch { }
						}
					});
					onUsage.Start();

					Console.ReadKey(true);
					exitMonitor = true;
				});
			}));
			commands.Add(new Command("live", "Open the live screen to watch all projects.", () =>
			{
				tryCatch(() =>
				{
					Console.Clear();
					int cursorPosition = Console.CursorTop;
					bool exitMonitor = false;
					int consoleWidth = Console.BufferWidth - (Console.BufferWidth % 2);
					int consoleHeight = Console.BufferHeight - (2 - (Console.BufferHeight % 2));

					int graphWidth = consoleWidth - 2;
					int graphHeight = consoleHeight - 2;
					int maxLength = ConfigHelper.Projects.Max(o => o.Length) + 8;
					if (maxLength == 0)
						throw new Exception("Projects was not found!");

					int?[] cpuIndex = new int?[graphHeight];
					char[,] graph = new char[graphHeight, graphWidth];
					List<string> previousLines = new List<string>();
					for (int i = 0; i < consoleHeight; i++)
						previousLines.Add("");

					for (int y = 0; y < graphHeight; y++)
						for (int x = 0; x < graphWidth; x++)
							graph[y, x] = ' ';

					Action draw = () =>
					{
						string text = "";
						for (int y = 1; y <= consoleHeight; y++)
						{
							string lineText = "";
							for (int x = 1; x <= consoleWidth; x++)
							{
								if (exitMonitor == true)
									return;

								if (x == 1 && y == 1)
									lineText += "[color][01,-1]┌";
								else if (x == consoleWidth && y == consoleHeight)
									lineText += "[color][01,-1]┘";
								else if (x == consoleWidth && y == 1)
									lineText += "[color][01,-1]┐";
								else if (x == 1 && y == consoleHeight)
									lineText += "[color][01,-1]└";
								else if (x == 1 || x == consoleWidth)
									lineText += "[color][01,-1]│";
								else if (y == 1 || y == consoleHeight)
									lineText += "[color][01,-1]─";
								else
								{
									char graphChar = graph[y - 2, x - 2];
									string color = "";
									if (x < maxLength + 2 - 8)
										color = "[color][14,-1]";
									else if (x < maxLength + 2 && cpuIndex[y - 2] == null)
										color = "[color][12,-1]";
									else if (x < maxLength + 2 && cpuIndex[y - 2] != null)
										color = "[color][10,-1]";
									else if (cpuIndex[y - 2] == null)
										color = "[color][08,-1]";
									else if (x < cpuIndex[y - 2] + maxLength + 3)
										color = "[color][15,-1]";
									else
										color = "[color][03,-1]";

									lineText += color + graphChar;
								}
							}

							if (previousLines[y - 1] != lineText)
							{
								previousLines[y - 1] = lineText;
								text += lineText;
							}
							text += "[color][--,-1]" + Environment.NewLine;
						}
						write(text);
					};
					draw();

					Thread onDraw = new Thread(() =>
					{
						while (exitMonitor == false)
						{
							Console.SetCursorPosition(0, cursorPosition);
							draw();
							Thread.Sleep(300);
						}
					});
					onDraw.Start();

					int rowIndex = 0;
					Thread onUsage = new Thread(() =>
					{
						while (exitMonitor == false)
						{
							try
							{
								var processesTotalProcessorTime = new List<(string projectName, Process process, double? totalTicks, DateTime? calcDate)>();
								foreach (var projectName in ConfigHelper.Projects)
								{
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

									processesTotalProcessorTime.Add((projectName: projectName, process: process, totalTicks, calcDate: calcDate));
								}

								Thread.Sleep(300);

								for (int y = 0; y < graphHeight; y++)
									for (int x = 0; x < graphWidth; x++)
										graph[y, x] = ' ';

								var projectIndex = 0;
								foreach (var project in processesTotalProcessorTime)
								{
									if (projectIndex < rowIndex || projectIndex >= rowIndex + graphHeight)
									{
										projectIndex++;
										continue;
									}

									string cpu = project.process == null ? "0%" : (((project.process.TotalProcessorTime.Ticks - project.totalTicks.Value) / (DateTime.Now - project.calcDate.Value).Ticks) * 100).ToString("N2") + "%";
									string ram = AgentHelper.ByteShort(project.process == null ? 0 : project.process.WorkingSet64);
									string projectName = project.projectName.PadRight(maxLength - 8, ' ') + " " + (project.process == null ? "CLOSED" : "OPENED");
									cpuIndex[projectIndex - rowIndex] = project.process == null ? null : (int?)cpu.Length;

									for (int i = 0; i < projectName.Length; i++)
										graph[Math.Min(graphHeight - 1, projectIndex - rowIndex), Math.Min(graphWidth - 1, i)] = projectName[i];

									for (int i = 0; i < cpu.Length; i++)
										graph[Math.Min(graphHeight - 1, projectIndex - rowIndex), Math.Min(graphWidth - 1, i + projectName.Length + 1)] = cpu[i];

									for (int i = 0; i < ram.Length; i++)
										graph[Math.Min(graphHeight - 1, projectIndex - rowIndex), Math.Min(graphWidth - 1, i + cpu.Length + 1 + projectName.Length + 1)] = ram[i];

									projectIndex++;
								}
							}
							catch { }
						}
					});
					onUsage.Start();

					do
					{
						var key = Console.ReadKey(true);
						if (key.Key == ConsoleKey.UpArrow)
							rowIndex = Math.Max(0, rowIndex - 1);
						else if (key.Key == ConsoleKey.DownArrow)
							rowIndex = Math.Min(ConfigHelper.Projects.Length - 1, rowIndex + 1);
						else
							break;
					} while (true);
					exitMonitor = true;
				});
			}));
			commands.Add(new Command("db -c", "db --connect", "Try to connect to database.", () =>
			{
				tryCatch(() =>
				{
					string databaseName = getLine("Write database name = ");
					if (databaseName == "")
						return;

					if (ConfigHelper.Databases.Any(o => o == databaseName) == false)
						throw new Exception("Was not found the database!");

					DatabaseHelper.TryToConnect(databaseName);
					successful();
				});
			}));
			commands.Add(new Command("db -ca", "db --connect-all", "Try to connect to all databases.", () =>
			{
				tryCatch(() =>
				{
					foreach (var databaseName in ConfigHelper.Databases)
					{
						writeLine("[color][09,--]Working on [color][14,--]{0} [color][09,--]database...", databaseName);
						DatabaseHelper.TryToConnect(databaseName);
					}
					successful();
				});
			}));
			commands.Add(new Command("db -r", "db --remove", "Remove all tables from database.", () =>
			{
				tryCatch(() =>
				{
					string databaseName = getLine("Write database name = ");
					if (databaseName == "")
						return;

					if (ConfigHelper.Databases.Any(o => o == databaseName) == false)
						throw new Exception("Was not found the database!");

					string message = getLine("Write the message (I am sure to remove) = ");
					if (message != "I am sure to remove")
					{
						writeLine("[color][15,--]You must be sure!");
						return;
					}

					DatabaseHelper.RemoveAllTables(databaseName);
					successful();
				});
			}));
			commands.Add(new Command("db -ra", "db --remove-all", "Remove all tables from all databases.", () =>
			{
				tryCatch(() =>
				{
					string message = getLine("Write the message (I am sure to remove) = ");
					if (message != "I am sure to remove")
					{
						writeLine("[color][15,--]You must be sure!");
						return;
					}

					foreach (var databaseName in ConfigHelper.Databases)
					{
						writeLine("[color][09,--]Working on [color][14,--]{0} [color][09,--]database...", databaseName);
						DatabaseHelper.RemoveAllTables(databaseName);
					}
					successful();
				});
			}));
			commands.Add(new Command("db -m", "db --migration", "Apply migrations on database.", () =>
			{
				tryCatch(() =>
				{
					string databaseName = getLine("Write database name = ");
					if (databaseName == "")
						return;

					if (ConfigHelper.Databases.Any(o => o == databaseName) == false)
						throw new Exception("Was not found the database!");

					DatabaseHelper.ApplyMigration(databaseName, (migrationName) =>
					{
						writeLine("[color][09,--]Applying migration [color][14,--]{0}[color][09,--]...", migrationName);
					});
					successful();
				});
			}));
			commands.Add(new Command("db -ma", "db --migration-all", "Apply migrations on all databases.", () =>
			{
				tryCatch(() =>
				{
					foreach (var databaseName in ConfigHelper.Databases)
					{
						writeLine("[color][09,--]Working on [color][14,--]{0} [color][09,--]database...", databaseName);
						DatabaseHelper.ApplyMigration(databaseName, (migrationName) =>
						{
							writeLine("[color][09,--]Applying migration [color][14,--]{0}[color][09,--]...", migrationName);
						});
					}
					successful();
				});
			}));
			commands.Add(new Command("pr -s", "pr --status", "Show projects status.", () =>
			{
				tryCatch(() =>
				{
					var processesTotalProcessorTime = new List<(string projectName, Process process, double? totalTicks, DateTime? calcDate)>();
					foreach (var projectName in ConfigHelper.Projects)
					{
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

						processesTotalProcessorTime.Add((projectName: projectName, process: process, totalTicks, calcDate: calcDate));
					}

					Thread.Sleep(300);

					var projectsLog = new List<(string projectName, string text)>();
					foreach (var project in processesTotalProcessorTime)
					{
						projectsLog.Add((
							projectName: project.projectName,
							text: project.process == null ?
										"[color][12,--]CLOSED[color][08,--], [color][15,--]CPU [color][08,--]:[color][11,--]       ?[color][08,--], [color][15,--]RAM [color][08,--]:[color][11,--] ?" :
										"[color][10,--]OPENED[color][08,--], [color][15,--]CPU [color][08,--]:[color][11,--] " + (
													(((project.process.TotalProcessorTime.Ticks - project.totalTicks.Value) / (DateTime.Now - project.calcDate.Value).Ticks)) * 100).ToString("N2").PadLeft(6, ' ') + "%[color][08,--], " +
													"[color][15,--]RAM [color][08,--]: [color][11,--]" + AgentHelper.ByteShort(project.process.WorkingSet64)
							));
					}

					writeLine();

					var maxLength = projectsLog.Max(o => o.projectName.Length);
					foreach (var project in projectsLog)
						writeLine("[color][14,--]" + project.projectName.PadRight(maxLength, ' ') + " [color][08,--]| " + project.text);

					writeLine();

					successful();
				});
			}));

			Action<string> restartProject = (projectName) =>
			{
				if (ConfigHelper.Projects.Any(o => o == projectName) == false)
					throw new Exception("Not found project!");

				var processId = ProcessSaveHelper.Get(projectName);
				if (processId != "")
					ShellHelper.Run("", "kill " + processId, false, false);

				var projectCommands = ConfigHelper.GetProjectCommands(projectName);
				foreach (var command in projectCommands)
					if (command.main == false)
						ShellHelper.Run(
							command.path,
							command.name + " " + command.arguments, true, true, callbackOutput: (process, text) =>
							{
								writeLine(text);
							}, callbackError: (process, text) =>
							{
								writeLine(text);
							});

				ShellHelper.Run("", "dotnet exec " + Path.Combine(CurrentDirectory, "bin/Release/netcoreapp2.1/dmuka2.CS.Deploy.dll") + " --background \"" + JsonConvert.SerializeObject(new
				{
					user_name = ConfigHelper.UserName,
					project_name = projectName
				}).Replace("\"", "\\\"") + "\" --configuration Release &", false, false, useShell: false);

				Thread.Sleep(1000);
			};
			commands.Add(new Command("pr -r", "pr --restart", "Restart project.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");
					if (projectName == "")
						return;

					restartProject(projectName);

					commands.FirstOrDefault(o => o.Name == "pr -s").Action();
				});
			}));
			commands.Add(new Command("pr -ra", "pr --restart-all", "Restart all projects.", () =>
			{
				tryCatch(() =>
				{
					areYouSure(() =>
					{
						foreach (var projectName in ConfigHelper.Projects)
						{
							writeLine("[color][09,--]Restarting [color][14,--]{0} [color][09,--]project...", projectName);
							restartProject(projectName);
						}

						commands.FirstOrDefault(o => o.Name == "pr -s").Action();
					});
				});
			}));
			Action<string> killProject = (projectName) =>
			{
				if (ConfigHelper.Projects.Any(o => o == projectName) == false)
					throw new Exception("Not found project!");

				var processId = ProcessSaveHelper.Get(projectName);
				if (processId == "")
					return;

				ShellHelper.Run("", "kill " + processId, false, false);
			};
			commands.Add(new Command("pr -k", "pr --kill", "Kill project.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");
					if (projectName == "")
						return;

					killProject(projectName);

					Thread.Sleep(1000);
					commands.FirstOrDefault(o => o.Name == "pr -s").Action();
				});
			}));
			commands.Add(new Command("pr -ka", "pr --kill-all", "Kill all projects.", () =>
			{
				tryCatch(() =>
				{
					areYouSure(() =>
					{
						foreach (var projectName in ConfigHelper.Projects)
						{
							writeLine("[color][09,--]Killing [color][14,--]{0} [color][09,--]project...", projectName);
							killProject(projectName);
						}

						Thread.Sleep(1000);
						commands.FirstOrDefault(o => o.Name == "pr -s").Action();
					});
				});
			}));
			#endregion

			#region Checking Args
			var existReturnArg = false;

			// If new background process is exists, it must be on args.
			// We will check it!
			string argCommand = "";
			int argIndex = 0;
			Action<string> runCommand = (commandName) =>
			{
				ConfigHelper.Load();
				__askDisable = true;
				int argumentIndex = argIndex + 1;
				getNewArgLine = () =>
				{
					if (args.Length <= argumentIndex || (args[argumentIndex].Length > 0 && args[argumentIndex][0] == '-'))
						throw new Exception((argumentIndex + 1) + ". argument was not found for " + commandName + " command!");

					string newArgLine = args[argumentIndex];
					argumentIndex++;
					argIndex++;
					return newArgLine;
				};

				var exists = false;
				foreach (var command in commands)
				{
					if (command.Name == commandName || command.LongName == commandName)
					{
						exists = true;
						command.Action();
					}
				}

				if (exists == false)
					writeLine("Command was not found!");
				existReturnArg = true;

				getNewArgLine = () => throw new Exception("Not found enough arguments!");
				argCommand = "";
			};
			Action checkIncorrectArguments = () =>
			{
				if (argCommand != "")
					throw new Exception("Not found " + argCommand + " argument!");
			};

			ConfigHelper.Load();
			for (argIndex = 0; argIndex < args.Length; argIndex++)
			{
				var arg = args[argIndex];
				switch (arg)
				{
					case "--background":
						{
							checkIncorrectArguments();

							// This means that we need a background process.
							var backgroundParameter = JsonConvert.DeserializeObject<JToken>(args[argIndex + 1]);
							var userName = backgroundParameter["user_name"].Value<string>();
							var projectName = backgroundParameter["project_name"].Value<string>();

							ConfigHelper.SetUserName(userName);
							var mainProcess = ConfigHelper.GetProjectCommands(projectName).Where(o => o.main).FirstOrDefault();

							var previousProcessId = ProcessSaveHelper.Get(projectName);
							if (previousProcessId != "")
								ShellHelper.Run(
									"",
									"kill " + previousProcessId,
									false,
									true);

							AgentHelper.StartTheQueue();
							AgentHelper.AddToQueue(() =>
							{
								var text = "Project is being opened...";
								AgentHelper.OnLog(projectName, false, text);
								LogHelper.Write(projectName, text);
							});

							try
							{
								ShellHelper.Run(
									mainProcess.path,
									mainProcess.name + " " + mainProcess.arguments, true, true, callbackOutput: (process, text) =>
									{
										AgentHelper.AddToQueue(() =>
										{
											AgentHelper.OnLog(projectName, false, text);
										});
										LogHelper.Write(projectName, text);
									}, callbackError: (process, text) =>
									{
										AgentHelper.AddToQueue(() =>
										{
											AgentHelper.OnLog(projectName, true, text);
										});
										LogHelper.Write(projectName, text);
									}, useShell: false, callbackStarted: (process) =>
									{
										var text = "Project has just been opened.";
										ProcessSaveHelper.Set(projectName, process.Id.ToString());

										AgentHelper.AddToQueue(() =>
										{
											AgentHelper.OnLog(projectName, false, text);
										});
										AgentHelper.AddToQueue(() =>
										{
											AgentHelper.OnProcessStart(projectName);
										});
										LogHelper.Write(projectName, text);
									});
							}
							catch (System.Exception ex)
							{
								var text = ex.ToString();
								AgentHelper.OnLog(projectName, true, text);
								LogHelper.Write(projectName, text);
							}

							existReturnArg = true;
							AgentHelper.StopTheQueue();

							throw new Exception("Process was finished!");
						}
					case "--current-directory":
					case "-d":
						{
							checkIncorrectArguments();

							CurrentDirectory = args[argIndex + 1];
							argIndex++;
							ConfigHelper.Load(true);
						}
						break;
					case "--help":
						{
							checkIncorrectArguments();

							commands.First(o => o.Name == "help").Action();
							existReturnArg = true;
						}
						break;
					default:
						{
							while (argIndex < args.Length)
							{
								if (argCommand != "")
									argCommand += " ";

								argCommand += args[argIndex];
								if (commands.Any(o => o.Name == argCommand || o.LongName == argCommand))
								{
									runCommand(argCommand);
									break;
								}
								argIndex++;
							}
						}
						break;
				}
			}
			checkIncorrectArguments();

			Console.ForegroundColor = (ConsoleColor)(-1);
			Console.BackgroundColor = (ConsoleColor)(-1);
			if (existReturnArg)
				return;

			ConfigHelper.Load();
			#endregion

			Console.CancelKeyPress += (sender, e) =>
			{
				exit = true;
				__askDisable = true;
				__writeDisable = true;
			};

			writeLine(@"
  ___  __  __ _   _ _  __   _     ___           _          
 |   \|  \/  | | | | |/ /  /_\   |   \ ___ _ __| |___ _  _ 
 | |) | |\/| | |_| | ' <  / _ \  | |) / -_) '_ \ / _ \ || |
 |___/|_|  |_|\___/|_|\_\/_/ \_\ |___/\___| .__/_\___/\_, |
                                          |_|         |__/ 
[line][01]
 [color][15,--]Version " + Assembly.GetExecutingAssembly().GetName().Version + @"
[line][01]
 [color][15,--]Welcome, if you are here, you want a thing from me?
 So, you can learn what can you do with help command.
[line][02]");


			while (exit == false)
			{
				string commandName = getLine("Write command = ");
				if (exit)
					break;

				if (commandName != "")
				{
					var exists = false;
					foreach (var command in commands)
					{
						if (command.Name == commandName || command.LongName == commandName)
						{
							exists = true;
							command.Action();
						}
					}

					if (exists == false)
						writeLine("Not found {0} command!", commandName);
				}

				if (exit == false)
					writeLine("[line][03]");
			}

			byeBye();
		}
	}
}
