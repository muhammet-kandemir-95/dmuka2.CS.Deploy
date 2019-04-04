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
        /// We must check that Has bye bye been typed?
        /// </summary>
        static bool __byeByeEnable = true;
        #endregion

        #region Methods
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
                    Console.WriteLine("We couldn't. Do you want see the error? (y/n) ");

                if (__askDisable == true || Console.ReadLine().ToLower() == "y")
                {
                    Console.WriteLine(ex.ToString());
                    if (__askDisable == false)
                    {
                        Console.WriteLine("Enter a line to continue...");
                        Console.ReadLine();
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

            Console.WriteLine("Are you sure? (y/n) ");
            if (Console.ReadLine().ToLower() == "y")
                action();
        }

        static void successful()
        {
            Console.WriteLine(@"  ____  _ __ __ ___ ______
 (_-< || / _/ _/ -_|_-<_-<
 /__/\_,_\__\__\___/__/__/
                          ");
        }

        static void byeBye()
        {
            if (__byeByeEnable == false)
                return;
            __byeByeEnable = false;

            Console.WriteLine(@"
    ____                ____           
   / __ )__  _____     / __ )__  _____ 
  / __  / / / / _ \   / __  / / / / _ \
 / /_/ / /_/ /  __/  / /_/ / /_/ /  __/
/_____/\__, /\___/  /_____/\__, /\___/ 
      /____/              /____/       ");
        }
        #endregion

        static void Main(string[] args)
        {
            ConfigHelper.SetUserName("");

            string argLine = "";
            Func<string, string> getLine = (msg) =>
            {
                if (__askDisable == true)
                {
                    return argLine;
                }

                Console.WriteLine(msg);
                return Console.ReadLine();
            };

            bool exit = false;
            Console.CancelKeyPress += (sender, e) =>
            {
                exit = true;
                byeBye();
            };

            #region Commands
            List<Command> commands = new List<Command>();
            commands.Add(new Command("help", "", () =>
            {
                var maxLength = commands.Max(o => o.Name.Length);

                foreach (var command in commands)
                    if (command.Name != "help")
                        Console.WriteLine(command.Name.PadRight(maxLength, ' ') + " = " + command.Description);
            }));
            commands.Add(new Command("exit", "Close this application safely.", () =>
            {
                exit = true;
            }));

            string deployShFilePath = Path.Combine(Directory.GetCurrentDirectory(), "deploy.sh");

            commands.Add(new Command("add -s", "Add deploy.sh to startup.", () =>
            {
                string linuxUserName = getLine("Write linux user name = ");
                string deployStartupCommand =
                        Environment.NewLine +
                        "@reboot " + linuxUserName + " " + deployShFilePath +
                        Environment.NewLine;

                File.WriteAllText(deployShFilePath,
                    "cd " + Directory.GetCurrentDirectory() +
                    Environment.NewLine +
                    @"dotnet run --cmd ""pr -ra""");

                ShellHelper.Run(
                    "",
                    "chmod +x " + deployShFilePath,
                    true,
                    true,
                    (process, text) =>
                    {
                        Console.WriteLine(text);
                    }, callbackError: (process, text) =>
                    {
                        Console.WriteLine(text);
                    });
                ShellHelper.Run(
                    "",
                    "chown " + linuxUserName + " " + deployShFilePath,
                    true,
                    true,
                    (process, text) =>
                    {
                        Console.WriteLine(text);
                    }, callbackError: (process, text) =>
                    {
                        Console.WriteLine(text);
                    });
                string crontabContent = File.ReadAllText("/etc/crontab");

                crontabContent = crontabContent.Replace(deployStartupCommand, "");
                crontabContent += deployStartupCommand;

                File.WriteAllText("/etc/crontab", crontabContent);
            }));
            commands.Add(new Command("remove -s", "Remove deploy.sh from startup.", () =>
            {
                string linuxUserName = getLine("Write linux user name = ");
                string deployStartupCommand =
                        Environment.NewLine +
                        "@reboot " + linuxUserName + " " + deployShFilePath +
                        Environment.NewLine;

                string crontabContent = File.ReadAllText("/etc/crontab");

                crontabContent = crontabContent.Replace(deployStartupCommand, "");

                File.WriteAllText("/etc/crontab", crontabContent);
            }));
            commands.Add(new Command("sleep -s", "Thread sleep as second.", () =>
            {
                tryCatch(() =>
                {
                    var second = Convert.ToInt32(getLine("Write second = "));
                    Console.WriteLine("Waiting {0} second...", second);
                    Thread.Sleep(second * 1000);
                });
            }));
            commands.Add(new Command("sleep -m", "Thread sleep as minute.", () =>
            {
                tryCatch(() =>
                {
                    var minute = Convert.ToInt32(getLine("Write minute = "));
                    Console.WriteLine("Waiting {0} minute...", minute);
                    Thread.Sleep(minute * 1000 * 60);
                });
            }));
            commands.Add(new Command("sleep -h", "Thread sleep as hour.", () =>
            {
                tryCatch(() =>
                {
                    var hour = Convert.ToInt32(getLine("Write hour = "));
                    Console.WriteLine("Waiting {0} hour...", hour);
                    Thread.Sleep(hour * 1000 * 60 * 60);
                });
            }));
            commands.Add(new Command("set -u", "Set user name.", () =>
            {
                ConfigHelper.SetUserName(getLine("Write user name = "));
            }));
            List<Process> logProcesses = new List<Process>();
            Console.CancelKeyPress += (sender, e) =>
            {
                foreach (var logProcess in logProcesses)
                {
                    try
                    {
                        logProcess.StandardInput.Close();
                        logProcess.Kill();
                    }
                    catch { }
                }
                logProcesses.Clear();
            };
            Action<string[]> runLogProcesses = (projects) =>
            {
                // We don't work on background.
                // It is dangerous!
                if (__askDisable)
                    return;

                var maxLength = projects.Max(o => o.Length);
                foreach (var projectName in projects)
                {
                    var logProjectPath = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                    var logProjectDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Log", projectName);

                    if (File.Exists(Path.Combine(logProjectDirectoryPath, logProjectPath)) == false)
                        continue;

                    var shellProcess = ShellHelper.Run(logProjectDirectoryPath, "tail -n 100 -f \"" + logProjectPath + "\"", true, false,
                        (process, text) =>
                        {
                            Console.WriteLine(projectName.PadRight(maxLength + 2, ' ') + "|" + text);
                        }, (process, text) =>
                        {
                            Console.WriteLine(projectName.PadRight(maxLength + 2, ' ') + "|" + text);
                        });

                    logProcesses.Add(shellProcess);
                }

                while (true)
                    Thread.Sleep(1);
            };
            commands.Add(new Command("log -a", "Show all projects log.", () =>
            {
                runLogProcesses(ConfigHelper.Projects);
            }));
            commands.Add(new Command("log", "Show log of project/projects.", () =>
            {
                var logs = getLine("Write project/projects name(You can use ';' for multiple project) = ").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(o => o.Trim()).ToArray();

                if (logs.Length != ConfigHelper.Projects.Where(o => logs.Any(a => a == o)).Count())
                {
                    Console.WriteLine("Not found a project in command!");
                    return;
                }

                runLogProcesses(logs);
            }));
            commands.Add(new Command("log -r", "Remove all logs of a project.", () =>
            {
                tryCatch(() =>
                {
                    string projectName = getLine("Write project name = ");
                    Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Log", projectName), true);
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
                            Directory.Delete(Path.Combine(Directory.GetCurrentDirectory(), "Log", projectName), true);
                        successful();
                    });
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
                        Console.WriteLine("Working on {0} database...", databaseName);
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
                            Console.WriteLine("Working on {0} database...", databaseName);
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
                        Console.WriteLine("Applying migration {0}...", migrationName);
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
                        Console.WriteLine("Working on {0} database...", databaseName);
                        DatabaseHelper.ApplyMigration(databaseName, (migrationName) =>
                        {
                            Console.WriteLine("Applying migration {0}...", migrationName);
                        });
                    }
                    successful();
                });
            }));
            commands.Add(new Command("pr -s", "Show projects status.", () =>
            {
                tryCatch(() =>
                {
                    List<(string name, string status)> statusList = new List<(string name, string status)>();
                    foreach (var project in ConfigHelper.Projects)
                    {
                        bool open = false;
                        try
                        {
                            open = Process.GetProcessById(Convert.ToInt32(ProcessSaveHelper.Get(project))).HasExited == false;
                        }
                        catch { }

                        statusList.Add((name: project, status: open ? "OPENED" : "CLOSED"));
                    }

                    var maxLength = statusList.Max(o => o.name.Length);
                    foreach (var status in statusList)
                        Console.WriteLine(status.name.PadRight(maxLength, ' ') + " = " + status.status);

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
                                Console.WriteLine(text);
                            }, callbackError: (process, text) =>
                            {
                                Console.WriteLine(text);
                            });

                ShellHelper.Run("", "dotnet run --background \"" + JsonConvert.SerializeObject(new
                {
                    user_name = ConfigHelper.UserName,
                    project_name = projectName
                }).Replace("\"", "\\\"") + "\" --configuration RELEASE &", false, false);

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
                            Console.WriteLine("Restarting {0} project...", projectName);
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
                            Console.WriteLine("Killing {0} project...", projectName);
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
                        {
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
                                Console.WriteLine("Command not found!");
                            existReturnArg = true;
                        }
                        break;
                    case "--background":
                        {
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

                            ShellHelper.Run(
                                mainProcess.path,
                                mainProcess.name + " " + mainProcess.arguments + @" & " +
                                @"echo """ + splitText + @"""$!""" + splitText + @"""", true, true, callbackOutput: (process, text) =>
                                {
                                    if (processId == "" && text.Contains(splitText))
                                    {
                                        processId = text.Split(new string[] { splitText }, StringSplitOptions.None)[1];
                                        ProcessSaveHelper.Set(projectName, processId);
                                    }
                                    else
                                        LogHelper.Write(projectName, text);
                                }, callbackError: (process, text) =>
                                {
                                    LogHelper.Write(projectName, text);
                                });

                            existReturnArg = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (existReturnArg)
                return;
            #endregion

            Console.WriteLine(@"
  ___  __  __ _   _ _  __   _     ___           _          
 |   \|  \/  | | | | |/ /  /_\   |   \ ___ _ __| |___ _  _ 
 | |) | |\/| | |_| | ' <  / _ \  | |) / -_) '_ \ / _ \ || |
 |___/|_|  |_|\___/|_|\_\/_/ \_\ |___/\___| .__/_\___/\_, |
                                          |_|         |__/ 

-----------------------------------------------------------
Welcome, if you are here, you want a thing from me?
So, you can learn what can you do with help command.
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            while (exit == false)
            {
                Console.WriteLine("Write command = ");
                string commandName = Console.ReadLine();
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
                    Console.WriteLine("Not found {0} command!", commandName);

                Console.WriteLine("***********************************************************");
            }

            byeBye();
        }
    }
}