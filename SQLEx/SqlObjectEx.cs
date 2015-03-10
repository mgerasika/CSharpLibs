using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using CoreEx;

namespace SQLEx
{
    public abstract class SqlObject : SqlObjectBase
    {
        public SqlObject()
        {
        }

        internal override void ReadSqlDataReader(OleDbDataReader reader)
        {
            for (int i = 0; i < reader.FieldCount; ++i)
            {
                string name = reader.GetName(i);
                object value = reader.GetValue(i);

                this.SetValue(name, value);
            }
        }

        internal override string GetUpdateSql(string tableName)
        {
            string sql = "";

            string lColumns = "";
            foreach (KeyValuePair<string, object> field in GetFields())
            {
                if (!field.Key.Contains('_'))
                {
                    if (!string.IsNullOrEmpty(lColumns))
                    {
                        lColumns += ", ";
                    }
                    lColumns += String.Format("{0}={1}", 
                        field.Key,
                        SafeReader.Get(field.Value));
                }
            }
            if (!string.IsNullOrEmpty(lColumns))
            {
                object id = this.GetValue("ID");
                sql = String.Format("UPDATE {0} SET {1} WHERE ID={2}", 
                    tableName, 
                    lColumns,
                    SafeReader.Get(id));
                
            }

            return sql;
        }

        internal override string GetInsertSql(string tableName)
        {
            string sql = "";

            string lColumns = "";
            string lValues = "";

            foreach(KeyValuePair<string,object> field in GetFields())
            {
                if (!field.Key.Contains('_'))
                {
                    if (!string.IsNullOrEmpty(lColumns))
                    {
                        lColumns += ", ";
                    }
                    lColumns += String.Format("{0}", field.Key);

                    if (!string.IsNullOrEmpty(lValues))
                    {
                        lValues += ", ";
                    }
                    lValues += String.Format("{0}", SafeReader.Get(field.Value));
                }
            }
            if(!string.IsNullOrEmpty(lColumns))
            {
                sql = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", 
                    tableName, 
                    lColumns, 
                    lValues);
            }

            return sql;
        }

        internal override string GetDeleteSql(object id, string tableName)
        {
            string sql = String.Format("DELETE FROM {0} WHERE ID='{1}'", 
                tableName, 
                SafeReader.Get(id.ToString()));
            return sql;
        }
    }
}
