using System;

namespace PirateJusticeStatus.Infrastructure
{
	public abstract class Column
	{
		public string Name { get; private set; }

		public int Size { get; private set; }

		public abstract object Value { get; set; }

        public Column(string name, int size)
		{
			Size = size;
			Name = name;
		}

		public abstract Type BaseType { get; }

		public virtual string ReferencedTable { get { return null; } }
	}
      
	public class Column<T> : Column
	{
		public Func<T> Get { get; private set; }
		public Action<T> Set { get; private set; }

		public Column(string name, int size, Func<T> get, Action<T> set) : base(name, size)
		{
			Get = get;
			Set = set;
		}

		public override object Value
		{
			get { return Get(); }
			set { Set((T)value); }
		}

		public override Type BaseType
		{
			get { return typeof(T); }
		}
	}

	public class Column<T, R> : Column<T> where R : DatabaseObject, new()
	{
		public Column(string name, int size, Func<T> get, Action<T> set) 
			: base(name, size, get, set)
		{ 
		}

		public override string ReferencedTable
		{
			get 
			{
				R temp = new R();
				return temp.Table;
			}
		}
	}
}
