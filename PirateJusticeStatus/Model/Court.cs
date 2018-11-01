﻿using System;
using System.Linq;
using System.Collections.Generic;
using PirateJusticeStatus.Infrastructure;
using PirateJusticeStatus.Util;

namespace PirateJusticeStatus.Model
{
	public enum CaseLoad
	{ 
		TenPlus,
        FivePlus,
        TwoPlus,
        One,
        None,
	}

	public class Court : DatabaseObject
	{
		public string Name { get; set; }
		public string BoardName { get; set; }
		public string Mail { get; set; }
		public string BoardMail { get; set; }
		public List<Judge> Judges { get; set; }
		public CaseLoad CaseLoad { get; set; }
		public DateTime LastUpdate { get; set; }
		public DateTime LastReminder { get; set; }
		public int ReminderLevel { get; set; }
		public string CourtKey { get; set; }
		public string BoardKey { get; set; }

		public Court() : base(Guid.Empty)
		{
			Name = string.Empty;
			BoardName = string.Empty;
            Mail = string.Empty;
            BoardMail = string.Empty;
            Judges = new List<Judge>();
            CaseLoad = CaseLoad.None;
			LastUpdate = Timestamp.Default;
			LastReminder = Timestamp.Default;
			ReminderLevel = 0;
			CourtKey = Rng.Get(16).ToHexString();
			BoardKey = Rng.Get(16).ToHexString();
		}
        
		public Court(Guid id) : base(id)
        {
            Name = string.Empty;
			BoardName = string.Empty;
            Mail = string.Empty;
            BoardMail = string.Empty;
            Judges = new List<Judge>();
            CaseLoad = CaseLoad.None;
			LastUpdate = Timestamp.Default;
            LastReminder = Timestamp.Default;
			ReminderLevel = 0;
			CourtKey = Rng.Get(16).ToHexString();
			BoardKey = Rng.Get(16).ToHexString();
        }

		public override IEnumerable<Column> Columns
		{
			get
			{
				yield return new Column<Guid>("id", 16, () => Id, (v) => Id = v);
				yield return new Column<string>("name", 256, () => Name, (v) => Name = v);
				yield return new Column<string>("boardname", 256, () => BoardName, (v) => BoardName = v);
				yield return new Column<string>("mail", 256, () => Mail, (v) => Mail = v);
				yield return new Column<string>("boardmail", 256, () => BoardMail, (v) => BoardMail = v);
				yield return new Column<int>("caseload", 4, () => (int)CaseLoad, (v) => CaseLoad = (CaseLoad)v);
				yield return new Column<DateTime>("lastupdate", 8, () => LastUpdate, (v) => LastUpdate = v);
				yield return new Column<DateTime>("lastreminder", 8, () => LastReminder, (v) => LastReminder = v);
				yield return new Column<int>("reminderlevel", 4, () => ReminderLevel, (v) => ReminderLevel = v);
				yield return new Column<string>("courtkey", 64, () => CourtKey, (v) => CourtKey = v);
				yield return new Column<string>("boardkey", 64, () => BoardKey, (v) => BoardKey = v);
			}
		}

        public override void CascadeQuery(IDatabase database)
		{
			Judges.AddRange(database.Query<Judge>("courtid", Id, j => j.Court.Id));
			Judges.ForEach(j => j.Court = this);
		}

		public override void CascadeUpdate(IDatabase database)
		{
			var current = database.Query<Judge>("courtid", Id, j => j.Court.Id);
			foreach (var d in current.Where(c => !Judges.Any(o => o.Id.Equals(c.Id))))
			{
				database.Delete(d);
			}

			foreach (var n in Judges)
			{
				if (current.Any(c => c.Id.Equals(n.Id)))
				{
					database.Update(n);
				}
				else
				{
					database.Insert(n);
				}
			}
		}

		public override void CascadeDelete(IDatabase database)
		{
			foreach (var d in Judges)
			{
				database.Delete(d);
			}
		}
	}
}