<div align="center">
  <br/>
  <img src="https://raw.githubusercontent.com/muhammet-kandemir-95/dmuka2.CS.Deploy/master/mdcontent/images/main.png" />
  <br/>
  <img width="126px" src="https://raw.githubusercontent.com/muhammet-kandemir-95/dmuka2.CS.Deploy/master/mdcontent/images/version.png" alt="version" />
  <br/>
  <h2>VERSION 1.0.0.13</h2>
</div>

> **Version Schema**
> 
> "_To Change the Algorithms of Programs_"."_To Fix Important Bug_"."_To Add New Command_"."_To Fix Bug or To Improve Some Features_"

## What is DEPMK?
 This application helps you to manage your applications working on your linux os. Also, you can see text log which is given by your applications. Thus, when you connect to your linux system via ssh or other ways, you can get what happened on your applications. 

 When you want to see how much cpu and ram your applications are using, default codes write it as agent log to text files. But you can use it to other ui systems(We haven't made it yet) like pm2+ with source code. It is very simple.
 
 Our programs sometimes do not work as well as planned and we need to see what they are doing at that moment and what causes it. This project ensures live cpu and ram datas via bash terminal with colors to help you understand easily. You don't need any tools to use it.
 
  You can manage your migrations of databases(Postgres, MySql, SQL Server) using this application. Also, if you want to check that _Is database working?_ or _Can I connect to the database?_, you can these with **db** commands.

## How do I install?

1. Open the project directory.
2. Run shell script -> **sh install.sh**

<div>
  <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/install.gif?raw=true" />
</div>

## How do I uninstall?

1. Open the project directory.
2. Run bash script -> **sh uninstall.sh**

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/uninstall.gif?raw=true" />
</div>

## Where is the help?

1. Run bash script -> **depmk help** or **depmk --help**

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/help.gif?raw=true" />
  <br />
  <br />
</div>

> Help will provide you the commands in the project. It will also produce 3 example scenarios to help you. If you still have problems, look at "**What are the commands?**" section for further examples and explanations.

## What is the project ui? How can I see it?

 First, you need to run bash script "**depmk**" after installation. When youdo it, you will see our ui designer. I can hear you asking "_Why will I use it?_" Reason of this is to catch more attention and provide more details.

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/ui.gif?raw=true" />
</div>

## What are the commands in this project?

**This table has the commands list, but you can also see the details of the commands below.**

|Command|Short Description|
|---|---|
|_add -p_ , _add --project_|Add a new project to config.|
|_add -s_ , _add --startup_|Add deploy.sh to startup by linux user name.|
|_add -a_ , _add --alias_|Add alias to bash as 'depmk' via current directory.|
|_alog -sa_ , _alos --show-all_|Show agent log of all projects.|
|_alog -s_ , _alog --show_|Show agent log of project/projects.|
|_clear_|Clear console.|
|_cwd_|Write current directory.|
|_db -c_ , _db --connect_|Try to connect to database.|
|_db -ca_ , _db --connect-all_|Try to connect to all databases.|
|_db -r_ , _db --remove_|Remove all tables from database.|
|_db -ra_ , _db --remove-all_|Remove all tables from all databases.|
|_db -m_ , _db --migration_|Apply migrations on database.|
|_db -ma_ , _db --migration-all_|Apply migrations on all databases.|
|_del -p_ , _del --project_|Delete a project from config.|
|_del -s_ , _del --startup_|Delete deploy.sh from startup by linux user name.|
|_del -a_ , _del --alias_|Delete alias from .bashrc.|
|_exit_|Close this application safely.|
|_get -u_ , _get --user_|Get user name.|
|_live_|Open the live screen to watch all projects.|
|_log -sa_ , _log --show-all_|Show all projects daily logs.|
|_log -s_ , _log --show_|Show daily logs of project/projects.|
|_log -r_ , _log --remove_|Remove all logs of a project.|
|_log -ra_ , _log --remove-all_|Remove logs of all projects.|
|_mon_ , _monitor_|Open the monitor to watch a project.|
|_pr -s_ , _pr --status_|Show projects status.|
|_pr -r_ , _pr --restart_|Restart project.|
|_pr -ra_ , _pr --restart-all_|Restart all projects.|
|_pr -k_ , _pr --kill_|Kill project.|
|_pr -ka_ , _pr --kill-all_|Kill all projects.|
|_set -c_ , _set --config_|Set config file.|
|_set -u_ , _set --user_|Set user name.|
|_show -c_ , _show --config_|Show config file.|
|_sleep -s_ , _sleep --second_|Thread sleep as second.|
|_sleep -m_ , _sleep --minute_|Thread sleep as minute.|
|_sleep -h_ , _sleep --hour_|Thread sleep as hour.|

## Description of Commands

### add -p , add --project
 Add a new project to config. For instance, you installed this project to your linux os and you want to add new project on anywhere. At this time, you should use this command for it.
 
**Schema**
```bash
$ depmk add -p "<project_name>" "<command_name>" "<command_arguments>"
# OR
$ depmk add --project "<project_name>" "<command_name>" "<command_arguments>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/add-p.gif?raw=true" />
  <br />
  <br />
</div>
 
  We have to say that, project_name isn't at config.json until you run this command. Because this command write your project name as json to config.json, and if it is repeated,you will get an error from the project saying "**Project already exist!**" So you shouldn't worry about this command, because you can delete it after adding.
 
### add -s , add --startup
  If you want that it will be started on reboot, you must run this command. Thus, if your linux os is closed and then opened, your applications will be started auto by **depmk**.
  
  You have to go to project directory. Because alias of **depmk** couldn't be added to _.bashrc_ which root has. So you go to project directory and run command by schema.
  
**Schema 1**
```bash
$ dotnet run add -s "<linux_user_name>"
# OR
$ dotnet run add --startup "<linux_user_name>"
```

**Schema 2**
```bash
$ dotnet exec bin/Release/netcoreapp2.1/dmuka2.CS.Deploy.dll add -s "<linux_user_name>"
# OR
$ dotnet exec bin/Release/netcoreapp2.1/dmuka2.CS.Deploy.dll add --startup "<linux_user_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/add-s.gif?raw=true" />
  <br />
  <br />
</div>

 You may get an error saying "_You don't have permission for this operation_" from linux. You should use **sudo** to fix it. If you wonder _why do I have to use sudo?_, we are writing to **/etc/crontab** files that startup shell scripts to run this project via your linux user.
 
### add -a , add --alias
 
 Actually, this is the most important thing in this project's commands. Why am I saying this? Because you have to run this command to use **depmk** command in bash. This command will add the **depmk** to _.bashrc_ as alias to the end line and reload on your current terminal to enable run. It also build the project on release mode to run last version of codes. It means if you change anything on this project, you have to run either this command or **install.sh**.

> You can also use this command to update your project by the source code of project.
  
**Schema**
```bash
$ depmk add -a
# OR
$ depmk add --alias
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/add-a.gif?raw=true" />
  <br />
  <br />
</div>
 
 You only need a permission that you can change the _.bashrc_ file. You don't need **sudo** for this situation.
  
### alog -sa , alog --show-all
  
 To see all projects' agent logs, you use this command. What does it mean? For instance, you build this project on your linux os and close the terminal or close the ssh. But you noticed that your project is very slow while running in the background. At this moment, you can get agent logs to see cpu and ram via this command for all projects.
   
**Schema**
```bash
$ depmk alog -sa
# OR
$ depmk alog --show-all
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/alog-sa.gif?raw=true" />
</div>
 
### alog -s , alog --show
  
 To see a project's agent logs, you use this command. What does it mean? For instance, you build this project on your linux os and close the terminal or close the ssh. But you noticed that your project is very slow while running in the background.. At this moment, you can get agent logs to see cpu and ram via this command for a project.
   
**Schema**
```bash
$ depmk alog -s "<project_name>"
# OR
$ depmk alog --show "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/alog-s.gif?raw=true" />
</div>
 
### clear
 
 To clear console, you use this command. I guess you don't need more detail.
  
**Schema**
```bash
$ depmk clear
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/clear.gif?raw=true" />
</div>
 
### cwd
 
 To write current directory of the project, you use this command for extreme situations. For instance, you can manage the config via other applications, you have to know where the logs and other things are. This command gives this.
  
**Schema**
```bash
$ depmk cwd
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/cwd.gif?raw=true" />
</div>
 
### db -c , db --connect
 
 To connect to a database, you use this command. It only checks that can we connect to the database.
  
**Schema**
```bash
$ depmk db -c "<database_name>"
# OR
$ depmk db --connect "<database_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/db-c.gif?raw=true" />
</div>
 
### db -ca , db --connect-all
 
 To connect to all databases, you use this command. It only checks that can we connect to the databases.
  
**Schema**
```bash
$ depmk db -ca
# OR
$ depmk db --connect-all
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/db-ca.gif?raw=true" />
</div>
 
### db -r , db --remove
 
 To remove all tables of a database, you use this command. You can think as clear database. But it only removes tables, doesn't change other things like trigger etc. However, you must send with an argument that "_I am sure to remove_".
  
**Schema**
```bash
$ depmk db -r "<database_name>" "I am sure to remove"
# OR
$ depmk db --remove "<database_name>" "I am sure to remove"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/db-r.gif?raw=true" />
</div>
 
### db -ra , db --remove-all
 
 To remove all tables of all databases, you use this command. You can think as clear database. But it only removes tables, doesn't change other things like trigger etc. However, you must send with an argument that "_I am sure to remove_".
  
**Schema**
```bash
$ depmk db -ra "I am sure to remove"
# OR
$ depmk db --remove-all "I am sure to remove"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/db-ra.gif?raw=true" />
</div>
 
### db -m , db --migration
 
 To commit the migrations of a database, you use this command. You can see the path on the **config.json**. There is the migration path. But you shouldn't forget that you must save your sql migrations as file on your disk. Program works by file name. First, it gets all files from your migration path and then the file names are sorted by it. Finally, program commits them by order. While the file names are being sorted, file extension doesn't matter.
  
**Schema**
```bash
$ depmk db -m "<database_name>"
# OR
$ depmk db --migration "<database_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/db-m.gif?raw=true" />
</div>
 
### db -ma , db --migration-all
 
 To commit the migrations of all databases, you use this command. You can see the path on the **config.json**. There is the migration path. But you shouldn't forget that you must save your sql migrations as file on your disk. Program works by file name. First, it gets all files from your migration path and then the file names are sorted by it. Finally, program commits them by order. While the file names are being sorted, file extension doesn't matter.
  
**Schema**
```bash
$ depmk db -ma
# OR
$ depmk db --migration-all
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/db-ma.gif?raw=true" />
</div>
 
### del -p , del --project
  
 To delete a project from the config, you use this command..
   
**Schema**
```bash
$ depmk del -p "<project_name>"
# OR
$ depmk del --project "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/del-p.gif?raw=true" />
</div>
 
### del -s , del --startup
  
 To delete from startup by linux username, you can use this command.
   
  You have to go to project directory. Because alias of **depmk** couldn't be added to _.bashrc_ which root has. So you go to project directory and run command by schema.
   
**Schema 1**
```bash
$ dotnet run del -s "<linux_user_name>"
# OR
$ dotnet run del --startup "<linux_user_name>"
```

**Schema 2**
```bash
$ dotnet exec bin/Release/netcoreapp2.1/dmuka2.CS.Deploy.dll del -s "<linux_user_name>"
# OR
$ dotnet exec bin/Release/netcoreapp2.1/dmuka2.CS.Deploy.dll del --startup "<linux_user_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/del-s.gif?raw=true" />
 <br />
 <br />
</div>

 You may get an error saying "_You don't have permission for this operation_" from linux. You should use **sudo** to fix it. If you wonder _why do I have to use sudo?_, we are writing to **/etc/crontab** files that startup shell scripts to run this project via your linux user.
 
### del -a , del --alias
  
 To delete **depmk** alias from _.bashrc_, you can use this command.
   
**Schema**
```bash
$ depmk del -a
# OR
$ depmk del --alias
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/del-a.gif?raw=true" />
 <br />
 <br />
</div>

 You only need a permission that you can change the .bashrc file. You don't need sudo for this situation.
  
### exit
 
 If you use ui, you shouldn't forget that you have to use this command when you want to close it. Because if you use CTRL+C, linux os sends SIGINT to child processes and projects like nodejs are closed automatically without your command.
 
**Schema**
```bash
$ depmk exit
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/exit.gif?raw=true" />
</div>
  
### set -u , set --user / get -u , get --user
 
 Sometimes you have different project's paths. For these paths, you can use username. If you closely review config.json, you can see default on commands. You can add more usernames there. But if you do it, you must use **set -u** command to catch this data. **get -u** command only shows _What is the current username?_.
 
**Schema**
```bash
$ depmk set -u "<user_name>"
$ depmk get -u
# OR
$ depmk set --user "<user_name>"
$ depmk get --user
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/set-u-get-u.gif?raw=true" />
  <br />
  <br />
</div>
 
 But don't forget that this command works only on a session. It means if you use any command like **depmk pr -r** which needs a path, you must use **set -u** before this command. For instance, **depmk set -u dmuka pr -r**. It's not globally variable.
 
### live
 
 To show all projects' status/cpu/ram datas live.
  
**Schema**
```bash
$ depmk live
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/live.gif?raw=true" />
</div>
  
### log -sa , log --show-all
  
 To show all projects' daily logs data, you can use this command. If you want to see previous days' logs, you have to open on project directory that "dmuka2.CS.Deploy/Log". You can get logs of all time from there.
   
**Schema**
```bash
$ depmk log -sa
# OR
$ depmk log --show-all
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/log-sa.gif?raw=true" />
</div>
  
### log -s , log --show
  
 To show all projects' daily logs data, this command is available. If you want to see previous days' logs, you have to open on project directory that "dmuka2.CS.Deploy/Log". You can get logs of all time from there.
  
**Schema**
```bash
$ depmk log -s "<project_name>"
# OR
$ depmk log --show "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/log-s.gif?raw=true" />
</div>
 
### log -ra , log --remove-all
  
 To delete all logs with agent logs, use this command.
  
**Schema**
```bash
$ depmk log -ra
# OR
$ depmk log --remove-all
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/log-ra.gif?raw=true" />
</div>
 
### log -r , log --remove
  
 To delete a project's logs with agent logs, this command is available.
  
**Schema**
```bash
$ depmk log -r "<project_name>"
# OR
$ depmk log --remove "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/log-r.gif?raw=true" />
</div>
 
### mon , monitor
  
 To show a project's live cpu and ram on graphics, you can use this commmand.
  
**Schema**
```bash
$ depmk mon "<project_name>"
# OR
$ depmk monitor "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/mon.gif?raw=true" />
</div>
  
### pr -s , pr --status
  
 To show all projects' status/cpu/ram data, you can use this command.
   
**Schema**
```bash
$ depmk pr -s
# OR
$ depmk pr --status
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-s.gif?raw=true" />
</div>
  
### pr -ra , pr --restart-all
  
 To restart all projects, use this command.
   
**Schema**
```bash
$ depmk pr -ra
# OR
$ depmk pr --restart-all
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-ra.gif?raw=true" />
</div>
 
### pr -r , pr --restart
  
 To restart a project, this command is available.
   
**Schema**
```bash
$ depmk pr -r "<project_name>"
# OR
$ depmk pr --restart "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-r.gif?raw=true" />
</div>
 
### pr -ka , pr --kill-all
  
 To kill all projects, you can use this command.
   
**Schema**
```bash
$ depmk pr -ka
# OR
$ depmk pr --kill-all
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-ka.gif?raw=true" />
</div>
 
### pr -k , pr --kill
  
 To kill a project, use this command.
   
**Schema**
```bash
$ depmk pr -k "<project_name>"
# OR
$ depmk pr --kill "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-k.gif?raw=true" />
</div>

### set -c , set --config
  
 To set new config by file path on current directory, this command is available.
   
**Schema**
```bash
$ depmk set -c "<file_path>"
# OR
$ depmk set --config "<file_path>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/set-c.gif?raw=true" />
</div>
  
### show -c , show --config
  
 To show config file, use this command.

**Schema**
```bash
$ depmk show -c
# OR
$ depmk show --config
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/show-c.gif?raw=true" />
</div>

## Example Config

```js
{
	"database": {
		"test_postgres": {
		  "type": "postgres",
		  "connection_string": {
			"default": "User ID=postgres;Password=1;Host=localhost;Port=5432;Database=Test;"
		  },
		  "migration_path": {
			  "default": "./Database/Migration/test_postgres"
		  }
		},
		"test_mysql": {
		  "type": "mysql",
		  "connection_string": {
			"default": "Server=localhost;Database=Test;Uid=root;Pwd=1;Allow User Variables=True;"
		  },
		  "migration_path": {
			  "default": "./Database/Migration/test_mysql"
		  }
		},
		"test_mssql": {
		  "type": "mssql",
		  "connection_string": {
			"default": "Server=localhost\\SQLEXPRESS;Database=Test;User Id=sa;Password=1;"
		  },
		  "migration_path": {
			  "default": "./Database/Migration/test_mssql"
		  }
		}
	},
  "project": {
    "test_consoleapp": {
      "commands": [
        {
          "main": false,
          "name": "dotnet",
          "arguments": "clean",
          "path": {
            "default": "./TestProjects/Test.ConsoleApp"
          }
        },
        {
          "main": false,
          "name": "dotnet",
          "arguments": "build -c Release",
          "path": {
            "default": "./TestProjects/Test.ConsoleApp"
          }
        },
        {
          "main": true,
          "name": "dotnet",
          "arguments": "exec Test.ConsoleApp.dll",
          "path": {
            "default": "./TestProjects/Test.ConsoleApp/bin/Release/netcoreapp2.1"
          }
        }
      ]
    },
    "test_nodejs": {
      "commands": [
        {
          "main": true,
          "name": "node",
          "arguments": "app.js",
          "path": {
            "default": "./TestProjects/Test.NodeJS"
          }
        }
      ]
    }
  }
}

```
