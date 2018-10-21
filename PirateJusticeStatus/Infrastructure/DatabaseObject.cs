using System;
using System.Collections.Generic;

namespace PirateJusticeStatus.Infrastructure
{
	public abstract class DatabaseObject
	{
		public const string ColumnId = "id";

		public Guid Id { get; protected set; }

		public DatabaseObject(Guid id)
		{
			Id = id;
		}

		public string Table { get { return GetType().Name.ToLowerInvariant(); } }

		public abstract IEnumerable<Column> Columns { get; }

		public virtual void CascadeQuery(IDatabase database) { }

		public virtual void CascadeUpdate(IDatabase database) { }

		public virtual void CascadeDelete(IDatabase database) { }
    }
}
