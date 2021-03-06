﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dmuka2.CS.Deploy
{
	public static class ConfigHelper
	{
		#region Variables
		/// <summary>
		/// Our JToken to use manage everything in config.json.
		/// </summary>
		public static JToken Config { get; set; }

		/// <summary>
		/// Who is user using this class?
		/// <para></para>
		/// If you need more information, you can look at <see cref="SetUserName(string)"/> method's description.
		/// </summary>
		public static string UserName { get; private set; }

		public static string[] Databases { get; private set; }

		public static string[] Projects { get; private set; }

		static string __configFilePath = null;
		#endregion

		#region Methods
		/// <summary>
		/// To load config by current directory.
		/// </summary>
		/// <param name="force">When this parameter is false, if Config have been loaded, doesn't run again.</param>
		public static void Load(bool force = false)
		{
			if (Config != null && force == false)
				return;

			// We are reading the config on static constructor.
			// It's mean that if you change anything on the config, you must restart this application.
			__configFilePath = Path.Combine(Program.CurrentDirectory, "config.json");
			if (File.Exists(__configFilePath) == false)
				return;
			Config = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(__configFilePath, Encoding.UTF8));

			// We are getting all databases name in config.json.
			List<string> allDatabasesNameList = new List<string>();

			foreach (var databaseProperty in (JObject)Config.SelectToken("database"))
				allDatabasesNameList.Add(databaseProperty.Key);

			Databases = allDatabasesNameList.ToArray();
			allDatabasesNameList.Clear();

			// We are getting all projects name in config.json.
			List<string> allProjectsNameList = new List<string>();

			foreach (var projectProperty in (JObject)Config.SelectToken("project"))
				allProjectsNameList.Add(projectProperty.Key);

			Projects = allProjectsNameList.ToArray();
			allProjectsNameList.Clear();
		}

		/// <summary>
		/// To save Config as json to file.
		/// </summary>
		public static void Save()
		{
			File.WriteAllText(__configFilePath, Config.ToString(), Encoding.UTF8);
		}

		/// <summary>
		/// We are setting a value which is user name to get special names which are will be able to change for each developer.
		/// <para></para>
		/// Thus, when developer change, we can get the correct path.
		/// <para></para>
		/// But, config won't be changed. We only get the user name. We will use this name on RAM.
		/// </summary>
		/// <param name="userName">What is user name?</param>
		public static void SetUserName(string userName)
		{
			UserName = userName == "" ? "default" : userName;
		}

		/// <summary>
		/// This method will return a config data.
		/// <para></para>
		/// Also, it is working according to "database.&lt;name&gt;" json data in config.json.
		/// <para></para>
		/// Also, you will get "connection_string" by user name.
		/// </summary>
		/// <param name="name">Database name in config.json.</param>
		/// <returns></returns>
		public static (string type, string connectionString) GetDatabaseConnectionString(string name)
		{
			string type = Config.SelectToken("database." + name + ".type").Value<string>();
			string connectionString = Config.SelectToken("database." + name + ".connection_string." + UserName).Value<string>();

			return (type: type, connectionString: connectionString);
		}

		/// <summary>
		/// This method will return a config data.
		/// <para></para>
		/// Also, it is working according to "database.&lt;name&gt;" json data in config.json.
		/// <para></para>
		/// Also, you will get "migration_path" by user name.
		/// </summary>
		/// <param name="name">Database name in config.json.</param>
		/// <returns></returns>
		public static string GetDatabaseMigrationPath(string name)
		{
			return Path.GetFullPath(Path.Combine(Program.CurrentDirectory, Config.SelectToken("database." + name + ".migration_path." + UserName).Value<string>()));
		}

		/// <summary>
		/// This method will return a config data.
		/// <para></para>
		/// Also, it is working according to "project.&lt;name&gt;.commands" json data in config.json.
		/// <para></para>
		/// Also, you will get "path" by user name.
		/// </summary>
		/// <param name="name">Database name in config.json.</param>
		/// <returns></returns>
		public static (bool main, string name, string arguments, string path)[] GetProjectCommands(string name)
		{
			List<(bool main, string name, string arguments, string path)> result = new List<(bool main, string name, string arguments, string path)>();

			foreach (var command in (JArray)Config.SelectToken("project." + name + ".commands"))
			{
				result.Add((
						main: command.Value<bool>("main"),
						name: command.Value<string>("name"),
						arguments: command.Value<string>("arguments"),
						path: command.SelectToken("path." + UserName).Value<string>()
					));
			}

			var resultAsArray = result.ToArray();
			result.Clear();
			return resultAsArray;
		}
		#endregion
	}
}
