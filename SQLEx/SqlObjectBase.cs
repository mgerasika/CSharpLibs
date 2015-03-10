using System.Collections.Generic;
using System.Data.OleDb;
using CoreEx;

namespace SQLEx
{
    public abstract class SqlObjectBase : ObjectEx
    {
        internal abstract string GetUpdateSql(string tableName);

        internal abstract string GetInsertSql(string tableName);

        internal abstract string GetDeleteSql(object id, string tableName);

        internal abstract void ReadSqlDataReader(OleDbDataReader reader);

        public void CopyFrom(ObjectEx target)
        {
            foreach (KeyValuePair<string, object> kv in target.GetFields())
            {
                this.SetValue(kv.Key, kv.Value);
            }
        }
    }
}