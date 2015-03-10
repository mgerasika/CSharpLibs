using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;

namespace SQLEx
{
    public abstract class SqlDictionaryObject : SqlObjectBase
    {
        public abstract string GetIDFieldName();

        public SqlDictionaryObject() : base()
        {
        }

        public object ID
        {
            get { return GetValue("ID"); }
            set { SetValue("ID",value);}
        }

        internal override string GetUpdateSql(string tableName)
        {
            Debug.Assert(false);
            return null;
        }

        /*
         public override T GetItem(string columnName, object columnValue, IMsSqlConnection connection)
        {
            string sql = String.Format("select * from {0} where {1} = {2}",
                _tableName,
                SafeReader.GetName(columnName),
                SafeReader.Get(columnValue));
            T res = SelectSingle(sql, connection);
            return res;
        }
         */

        internal override string GetInsertSql(string tableName)
        {
            string sqls = "";
            foreach(KeyValuePair<string,object> field in GetFields())
            {
                if (!field.Key.Contains('_'))
                {
                    object id = GetValue(this.GetIDFieldName());
                    string sql = String.Format("INSERT INTO {0} ({1},Name,Value) VALUES ('{2}','{3}',{4} );\n",
                                               SafeReader.Escape(tableName), 
                                               SafeReader.Escape(this.GetIDFieldName()),
                                               SafeReader.Escape(id.ToString()), 
                                               SafeReader.Escape(field.Key), 
                                               SafeReader.Get(field.Value.ToString()));

                    sqls += sql;
                }

            }
            return sqls;
        }

        internal override string GetDeleteSql(object id, string tableName)
        {
            string sql = String.Format("DELETE FROM {0} WHERE {1}={2}", 
                tableName,
                this.GetIDFieldName(), 
                SafeReader.Get(id));
            return sql;
        }

        internal override void ReadSqlDataReader(OleDbDataReader reader)
        {
            object name = GetColumn("Name",reader);
            object value = GetColumn("Value", reader);
            
            this.SetValue(name as string, value);
        }

        public static object GetColumn(string columnName,OleDbDataReader reader)
        {
            object res = null;
            for (int i = 0; i < reader.FieldCount; ++i)
            {
                string name = reader.GetName(i);
                if (name == columnName)
                {
                    object value = reader.GetValue(i);
                    res = value;
                    break;
                }
            }
            return res;
        }
    }
}
