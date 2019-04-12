using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
		/// Current Directory
		/// </summary>
		internal static string CurrentDirectory = "";
		#endregion

		#region Methods
		static void write(string text, params object[] arguments)
		{
			if (__writeDisable == true)
				return;

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
				Console.Write(item, arguments);
			}
			Console.ForegroundColor = previousForeColor;
			Console.BackgroundColor = previousBackColor;
		}

		static void writeLine(string text, params object[] arguments)
		{
			write(text + Environment.NewLine, arguments);
		}

		static void writeLine()
		{
			write(Environment.NewLine);
		}

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

		static void successful()
		{
			writeLine(@"[color][10,--]Successfull");
		}

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

			string argLine = "";
			Func<string, string> getLine = (msg) =>
			{
				if (__askDisable == true)
				{
					return argLine;
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
				var maxLength = commands.Max(o => o.Name.Length);

				foreach (var command in commands)
					if (command.Name != "help")
						writeLine("[color][11,--]" + command.Name.PadRight(maxLength, ' ') + " [color][08,--]= [color][15,--]" + command.Description);
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

			string deployShFilePath = Path.Combine(CurrentDirectory, "deploy.sh");

			commands.Add(new Command("add -s", "Add deploy.sh to startup by linux user name.", () =>
			{
				tryCatch(() =>
				{
					string linuxUserName = getLine("Write linux user name = ");
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
			commands.Add(new Command("remove -s", "Remove deploy.sh from startup by linux user name.", () =>
			{
				tryCatch(() =>
				{
					string linuxUserName = getLine("Write linux user name = ");
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
			commands.Add(new Command("add -a", "Add alias to bash as 'depmk' via current directory.", () =>
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
			commands.Add(new Command("remove -a", "Remove alias from .bashrc.", () =>
			{
				tryCatch(() =>
				{
					string bashrc = string.Join(Environment.NewLine, File.ReadAllText(bashrcFilePath).Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Where(o => o.Contains("alias depmk") == false));

					File.WriteAllText(bashrcFilePath, bashrc);

					successful();
				});
			}));
			commands.Add(new Command("sleep -s", "Thread sleep as second.", () =>
			{
				tryCatch(() =>
				{
					var second = Convert.ToInt32(getLine("Write second = "));
					writeLine("[color][13,--]Waiting {0} second...", second);
					Thread.Sleep(second * 1000);
					successful();
				});
			}));
			commands.Add(new Command("sleep -m", "Thread sleep as minute.", () =>
			{
				tryCatch(() =>
				{
					var minute = Convert.ToInt32(getLine("Write minute = "));
					writeLine("[color][13,--]Waiting {0} minute...", minute);
					Thread.Sleep(minute * 1000 * 60);
					successful();
				});
			}));
			commands.Add(new Command("sleep -h", "Thread sleep as hour.", () =>
			{
				tryCatch(() =>
				{
					var hour = Convert.ToInt32(getLine("Write hour = "));
					writeLine("[color][13,--]Waiting {0} hour...", hour);
					Thread.Sleep(hour * 1000 * 60 * 60);
					successful();
				});
			}));
			commands.Add(new Command("set -u", "Set user name.", () =>
			{
				ConfigHelper.SetUserName(getLine("Write user name = "));
				successful();
			}));
			commands.Add(new Command("get -u", "Get user name.", () =>
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
			commands.Add(new Command("alog -a", "Show agent log of all projects.", () =>
			{
				runAgentLogProcesses(ConfigHelper.Projects);
			}));
			commands.Add(new Command("alog", "Show agent log of project/projects.", () =>
			{
				tryCatch(() =>
				{
					var logs = getLine("Write project/projects name(You can use ';' for multiple project) = ").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToArray();

					if (logs.Length == 0 || logs.Length != ConfigHelper.Projects.Where(o => logs.Any(a => a == o)).Count())
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
			commands.Add(new Command("log -a", "Show all projects log.", () =>
			{
				runLogProcesses(ConfigHelper.Projects);
			}));
			commands.Add(new Command("log", "Show log of project/projects.", () =>
			{
				tryCatch(() =>
				{
					var logs = getLine("Write project/projects name(You can use ';' for multiple project) = ").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToArray();

					if (logs.Length == 0 || logs.Length != ConfigHelper.Projects.Where(o => logs.Any(a => a == o)).Count())
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
			commands.Add(new Command("log -r", "Remove all logs of a project.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");
					deleteLog(projectName);
					successful();
				});
			}));
			commands.Add(new Command("log -ra", "Remove logs of all projects.", () =>
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
			commands.Add(new Command("mon", "Open the monitor to watch all project.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");
					if (ConfigHelper.Projects.Any(o => o == projectName) == false)
						throw new Exception("Not found project!");

					Console.Clear();
					int cursorPosition = Console.CursorTop;
					bool exitMonitor = false;
					int consoleWidth = Console.BufferWidth - (Console.BufferWidth % 2);
					int consoleHeight = Console.BufferHeight - (2 - (Console.BufferWidth % 2));

					int graphWidth = consoleWidth - 2;
					int graphHeight = consoleHeight - 2;

					char[,] graph = new char[graphHeight, graphWidth];
					for (int y = 0; y < graphHeight; y++)
						for (int x = 0; x < graphWidth; x++)
							graph[y, x] = ' ';

					Action draw = () =>
					{
						for (int y = 1; y <= consoleHeight; y++)
						{
							for (int x = 1; x <= consoleWidth; x++)
							{
								if (exitMonitor == true)
									return;

								if (x == 1 && y == 1)
									write("[color][01,--]┌");
								else if (x == consoleWidth && y == consoleHeight)
									write("[color][01,--]┘");
								else if (x == consoleWidth && y == 1)
									write("[color][01,--]┐");
								else if (x == 1 && y == consoleHeight)
									write("[color][01,--]└");
								else if (x == 1 || x == consoleWidth)
									write("[color][01,--]│");
								else if (y == 1 || y == consoleHeight)
									write("[color][01,--]─");
								else
								{
									char graphChar = graph[y - 2, x - 2];
									string color = "";
									if (y == 2 || y == consoleHeight / 2 + 1)
										color = "[color][14,--]";
									else if (y == 3 || y == consoleHeight / 2 + 2)
									{
										if (x < 6)
											color = "[color][07,--]";
										else
											color = "[color][11,--]";
									}
									else if (y == consoleHeight / 2)
										color = "[color][01,--]";
									else if (graphChar != ' ')
										color = "[color][--,15]";

									write(color + graphChar);
								}
							}
							writeLine();
						}
					};
					draw();

					Thread onDraw = new Thread(() =>
					{
						while (exitMonitor == false)
						{
							Console.SetCursorPosition(0, cursorPosition);
							draw();
							Thread.Sleep(600);
						}
					});
					onDraw.Start();

					Thread onUsage = new Thread(() =>
					{
						List<double> beforeCpus = new List<double>();
						List<long> beforeRAMs = new List<long>();
						var ramY = graphHeight / 2;

						var usageGraphYMin = 3;
						var usageGraphYMax = ramY - 2;
						var usageGraphXMax = graphWidth;
						long maxRam = 100;
						while (exitMonitor == false)
						{
							var usage = AgentHelper.GetProjectUsage(projectName);
							var cpu = usage.cpuPercent ?? 0;
							var ram = usage.ramUsage ?? 0;

							for (int y = 0; y < graphHeight; y++)
								for (int x = 0; x < graphWidth; x++)
									graph[y, x] = ' ';

							for (int x = 0; x < graphWidth; x++)
								graph[ramY - 1, x] = '─';

							for (int i = 0; i < projectName.Length; i++)
							{
								graph[0, i + 1] = projectName[i];
								graph[ramY, i + 1] = projectName[i];
							}

							graph[1, 1] = 'C';
							graph[1, 2] = 'P';
							graph[1, 3] = 'U';

							graph[ramY + 1, 1] = 'R';
							graph[ramY + 1, 2] = 'A';
							graph[ramY + 1, 3] = 'M';

							beforeCpus.Add(cpu);
							beforeRAMs.Add(ram);
							maxRam = Math.Max(maxRam, ram);

							var cpuStr = cpu.ToString();
							for (int i = 0; i < cpuStr.Length; i++)
								graph[1, 5 + i] = cpuStr[i];
							graph[1, 5 + cpuStr.Length] = '%';

							var ramStr = AgentHelper.ByteShort(ram);
							for (int i = 0; i < ramStr.Length; i++)
								graph[1 + ramY, 5 + i] = ramStr[i];

							if (beforeCpus.Count > usageGraphXMax)
							{
								beforeCpus.RemoveAt(0);
								beforeRAMs.RemoveAt(0);
							}

							for (int i = 0; i < beforeCpus.Count; i++)
								graph[Math.Max(3, Math.Min(usageGraphYMax - 1, usageGraphYMax - (int)((beforeCpus[i] * (usageGraphYMax - usageGraphYMin)) / 100))), i] = '0';

							for (int i = 0; i < beforeRAMs.Count; i++)
								graph[ramY + Math.Max(0, Math.Min(usageGraphYMax - 1, (usageGraphYMax - (int)((beforeRAMs[i] * (usageGraphYMax - usageGraphYMin)) / maxRam)))), i] = '0';

							Thread.Sleep(300);
						}
					});
					onUsage.Start();

					Console.ReadKey(true);
					exitMonitor = true;
				});
			}));
			commands.Add(new Command("db -c", "Try to connect to database.", () =>
			{
				tryCatch(() =>
				{
					string databaseName = getLine("Write database name = ");

					DatabaseHelper.TryToConnect(databaseName);
					successful();
				});
			}));
			commands.Add(new Command("db -ca", "Try to connect to all databases.", () =>
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
			commands.Add(new Command("db -r", "Remove all tables from database.", () =>
			{
				tryCatch(() =>
				{
					string databaseName = getLine("Write database name = ");

					DatabaseHelper.RemoveAllTables(databaseName);
					successful();
				});
			}));
			commands.Add(new Command("db -ra", "Remove all tables from all databases.", () =>
			{
				tryCatch(() =>
				{
					areYouSure(() =>
					{
						foreach (var databaseName in ConfigHelper.Databases)
						{
							writeLine("[color][09,--]Working on [color][14,--]{0} [color][09,--]database...", databaseName);
							DatabaseHelper.RemoveAllTables(databaseName);
						}
						successful();
					});
				});
			}));
			commands.Add(new Command("db -m", "Apply migrations on database.", () =>
			{
				tryCatch(() =>
				{
					string databaseName = getLine("Write database name = ");

					DatabaseHelper.ApplyMigration(databaseName, (migrationName) =>
					{
						writeLine("[color][09,--]Applying migration [color][14,--]{0}[color][09,--]...", migrationName);
					});
					successful();
				});
			}));
			commands.Add(new Command("db -ma", "Apply migrations on all databases.", () =>
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
			commands.Add(new Command("pr -s", "Show projects status.", () =>
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

				var processId = ProcessSaveHelper.Get(projectName);
				if (processId != "")
					ShellHelper.Run("", "kill " + processId, false, false);

				ShellHelper.Run("", "dotnet run --background \"" + JsonConvert.SerializeObject(new
				{
					user_name = ConfigHelper.UserName,
					project_name = projectName
				}).Replace("\"", "\\\"") + "\" --configuration Release &", false, false, useShell: false);


				Thread.Sleep(1000);
			};
			commands.Add(new Command("pr -r", "Restart project.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");

					restartProject(projectName);

					successful();
				});
			}));
			commands.Add(new Command("pr -ra", "Restart all projects.", () =>
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

						successful();
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
			commands.Add(new Command("pr -k", "Kill project.", () =>
			{
				tryCatch(() =>
				{
					string projectName = getLine("Write project name = ");

					killProject(projectName);

					successful();
				});
			}));
			commands.Add(new Command("pr -ka", "Kill all projects.", () =>
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

						successful();
					});
				});
			}));
			#endregion

			#region Checking Args
			var existReturnArg = false;

			// If new background process is exists, it must be on args.
			// We will check it!
			for (int i = 0; i < args.Length; i++)
			{
				var arg = args[i];
				switch (arg)
				{
					case "--cmd":
					case "-c":
						{
							ConfigHelper.Load();
							__askDisable = true;
							argLine = i + 2 < args.Length ? args[i + 2] : "";

							var exists = false;
							foreach (var command in commands)
							{
								if (command.Name == args[i + 1])
								{
									exists = true;
									command.Action();
								}
							}

							if (exists == false)
								writeLine("Command not found!");
							existReturnArg = true;
						}
						break;
					case "--background":
						{
							ConfigHelper.Load();
							// This means that we need a background process.
							var backgroundParameter = JsonConvert.DeserializeObject<JToken>(args[i + 1]);
							var userName = backgroundParameter["user_name"].Value<string>();
							var projectName = backgroundParameter["project_name"].Value<string>();

							ConfigHelper.SetUserName(userName);
							var mainProcess = ConfigHelper.GetProjectCommands(projectName).Where(o => o.main).FirstOrDefault();

							string splitText = "~_/\\_~";
							string processId = "";

							var previousProcessId = ProcessSaveHelper.Get(projectName);
							if (previousProcessId != "")
								ShellHelper.Run(
									"",
									"kill " + previousProcessId,
									false,
									true);

							AgentHelper.StartTheQueue();

							ShellHelper.Run(
								mainProcess.path,
								mainProcess.name + " " + mainProcess.arguments + @" & " +
								@"echo """ + splitText + @"""$!""" + splitText + @"""", true, true, callbackOutput: (process, text) =>
								{
									if (processId == "" && text.Contains(splitText))
									{
										processId = text.Split(new string[] { splitText }, StringSplitOptions.None)[1];
										ProcessSaveHelper.Set(projectName, processId);

										AgentHelper.AddToQueue(() =>
										{
											AgentHelper.OnProcessStart(projectName);
										});
									}
									else
									{
										AgentHelper.AddToQueue(() =>
										{
											AgentHelper.OnLog(projectName, false, text);
										});
										LogHelper.Write(projectName, text);
									}
								}, callbackError: (process, text) =>
								{
									AgentHelper.AddToQueue(() =>
									{
										AgentHelper.OnLog(projectName, true, text);
									});
									LogHelper.Write(projectName, text);
								});

							existReturnArg = true;
							AgentHelper.StopTheQueue();

							throw new Exception("Process was finished!");
						}
					case "--current-directory":
					case "-d":
						{
							CurrentDirectory = args[i + 1];
							ConfigHelper.Load();
						}
						break;
					default:
						break;
				}
			}

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
				commands.FirstOrDefault(o => o.Name == "pr -ka").Action();
			};

			writeLine(@"
  ___  __  __ _   _ _  __   _     ___           _          
 |   \|  \/  | | | | |/ /  /_\   |   \ ___ _ __| |___ _  _ 
 | |) | |\/| | |_| | ' <  / _ \  | |) / -_) '_ \ / _ \ || |
 |___/|_|  |_|\___/|_|\_\/_/ \_\ |___/\___| .__/_\___/\_, |
                                          |_|         |__/ 
[line][01]
 [color][15,--]Version 1.0.0.0
[line][01]
 [color][15,--]Welcome, if you are here, you want a thing from me?
 So, you can learn what can you do with help command.
[line][02]");


			while (exit == false)
			{
				string commandName = getLine("Write command = ");
				if (exit)
					break;

				var exists = false;
				foreach (var command in commands)
				{
					if (command.Name == commandName)
					{
						exists = true;
						command.Action();
					}
				}

				if (exists == false)
					writeLine("Not found {0} command!", commandName);

				if (exit == false)
					writeLine("[line][03]");
			}

			byeBye();
		}
	}
}
