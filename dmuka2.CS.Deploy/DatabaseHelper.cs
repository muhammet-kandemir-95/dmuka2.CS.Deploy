using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace dmuka2.CS.Deploy
{
    /// <summary>
    /// We will use this library to manage database processes.
    /// <para></para>
    /// For instance, connect any database with anonymous type.
    /// </summary>
    public static class DatabaseHelper
    {
        #region Variables
        #region Database Types
        public const string Postgres = "postgres";
        #endregion

        /// <summary>
        /// This means that "Database/Migration".
        /// <para></para>
        /// This will be used to get new migrations.
        /// </summary>
        static string __migrationDirectoryPath = null;

        /// <summary>
        /// This data store queries which are in "Database/Command/&lt;database_type&gt;/&lt;query_name&gt;.sql" folder.
        /// <para></para>
        /// If you want to change something which about database or you want to add a new database, you must work on this folder.
        /// <para></para>
        /// Also, "query_name" will be stored without extension.
        /// <para></para>
        /// Schema = &lt;database_type, &lt;query_name, query_text&gt;&gt;
        /// </summary>
        static Dictionary<string, Dictionary<string, string>> __queries = new Dictionary<string, Dictionary<string, string>>();
        #endregion

        #region Classes

        #endregion

        #region Constructors
        static DatabaseHelper()
        {
            // We are reading the config on static constructor.
            // It's mean that if you change anything on the folder, you must restart this application.
            var currentDirectory = Directory.GetCurrentDirectory();
            __migrationDirectoryPath = Path.Combine(currentDirectory, "Database", "Migration");
            var databaseCommandDirectory = Path.Combine(currentDirectory, "Database", "Command");
            var databasesCommandNameAsDirectory = Directory.GetDirectories(databaseCommandDirectory);
            foreach (var databaseCommandNameAsDirectory in databasesCommandNameAsDirectory)
            {
                Dictionary<string, string> databaseCommandQueries = new Dictionary<string, string>();

                var databaseCommandQueriesNameAsFileName = Directory.GetFiles(databaseCommandNameAsDirectory);
                foreach (var databaseCommandQueryNameAsFileName in databaseCommandQueriesNameAsFileName)
                    databaseCommandQueries.Add(Path.GetFileNameWithoutExtension(databaseCommandQueryNameAsFileName), File.ReadAllText(databaseCommandQueryNameAsFileName, Encoding.UTF8));

                __queries.Add(Path.GetFileName(databaseCommandNameAsDirectory), databaseCommandQueries);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// This method only try to connect database via connection on config.json by "databaseName".
        /// </summary>
        /// <param name="databaseName">Which is database name in config.json?</param>
        public static void TryToConnect(string databaseName)
        {
            var databaseConfig = ConfigHelper.GetDatabase(databaseName);

            switch (databaseConfig.type)
            {
                case Postgres:
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(databaseConfig.connectionString))
                        {
                            connection.Open();

                            connection.Close();
                        }
                    }
                    break;
                default:
                    throw new Exception(databaseConfig.type + " was not supported by deploy!");
            }
        }

        /// <summary>
        /// This method remove all tables from all database.
        /// </summary>
        /// <param name="databaseName">Which is database name in config.json?</param>
        public static void RemoveAllTables(string databaseName)
        {
            var databaseConfig = ConfigHelper.GetDatabase(databaseName);

            switch (databaseConfig.type)
            {
                case Postgres:
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(databaseConfig.connectionString))
                        {
                            connection.Open();
                            using (NpgsqlCommand command = new NpgsqlCommand(__queries[databaseConfig.type]["RemoveAllTables"], connection))
                                command.ExecuteNonQuery();

                            connection.Close();
                        }
                    }
                    break;
                default:
                    throw new Exception(databaseConfig.type + " was not supported by deploy!");
            }
        }

        #region Migration
        /// <summary>
        /// We are creating the migration to check new updates if the migration table is not exist.
        /// <para></para>
        /// If it is exists, this method won't do anything.
        /// </summary>
        /// <param name="databaseName">Which is database name in config.json?</param>
        static void createMigrationTable(string databaseName)
        {
            var databaseConfig = ConfigHelper.GetDatabase(databaseName);

            switch (databaseConfig.type)
            {
                case Postgres:
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(databaseConfig.connectionString))
                        {
                            connection.Open();

                            using (NpgsqlCommand command = new NpgsqlCommand(__queries[databaseConfig.type]["CreateMigrationTable"], connection))
                                command.ExecuteNonQuery();

                            connection.Close();
                        }
                    }
                    break;
                default:
                    throw new Exception(databaseConfig.type + " was not supported by deploy!");
            }
        }

        /// <summary>
        /// We are reading migrations from database which you gave name to this method by config.json.
        /// <para></para>
        /// Results only contain file name of migration.
        /// </summary>
        /// <param name="databaseName">Which is database name in config.json?</param>
        /// <returns></returns>
        static string[] getAllMigrationsFromDatabase(string databaseName)
        {
            var databaseConfig = ConfigHelper.GetDatabase(databaseName);
            List<string> migrationFileNames = new List<string>();

            switch (databaseConfig.type)
            {
                case Postgres:
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(databaseConfig.connectionString))
                        {
                            connection.Open();

                            using (NpgsqlCommand migrationReadCommand = new NpgsqlCommand("SELECT file_name FROM public.__migrations;", connection))
                            using (NpgsqlDataReader migrationReadReader = migrationReadCommand.ExecuteReader())
                                while (migrationReadReader.Read())
                                    migrationFileNames.Add(migrationReadReader[0].ToString());

                            connection.Close();
                        }
                    }
                    break;
                default:
                    throw new Exception(databaseConfig.type + " was not supported by deploy!");
            }

            var resultAsArray = migrationFileNames.ToArray();
            migrationFileNames.Clear();
            return resultAsArray;
        }

        /// <summary>
        /// We are reading migrations from disk which you gave name to this method by config.json on the migration directory path.
        /// <para></para>
        /// Results only contain file name of migration.
        /// </summary>
        /// <param name="databaseName">Which is database name in config.json?</param>
        /// <returns></returns>
        static string[] getAllMigrationsFromDisk(string databaseName)
        {
            var databaseMigrationDirectoryPath = Path.Combine(__migrationDirectoryPath, databaseName);
            if (Directory.Exists(databaseMigrationDirectoryPath) == false)
                return new string[0];

            return Directory.GetFiles(Path.Combine(__migrationDirectoryPath, databaseName)).Select(o => Path.GetFileName(o)).ToArray();
        }

        /// <summary>
        /// We will execute a migration that will be by "fileName" by "databaseName".
        /// </summary>
        /// <param name="databaseName">Which is database name in config.json?</param>
        /// <param name="fileName">Which is migration name in migration directory?</param>
        static void runMigration(string databaseName, string fileName)
        {
            var databaseConfig = ConfigHelper.GetDatabase(databaseName);
            switch (databaseConfig.type)
            {
                case Postgres:
                    {
                        using (NpgsqlConnection connection = new NpgsqlConnection(databaseConfig.connectionString))
                        {
                            connection.Open();

                            using (NpgsqlCommand applyMigrationCommand = new NpgsqlCommand(File.ReadAllText(Path.Combine(__migrationDirectoryPath, databaseName, fileName), Encoding.UTF8), connection))
                                applyMigrationCommand.ExecuteNonQuery();

                            using (NpgsqlCommand addMigrationCommand = new NpgsqlCommand(__queries[databaseConfig.type]["InsertAMigration"], connection))
                            {
                                addMigrationCommand.Parameters.Add(new NpgsqlParameter("@file_name", fileName));
                                addMigrationCommand.ExecuteNonQuery();
                            }

                            connection.Close();
                        }
                    }
                    break;
                default:
                    throw new Exception(databaseConfig.type + " was not supported by deploy!");
            }
        }

        /// <summary>
        /// We are applying new migrations and save this changes to database.
        /// <para></para>
        /// Then, execute callback parameters to show logs.
        /// </summary>
        /// <param name="databaseName">Which is database name in config.json?</param>
        /// <param name="callbackMigrationBeforeRun">To catch the migrations</param>
        public static void ApplyMigration(string databaseName, Action<string> callbackMigrationBeforeRun)
        {
            createMigrationTable(databaseName);

            var databaseMigrations = getAllMigrationsFromDatabase(databaseName);
            var diskMigrations = getAllMigrationsFromDisk(databaseName);
            var differentMigrations = diskMigrations
                            .Where(o => databaseMigrations.Any(a => a == o) == false)
                            .ToArray();

            var databaseMigrationDirectoryPath = Path.Combine(__migrationDirectoryPath, databaseName);
            
            foreach (var migrationFileName in differentMigrations)
            {
                callbackMigrationBeforeRun(migrationFileName);
                runMigration(databaseName, migrationFileName);
            }
        }
        #endregion

        #endregion
    }
}
