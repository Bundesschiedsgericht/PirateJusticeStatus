using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using PirateJusticeStatus.Model;

namespace PirateJusticeStatus.Infrastructure
{
	public class PostgresDatabase : IDatabase, IDisposable
    {
		private NpgsqlConnection _connection;
		private NpgsqlTransaction _transaction;
		private Dictionary<Guid, DatabaseObject> _cache;
		private List<string> _completelyCached;

		public PostgresDatabase(Config config)
        {
			var connectionString = string.Format(
				"Server={0};Port={1};Database={2};User Id={3};Password={4};",
				config.DatabaseServer,
				config.DatabasePort,
				config.DatabaseName,
				config.DatabaseUsername,
			    config.DatabasePassword);
			_connection = new NpgsqlConnection(connectionString);
			_connection.Open();
			_cache = new Dictionary<Guid, DatabaseObject>();
			_completelyCached = new List<string>();
        }

        private NpgsqlCommand Command(string text, params string[] arguments)
		{
			if (_transaction != null)
			{
				return new NpgsqlCommand(string.Format(text, arguments), _connection, _transaction);
			}
			else
			{ 
				return new NpgsqlCommand(string.Format(text, arguments), _connection);
			}
		}

		public void Delete<T>(T obj) where T : DatabaseObject, new()
		{
			obj.CascadeDelete(this);
			var command = Command("DELETE FROM {0} WHERE id = @id", obj.Table);
			command.AddParam("@id", obj.Id);
            command.ExecuteNonQuery();
		}

		public void Insert<T>(T obj) where T : DatabaseObject, new()
		{
			if (_cache.ContainsKey(obj.Id))
            {
                throw new InvalidOperationException("Object already cached.");
            }

			var columns = string.Join(", ", obj.Columns.Select(c => c.Name));
			var values = string.Join(", ", obj.Columns.Select(c => "@" + c.Name));
			var command = Command("INSERT INTO {0} ({1}) VALUES ({2})", obj.Table, columns, values);
            foreach (var c in obj.Columns)
			{
				command.AddParam("@" + c.Name, c.Value);
			}
			command.ExecuteNonQuery();
			_cache.Add(obj.Id, obj);
			obj.CascadeUpdate(this);
		}

		public T Cache<T>(Guid id) where T : DatabaseObject, new()
		{
			if (_cache.ContainsKey(id))
			{
				return _cache[id] as T;
			}
			else
			{
				return null;
			}
		}
              
		public IEnumerable<T> Query<T>() where T : DatabaseObject, new()
		{
			var list = new List<T>();

			if (_completelyCached.Contains(typeof(T).FullName))
			{
                foreach (var obj in _cache.Values)
				{
					if (obj is T)
					{
						list.Add((T)obj);
					}
				}
			}
			else
			{
				T proto = new T();
				var columns = string.Join(", ", proto.Columns.Select(c => c.Name));
				var command = Command("SELECT {0} FROM {1}", columns, proto.Table);
				var cascade = new Queue<T>();

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var cached = Cache<T>((Guid)reader["id"]);

						if (cached == null)
						{
							T ret = new T();

							foreach (var c in ret.Columns)
							{
								c.Value = reader[c.Name];
							}

							_cache.Add(ret.Id, ret);
							cascade.Enqueue(ret);
							list.Add(ret);
						}
						else
						{
							list.Add(cached);
						}
					}
				}

				while (cascade.Count > 0)
                {
                    cascade.Dequeue().CascadeQuery(this);
                }

				_completelyCached.Add(typeof(T).FullName);
			}

			return list;
		}

		public IEnumerable<T> Query<T>(string column, Guid value, Func<T, Guid> accessor) where T : DatabaseObject, new()
        {
			var list = new List<T>();

			if (_completelyCached.Contains(typeof(T).FullName))
			{
				foreach (var obj in _cache.Values)
				{
					if (obj is T)
					{
						var tobj = (T)obj;

						if (accessor(tobj).Equals(value))
						{
							list.Add(tobj);
						}
					}
				}
			}
			else
			{
				T proto = new T();
				var columns = string.Join(", ", proto.Columns.Select(c => c.Name));
				var command = Command("SELECT {0} FROM {1} WHERE {2} = @{2}", columns, proto.Table, column);
				command.AddParam("@" + column, value);
				var cascade = new Queue<T>();

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var cached = Cache<T>((Guid)reader["id"]);

						if (cached == null)
						{
							T ret = new T();

							foreach (var c in ret.Columns)
							{
								c.Value = reader[c.Name];
							}

							_cache.Add(ret.Id, ret);
							cascade.Enqueue(ret);

							list.Add(ret);
						}
						else
						{
							list.Add(cached);
						}
					}
				}

                while (cascade.Count > 0)
				{
					cascade.Dequeue().CascadeQuery(this);
				}
			}

			return list;
        }

		public T Query<T>(Guid id) where T : DatabaseObject, new()
		{
			T cached = Cache<T>(id);

			if (cached != null)
			{
				return cached;
			}

			T proto = new T();
            var columns = string.Join(", ", proto.Columns.Select(c => c.Name));
            var command = Command("SELECT {0} FROM {1} WHERE id = @id", columns, proto.Table);
			command.AddParam("@id", id);
			var list = new List<T>();

			using (var reader = command.ExecuteReader())
			{
				if (reader.Read())
				{
					T ret = new T();

					foreach (var c in ret.Columns)
					{
						c.Value = reader[c.Name];
					}

					_cache.Add(ret.Id, ret);
					list.Add(ret);
				}
			}

            if (list.Count > 0)
			{
				list.Single().CascadeQuery(this);
				return list.Single();
			}
			else            
			{
				return null;
			}
		}

		public void Update<T>(T obj) where T : DatabaseObject, new()
		{
			if (!_cache.ContainsKey(obj.Id))
			{
				throw new InvalidOperationException("Object not cached.");
			}

			var columns = string.Join(", ", obj.Columns
			                          .Where(c => c.Name != "id")
			                          .Select(c => c.Name + " = @" + c.Name));
			var command = Command("UPDATE {0} SET {1} WHERE id = @id", obj.Table, columns);
            foreach (var c in obj.Columns)
            {
                command.AddParam("@" + c.Name, c.Value);
            }
            command.ExecuteNonQuery();
			obj.CascadeUpdate(this);
		}

		public void Dispose()
		{
			if (_connection != null)			
			{
				_connection.Close();
				_connection = null;
			}
		}

		private string ColumnTypeDefinition(Column column)
		{
            if (column.BaseType == typeof(Guid))
            {
                return string.Format("{0} uuid NOT NULL", column.Name);
            }
            else if (column.BaseType == typeof(Guid?))
            {
                return string.Format("{0} uuid", column.Name);
            }
            else if (column.BaseType == typeof(int))
            {
                return string.Format("{0} integer NOT NULL", column.Name);
            }
            else if (column.BaseType == typeof(string))
            {
                if (column.Size > 1024)
                {
                    return string.Format("{0} text NOT NULL", column.Name);
                }
                else
                {
                    return string.Format("{0} varchar({1}) NOT NULL", column.Name, column.Size);
                }
            }
            else if (column.BaseType == typeof(DateTime))
            {
                return string.Format("{0} timestamp NOT NULL", column.Name);
            }
            else
            {
                throw new InvalidOperationException("Data type " + column.BaseType.FullName + " not supported");
            }
		}

		private string ColumnDefinition(Column column)
		{
			var prepared = ColumnTypeDefinition(column);

			if (column.Name == "id")
			{
				return string.Format("{0} PRIMARY KEY", prepared);
			}
			else if (column.ReferencedTable != null)
			{
				return string.Format("{0} REFERENCES {1}(id)", prepared, column.ReferencedTable);
			}
			else
			{
				return prepared;
			}
		}

		public bool TableExists(string name)
		{
			var command = Command("SELECT count(1) FROM pg_catalog.pg_tables WHERE schemaname = 'public' AND tablename = @tablename");
			command.AddParam("@tablename", name);
			return (long)command.ExecuteScalar() == 1;
		}

		public void CreateTable<T>() where T : DatabaseObject, new()
		{
			T proto = new T();

			if (!TableExists(proto.Table))
			{
				var columnDefinitions = proto.Columns.Select(c => ColumnDefinition(c));
				var columns = string.Join(", ", columnDefinitions);
				var command = Command("CREATE TABLE {0} ({1})", proto.Table, columns);
				command.ExecuteNonQuery();
			}
		}

		public void BeginTransaction()
		{
			if (_transaction != null)
			{
				throw new InvalidOperationException("Transaction already active.");
			}

			_transaction = _connection.BeginTransaction();
		}

		public void CommitTransaction()
		{
			if (_transaction == null)
			{
				throw new InvalidOperationException("No transaction active.");
			}

			_transaction.Commit();
			_transaction = null;
		}

		public void AbortTransaction()
		{
			if (_transaction == null)
            {
                throw new InvalidOperationException("No transaction active.");
            }

            _transaction.Rollback();
            _transaction = null;
		}
	}

    public static class Extensions
	{
		public static void AddParam(this NpgsqlCommand command, string name, object value)
        {
            command.Parameters.Add(new NpgsqlParameter(name, value));
        }
	}
}
