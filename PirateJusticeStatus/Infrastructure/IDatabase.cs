using System;
using System.Linq;
using System.Collections.Generic;

namespace PirateJusticeStatus.Infrastructure
{
    public interface IDatabase
    {
		IEnumerable<T> Query<T>() where T : DatabaseObject, new();
		IEnumerable<T> Query<T>(string column, Guid value, Func<T, Guid> accessor) where T : DatabaseObject, new();
		T Query<T>(Guid id) where T : DatabaseObject, new();
		void Update<T>(T obj) where T : DatabaseObject, new();
		void Insert<T>(T obj) where T : DatabaseObject, new();
		void Delete<T>(T obj) where T : DatabaseObject, new();
		void CreateTable<T>() where T : DatabaseObject, new();
		void BeginTransaction();
		void CommitTransaction();
		void AbortTransaction();
    }
}
