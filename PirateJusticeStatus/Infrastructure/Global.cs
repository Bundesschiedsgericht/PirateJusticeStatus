using System;
using System.IO;
using System.Collections.Generic;

namespace PirateJusticeStatus.Infrastructure
{
    public static class Global
    {
		private static Config _config;
		private static IDatabase _database;
		private static UserMapper _users;
		private static Logger _logger;
		private static Mailer _mailer;
        
		private static IEnumerable<string> ConfigPaths
		{
			get
			{
				yield return "/Security/PPDE/piratejusticestatus.xml";
				yield return "/etc/piratejusticestatus.xml";
			}
		}

        private static string ConfigPath
		{
			get
			{
                foreach (var path in ConfigPaths)
				{
                    if (File.Exists(path))
					{
						return path;
					}
				}

				throw new FileNotFoundException();
			}
		}

		public static Config Config
		{
			get
			{
				if (_config == null)
				{
					_config = new Config();
					_config.Load(ConfigPath);
				}

				return _config;
			}
		}

		public static IDatabase Database
		{
			get
			{
				if (_database == null)
				{
					_database = new PostgresDatabase(Config);
				}

				return _database;
			}
		}

		public static UserMapper Users
        {
            get
            {
                if (_users == null)
                {
					_users = new UserMapper();
                }

				return _users;
            }
        }

		public static Logger Log
        {
            get
            {
				if (_logger == null)
                {
					_logger = new Logger();
                }

				return _logger;
            }
        }

		public static Mailer Mail
		{
            get
            {
				if (_mailer == null)
                {
					_mailer = new Mailer(Log, Config);
                }

				return _mailer;
            }
        }
    }
}
