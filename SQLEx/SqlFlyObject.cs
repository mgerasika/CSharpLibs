using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SQLEx
{
    public class SqlFlyObject : SqlDictionaryObject
    {
        public SqlFlyObject()
        {
            this.CsType = this.GetType().FullName;
        }
        
        public string CsType
        {
            get { return GetValueStr("CsType"); }
            set { SetValue("CsType", value); }
        }

        public override string GetIDFieldName()
        {
            return "ID";
        }

        public virtual string GetFieldId(string name)
        {
            Debug.Assert(false);
            return name;
        }

        internal override string GetInsertSql(string tableName)
        {
            string lRes = "";

            foreach (KeyValuePair<string, object> field in GetFields())
            {
                if (!field.Key.Contains('_'))
                {
                    string sqlTableName = GetSqlTableName(tableName,field);
                    string sql = "";
                    if (field.Key != "ID")
                    {
                        string fieldId = GetFieldId(field.Key);
                        sql = String.Format("INSERT INTO {0} (ID,Name,Value) VALUES ({1},{2},{3} )",
                                            sqlTableName,
                                            SafeReader.Get(this.ID),
                                            fieldId,
                                            SafeReader.Get(field.Value));
                    }
                    else
                    {
                        sql = String.Format("INSERT INTO {0} (ID) VALUES ({1})",
                                              sqlTableName,
                                              SafeReader.Get(this.ID));
                    }
                    lRes += sql;
                }

            }
            return lRes;
        }

        private string GetSqlTableName(string tableName,KeyValuePair<string, object> field)
        {
            string lRes = "dbo.StringDictionary";
            if (field.Value is DateTime)
            {
                lRes = "dbo.DateDictionary";
            }
            else if (field.Value is float)
            {
                lRes = "dbo.FloatDictionary";
            }
            else if (field.Value is int)
            {
                lRes = "dbo.IntDictionary";
            }
            else if(field.Key == "ID")
            {
                lRes = tableName;
            }
            return lRes;
        }
    }
}
