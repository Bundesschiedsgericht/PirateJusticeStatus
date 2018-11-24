using System;
using System.IO;
using PirateJusticeStatus.Model;
using PirateJusticeStatus.Util;

namespace PirateJusticeStatus.Infrastructure
{
	public class Runner
	{
		private IDatabase _db;

		public Runner()
		{
			_db = Global.Database;
		}

		public void RunOnce()
		{
			var courts = _db.Query<Court>();

			foreach (var court in courts)
			{
                try
                {
                    RunCourt(court);
                }
                catch (Exception exception)
                {
                    Global.Log.Error("Runner error at " + court.Name);
                    Global.Log.Error(exception.ToString());
                }
			}
		}

		private void RunCourt(Court court)
		{
            if (court.Substitute != null)
            {
                return;
            }

			if (court.LastUpdate.AddDays(30) < DateTime.Now)
			{
				if (court.LastReminder <= court.LastUpdate)
				{
					// Request new update
					_db.BeginTransaction();

					try
					{
						court.CourtKey = Rng.Get(16).ToHexString();
						court.BoardKey = Rng.Get(16).ToHexString();
						court.LastReminder = DateTime.Now;
						court.ReminderLevel = 0;
						SendRequest(court, court.ReminderLevel);
						_db.Update(court);
						_db.CommitTransaction();
					}
					catch
					{
						_db.AbortTransaction();
                        throw;
                    }
				}
				else if (court.LastReminder.AddDays(5) < DateTime.Now)
				{
					// Remind to update
					_db.BeginTransaction();

					try
					{
						court.LastReminder = DateTime.Now;
						court.ReminderLevel++;
						SendRequest(court, court.ReminderLevel);
						_db.Update(court);
						_db.CommitTransaction();
					}
					catch
					{
						_db.AbortTransaction();
                        throw;
					}
				}
			}
		}

		private void SendRequest(Court court, int level)
		{
			switch (level)
			{
				case 0:
					SendRequest(court, false, "request.txt");
					break;
				case 1:
				case 2:
				case 3:
					SendRequest(court, false, "reminder.txt");
					break;
				case 4:
				case 5:
					SendRequest(court, true, "board.txt");
					break;
			}
		}

        private string ApplyTemplate(string text, string link, Court court)
        {
            return text
                .Replace("$$LINK$$", link)
                .Replace("$$COURT$$", court.Name)
                .Replace("$$BOARD$$", court.BoardName);
        }

		private void SendRequest(Court court, bool board, string filename)
		{
			var template = ReadTemplate(filename);
			var key = board ? court.BoardKey : court.CourtKey;
			var name = board ? court.BoardName : court.Name;
			var address = board ? court.BoardMail : court.Mail;
			var link = string.Format("{0}/login/{1}/{2}", Global.Config.WebSiteAddress, court.Id.ToString(), key);
            var subject = ApplyTemplate(template.Item1, link, court);
            var body = ApplyTemplate(template.Item2, link, court);

            if (address.IsNullOrEmpty())
			{
				if (board)
				{
					Global.Log.Warning("No mail address for " + court.BoardName);
					Global.Mail.SendAdmin("[PJS] Missing address", "No mail address for " + court.BoardName);
				}
				else
				{
					Global.Log.Warning("No mail address for " + court.Name);
					Global.Mail.SendAdmin("[PJS] Missing address", "No mail address for " + court.Name);
				}
			}
			else
			{
				Global.Mail.Send(name, address, subject, body);
				Global.Log.Warning("Requested from " + name + " at level " + court.ReminderLevel);
				Global.Mail.SendAdmin("[PJS] Requested", "Requested from " + name + " at level " + court.ReminderLevel);
			}         
		}

        private Tuple<string, string> ReadTemplate(string filename)
		{
			var path = Path.Combine("Template", filename);
			var text = File.ReadAllText(path);
			var position = text.IndexOf("\n", StringComparison.InvariantCulture);
			var subject = text.Substring(0, position);
			var body = text.Substring(position + 1);
			return new Tuple<string, string>(subject, body);
		}
    }
}
