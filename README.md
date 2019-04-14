<div align="center">
  <br/>
  <img src="https://raw.githubusercontent.com/muhammet-kandemir-95/dmuka2.CS.Deploy/master/mdcontent/images/main.png" />
  <br/>
  <img width="140px" src="https://raw.githubusercontent.com/muhammet-kandemir-95/dmuka2.CS.Deploy/master/mdcontent/images/version.png" alt="version" />
</div>

## What is this?
 This application provides to you that you can manage your applications which are working on your linux os. Also, you can see text log which is given by your applications. Thus, when you connect to your linux system via ssh or other ways, you can get what happened on your applications. 

 When you want to see how much cpu and ram are your applications using, default codes write it as agent log to text files. But you can use it to other ui systems(We haven't made it yet) like pm2+ with source code. It is very simple.
 
  Our programs sometimes doesn't work as well and we need to see what is it doing at the moment. This project ensure the live cpu and ram datas via bash terminal with colors to you understand easly. You don't need anytool to use it.

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

> Help will give you what are the commands in projects. It also will give 3 example scenario to learn. If you don't understand, you should look at **What is the commands?** section to more example and more descriptions.

## What is the project ui? How can I see it?

 First you have to run bash script "**depmk**" after installed to see this. If you do it, you see our ui designer. I can hear "_Why will I use it_". Reason of this is more attention and more details. When you use other way(**depmk pr -r** like that), program won't ask anything for any warning. But this way provides it. Also, if you haven't used it, you should use this project with ui.

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/ui.gif?raw=true" />
</div>

## What are the commands in this project?

### add -p
 Add a new project to config. For instance, you installed this project to your linux os and you want to add new project on anywhere. At this time, you should use this command for it.
 
**Schema**
```console
depmk add -p "<project_name>" "<command_name>" "<command arguments>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/add-p.gif?raw=true" />
  <br />
  <br />
</div>
 
  We have to say that project_name isn't at config.json before run this command. Because this command write to config.json your project name as json, and if it repeat, you will take an error from this project which is "**Project already is exist!**. So you shouldn't worry about this command. Because you can remove it after added. Also, if you don't run your project which was added by you, nothing will happen.
 
### add -s
  If you want to will be started on reboot, you must run this command. Thus, if your linux os is closed and then opened, your applications will be started auto by **depmk**.
  
**Schema**
```console
depmk add -s "<linux_user_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/add-s.gif?raw=true" />
  <br />
  <br />
</div>

 You may take an error which is "_You don't have permission for this operation_" from linux. You should use **sudo** to fix it. If you wonder _why do I have to use sudo?_, we are writing to **/etc/crontab** files that startup shell scripts to run this project via your linux user.
 
### add -a
 
 Actually, this is the most important thing in this project's commands. Why am I saying this? Because you have to run this command to use **depmk** command in bash. This command will add the **depmk** to _.bashrc_ as alias to end line and reload on your current terminal to able run. It also build the project on release mode to run last version of codes. It means if you change anything on this project, you have to run either this command or **install.sh**.
  
**Schema**
```console
depmk add -a
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/add-a.gif?raw=true" />
  <br />
  <br />
</div>
 
 You only need a permission that you can change the _.bashrc_ file. You don't need **sudo** for this situation.
  
### alog -sa
  
 To see all projects agent logs. What does it mean? For instance, you build this project on your linux os and close the terminal or close the ssh. But you notice a thing that your project is very slow while is running background. At this moment, you can get agent logs to see cpu and ram via this command for all projects.
   
**Schema**
```console
depmk alog -sa
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/alog-sa.gif?raw=true" />
</div>
 
### alog -s
  
 To see a project agent logs. What does it mean? For instance, you build this project on your linux os and close the terminal or close the ssh. But you notice a thing that your project is very slow while is running background. At this moment, you can get agent logs to see cpu and ram via this command for a project.
   
**Schema**
```console
depmk alog -s "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/alog-s.gif?raw=true" />
</div>
 
### clear
 
 To clear console. I guess you don't need more detail.
  
**Schema**
```console
depmk clear
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/clear.gif?raw=true" />
</div>
 
### cwd
 
 To write the current directory of project. You can use this command for extreme situations. For instance, you can manage the config via other applications, you have to know where are the logs or other things. This command give it.
  
**Schema**
```console
depmk cwd
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/cwd.gif?raw=true" />
</div>
  
### exit
 
 If you use the ui, you don't forget a thing that you must use this command when you want to close it. Because, if you use _CTRL + C_, linux os send _SIGINT_ to child processes and projects like nodejs is closed auto without your commands.
 
**Schema**
```console
depmk exit
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/exit.gif?raw=true" />
</div>
  
### set -u / get -u
 
 Sometimes, you have different project's paths. For this paths you can use **username**. If you detailed review config.json, you see **default** on commands. You can add more **username** there. But if you do it, you must use **set -u** command to program catch this data. **get -u** command only show _What is the current username?_.
 
**Schema**
```console
depmk set -u "<user_name>"
depmk get -u
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/set-u-get-u.gif?raw=true" />
  <br />
  <br />
</div>
 
 But don't forget that this command works only on a session. It means if you use any command like **depmk pr -r** which needs a path, you must use **set -u** before this command. For instance, **depmk set -u dmuka pr -r**. It's not global variable.
 
### live
 
 To show all project's status/cpu/ram datas live.
  
**Schema**
```console
depmk live
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/live.gif?raw=true" />
</div>
  
### log -sa
  
 To show all projects log datas.
   
**Schema**
```console
depmk log -sa
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/log-sa.gif?raw=true" />
</div>
  
### log -s
  
 To show a project's log datas.
  
**Schema**
```console
depmk log -s "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/log-s.gif?raw=true" />
</div>
 
### log -ra
  
 To remove all logs with agent logs.
  
**Schema**
```console
depmk log -ra
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/log-ra.gif?raw=true" />
</div>
 
### log -r
  
 To remove a project's logs with agent logs.
  
**Schema**
```console
depmk log -r "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/log-r.gif?raw=true" />
</div>
 
### mon
  
 To show a project's cpu and ram on graphics live.
  
**Schema**
```console
depmk mon "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/mon.gif?raw=true" />
</div>
  
### pr -s
  
 To show all projects status/cpu/ram datas.
   
**Schema**
```console
depmk pr -s
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-s.gif?raw=true" />
</div>
  
### pr -ra
  
 To restart all projects.
   
**Schema**
```console
depmk pr -ra
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-ra.gif?raw=true" />
</div>
 
### pr -r
  
 To restart a project.
   
**Schema**
```console
depmk pr -r "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-r.gif?raw=true" />
</div>
 
### pr -ka
  
 To kill all projects.
   
**Schema**
```console
depmk pr -ka
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-ka.gif?raw=true" />
</div>
 
### pr -k
  
 To kill a project.
   
**Schema**
```console
depmk pr -k "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/pr-k.gif?raw=true" />
</div>
 
### remove -p
  
 To remove a project from config.
   
**Schema**
```console
depmk remove -p "<project_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/remove-p.gif?raw=true" />
</div>
 
### remove -s
  
   To remove from startup by linux user name.
   
**Schema**
```console
depmk remove -s "<linux_user_name>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/remove-s.gif?raw=true" />
</div>
 
### remove -a
  
 To remove **depmk** alias from _.bashrc_.
   
**Schema**
```console
depmk remove -a
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/remove-a.gif?raw=true" />
</div>
  
### set -c
  
 To set new config by file path on current directory.
   
**Schema**
```console
depmk set -c "<file_path>"
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/set-c.gif?raw=true" />
</div>
  
### show -c
  
 To show config file.
   

**Schema**
```console
depmk show -c
```

<div>
 <img src="https://github.com/muhammet-kandemir-95/dmuka2.CS.Deploy/blob/master/mdcontent/images/show-c.gif?raw=true" />
</div>

## Example Config

```js
{
  "database": {
    // For postgres
    "TestDb": {
      "type": "postgres",
      "connection_string": {
        "default": "User ID=postgres;Password=123;Host=localhost;Port=5432;Database=TestDb;"
      }
    }
  },
  "project": {
    // For dotnet core
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
    // For nodejs
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
