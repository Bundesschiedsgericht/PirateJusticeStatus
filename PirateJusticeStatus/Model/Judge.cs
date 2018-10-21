using System;
using System.Collections.Generic;
using PirateJusticeStatus.Infrastructure;

namespace PirateJusticeStatus.Model
{
	public enum JudgeType
	{ 
		Full,
        Reserve,
        Chair
	}

	public enum JudgeStatus
	{
		Available,
        OnLeave,
        Resigned,
		Unavailable,
	}

	public class Judge : DatabaseObject
    {
		public string Name { get; set; }
		public JudgeType Type { get; set; }
		public JudgeStatus Status { get; set; }
		public Court Court { get; set;  }

		public Judge() : base(Guid.Empty)
        {
			Name = string.Empty;
			Type = JudgeType.Full;
			Status = JudgeStatus.Unavailable;
			Court = null;
        }

		public Judge(Guid id, Court court) : base(id)
        {
			Name = string.Empty;
            Type = JudgeType.Full;
            Status = JudgeStatus.Unavailable;
			Court = court;
        }

		public override IEnumerable<Column> Columns
        {
            get
            {
				yield return new Column<Guid>("id", 16, () => Id, (v) => Id = v);
                yield return new Column<string>("name", 256, () => Name, (v) => Name = v);
				yield return new Column<int>("type", 256, () => (int)Type, (v) => Type = (JudgeType)v);
				yield return new Column<int>("status", 256, () => (int)Status, (v) => Status = (JudgeStatus)v);
				yield return new Column<Guid, Court>("courtid", 16, () => Court.Id, (v) => { });
            }
        }
	}
}
