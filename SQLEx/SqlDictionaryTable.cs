using System;
using System.Collections.Generic;
using System.Data.OleDb;
using CoreEx;

namespace SQLEx
{
    public class SqlDictionaryTable<T> : SqlTableBase<T> where T : SqlDictionaryObject
    {
        public SqlDictionaryTable(string tableName) : base(tableName) {
        }

        protected override List<T> ProcessSelect(string sql, bool single, IMsSqlConnection connection)
        {
            List<T> items = new List<T>();
            using (connection.Open())
            {
                OleDbCommand command = new OleDbCommand(sql);
                command.Connection = connection.Connection;
                using (OleDbDataReader reader = command.ExecuteReader())
                {
                    T lItem = null;
                    while (reader.Read())
                    {
                        EnsureItem(reader, ref lItem, items);

                        if(single && items.Count>1)
                        {
                            break;
                        }
                        lItem.ReadSqlDataReader(reader);
                    }
                }
            }
            return items;
        }

        private void EnsureItem(OleDbDataReader reader,ref T item, List<T> items)
        {
            if (item == null || !item.GetValue(item.GetIDFieldName()).Equals(SqlDictionaryObject.GetColumn(item.GetIDFieldName(), reader)))
            {
                item = FindById(items,item);
                if (null == item)
                {
                    item = (T) Activator.CreateInstance(typeof (T));
                    items.Add(item);
                    item.SetValue(item.GetIDFieldName(), SqlDictionaryObject.GetColumn(item.GetIDFieldName(), reader));
                }
            }
        }

        private T FindById(List<T> items,object id)
        {
            T res = null;
            foreach (T item in items)
            {
               if(item.GetValue(item.GetIDFieldName()).Equals(id))
               {
                   res = item;
                   break;
               } 
            }
            return res;
        }
    }
}
