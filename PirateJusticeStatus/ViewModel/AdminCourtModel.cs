using System;
using System.Collections.Generic;
using PirateJusticeStatus.Model;
using System.Linq;
using PirateJusticeStatus.Util;

namespace PirateJusticeStatus.ViewModel
{
	public class AdminCourtModel : UpdateCourtModel
    {
		public string BoardName { get; set; }
		public string Mail { get; set; }
		public string BoardMail { get; set; }
		public string BoardKey { get; set; }
		public string CourtKey { get; set; }
		public string LastUpdate { get; set; }
		public string LastReminder { get; set; }
		public string ReminderLevel { get; set; }

		public AdminCourtModel()
			: base()
        {
			BoardName = string.Empty;
			Mail = string.Empty;
			BoardMail = string.Empty;
			PendingCases = string.Empty;
			CourtKey = string.Empty;
			BoardKey = string.Empty;
            LastUpdate = Timestamp.Default.Format();
			LastReminder = Timestamp.Default.Format();
			ReminderLevel = 0.ToString();
        }

		public AdminCourtModel(Court court)
			: base(court)
        {
			UpdateStatus = court.LastUpdate.ToShortDateString();

			if (court.Judges.Count > 0)
			{
				var judges = court.Judges
				                  .OrderBy(j => JudgeOrdering(j))
				                  .Select(j => FormatJudge(j));
				JudgeStatus = string.Join("<br/>", judges.ToArray());
			}
			else
			{
				JudgeStatus = "<font color=\"darkred\">Unbesetzt</font>";
			}
            

            BoardName = court.BoardName;
			Mail = court.Mail;
			BoardMail = court.BoardMail;
			BoardKey = court.BoardKey;
			CourtKey = court.CourtKey;
			LastUpdate = court.LastUpdate.Format();
			LastReminder = court.LastReminder.Format();
			ReminderLevel = court.ReminderLevel.ToString();
        }

        private string JudgeOrdering(Judge judge)
		{
            switch (judge.Type)
			{
				case JudgeType.Chair:
					return "1" + judge.Name;
				case JudgeType.Full:
					return "2" + judge.Name;
				case JudgeType.Reserve:
					return "3" + judge.Name;
				default:
					return "4" + judge.Name;
			}
		}

        private string FormatJudge(Judge judge)
		{
			var b = string.Format("{0} {1}", FormatJudgeType(judge.Type), judge.Name);

			switch (judge.Status)
			{
				case Model.JudgeStatus.Available:
					return "<font color=\"darkgreen\">" + b + "</font>";
				case Model.JudgeStatus.OnLeave:
					return "<font color=\"darkorange\">" + b + "</font>";
				case Model.JudgeStatus.Unavailable:
					return "<font color=\"darkred\">" + b + "</font>";
				case Model.JudgeStatus.Resigned:
					return "<s><font color=\"darkred\">" + b + "</font></s>";
				default:
					return b;
			}
		}

        private string FormatJudgeType(JudgeType type)
		{
			switch (type)
            {
				case JudgeType.Chair:
					return "VRi";
				case JudgeType.Full:
                    return "Ri";
				case JudgeType.Reserve:
                    return "ERi";
				default:
					return "X";
            }
		}

		public override void Update(Court court, IEnumerable<JudgeModel> judges)
		{
			base.Update(court, judges);
			court.Name = Name;
			court.BoardName = BoardName;
			court.Mail = Mail;
			court.BoardMail = BoardMail;

			if (CourtKey.IsNullOrEmpty())
			{
				court.CourtKey = Rng.Get(16).ToHexString();
			}
			else
			{
				court.CourtKey = CourtKey;
			}

			if (BoardKey.IsNullOrEmpty())
            {
				court.BoardKey = Rng.Get(16).ToHexString();
            }
            else
            {
				court.BoardKey = BoardKey;
            }

			court.LastUpdate = LastUpdate.ParseTimestamp();
			court.LastReminder = LastReminder.ParseTimestamp();
			court.ReminderLevel = int.Parse(ReminderLevel);
		}
    }
}
