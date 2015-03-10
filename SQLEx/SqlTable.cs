using System;
using System.Collections.Generic;
using System.Data.OleDb;
using CoreEx;

namespace SQLEx
{
    public class SqlTable<T> : SqlTableBase<T>, IDisposable where T : SqlObject
    {
        public SqlTable(string tableName) : base(tableName) {
        }

        protected override List<T> ProcessSelect(string sql, bool single, IMsSqlConnection connection)
        {
            List<T> list = new List<T>();
            using (connection.Open())
            {
                OleDbCommand command = new OleDbCommand(sql);

                command.Connection = connection.Connection;
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T item = (T)Activator.CreateInstance(typeof(T));
                        item.ReadSqlDataReader(reader);
                        list.Add(item);
                        if (single)
                        {
                            break;
                        }
                    }
                }
            }
            return list;
        }
    }
}
