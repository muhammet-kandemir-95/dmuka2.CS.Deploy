using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace dmuka2.CS.Deploy
{
    class Program
    {
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
                Console.WriteLine("We couldn't. Do you want see the error? (y/n) ");
                if (Console.ReadLine().ToLower() == "y")
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine("Enter a line to continue...");
                    Console.ReadLine();
                }

                return false;
            }
        }

        static void areYouSure(Action action)
        {
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
        #endregion

        static void Main(string[] args)
        {
            #region Checking Args
            // If new background process is exists, it must be on args.
            // We will check it!
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                switch (arg)
                {
                    // We did it!
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
                                mainProcess.name + " " + mainProcess.arguments + @" &" + Environment.NewLine +
                                @"echo """ + splitText + @"""$!""" + splitText + @"""", true, true, callbackOutput: (process, text) =>
                                {
                                    if (processId == "" && text.Contains(splitText))
                                    {
                                        processId = text.Split(new string[] { splitText }, StringSplitOptions.None)[1];
                                        ProcessSaveHelper.Set(projectName, processId);
                                    }
                                    else
                                        LogHelper.Write(projectName, text);
                                });

                            return;
                        }
                    default:
                        break;
                }
            }
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

            ConfigHelper.SetUserName("");

            List<Command> commands = new List<Command>();
            commands.Add(new Command("help", "", () =>
            {
                var maxLength = commands.Max(o => o.Name.Length);

                foreach (var command in commands)
                    if (command.Name != "help")
                        Console.WriteLine(command.Name.PadRight(maxLength, ' ') + " = " + command.Description);
            }));
            commands.Add(new Command("set -u", "Set user name.", () =>
            {
                Console.WriteLine("Write user name = ");
                ConfigHelper.SetUserName(Console.ReadLine());
            }));
            commands.Add(new Command("db -c", "Try to connect to database.", () =>
            {
                tryCatch(() =>
                {
                    Console.WriteLine("Write database name = ");
                    string databaseName = Console.ReadLine();

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
                    Console.WriteLine("Write database name = ");
                    string databaseName = Console.ReadLine();

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
                    Console.WriteLine("Write database name = ");
                    string databaseName = Console.ReadLine();

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

                ShellHelper.Run("", "dotnet run --background \"" + JsonConvert.SerializeObject(new
                {
                    user_name = ConfigHelper.UserName,
                    project_name = projectName
                }).Replace("\"", "\\\"") + "\" --configuration RELEASE", false, false);
            };
            commands.Add(new Command("pr -r", "Restart project.", () =>
            {
                tryCatch(() =>
                {
                    Console.WriteLine("Write project name = ");
                    string projectName = Console.ReadLine();

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
                    });

                    successful();
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
                    Console.WriteLine("Write project name = ");
                    string projectName = Console.ReadLine();

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
                    });

                    successful();
                });
            }));

            while (true)
            {
                Console.WriteLine("Write command = ");
                string commandName = Console.ReadLine();

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
        }
    }
}
