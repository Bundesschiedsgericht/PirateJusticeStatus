using System;

namespace PirateJusticeStatus.Infrastructure
{
    public static class Global
    {
		private static Config _config;
		private static IDatabase _database;
		private static UserMapper _users;
		private static Logger _logger;
		private static Mailer _mailer;

		public static Config Config
		{
			get
			{
				if (_config == null)
				{
					_config = new Config();
					_config.Load("/Security/PPDE/piratejusticestatus.xml");
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
