using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Diagnostics;
using System.Threading;
using CoreEx;

namespace SQLEx
{
    public class SqlFlyTable<T> : SqlDictionaryTable<T> where T : SqlFlyObject
    {
        public SqlFlyTable(string tableName) : base(tableName) {
        }

        protected override List<T> ProcessSelect(string cmd, bool single, IMsSqlConnection connection)
        {
            List<T> res = new List<T>();

            List<T> tmp = base.ProcessSelect(cmd, single, connection);
            foreach (T i in tmp)
            {
                SqlFlyObject item = i as SqlFlyObject;
                Type itemType = AppAssembly.Inst().GetObjectType(item.CsType);
                Debug.Assert(null != itemType);
                if (itemType.IsSubclassOf(typeof(T)) || itemType == typeof(T))
                {
                    T newItem = (T)Activator.CreateInstance(itemType);
                    newItem.CopyFrom(item);
                    res.Add(newItem);
                }
            }

            return res;
        }

        protected virtual string GetSelectSql(string tableName)
        {
            string lSql = String.Format(@"select * from(
                select f.Name,v.ID, cast (v.value as nvarchar(50)) Value from dbo.Fields f,dbo.FloatDictionary v where f.id = v.Name
                union all
                select f.Name,v.ID, cast(v.value as nvarchar(50)) Value from dbo.Fields f,dbo.IntDictionary v where f.id = v.Name
                union all
                select f.Name,v.ID, cast(v.value as nvarchar(50)) Value from dbo.Fields f,dbo.StringDictionary v where f.id = v.Name 
                union all
                select f.Name,v.ID, convert(nvarchar,v.value,101) Value from dbo.Fields f,dbo.DateDictionary v where f.id = v.Name) t
                where id in
                (select * from {0})", tableName);
            return lSql;
        }

        public List<T> GetItems(IMsSqlConnection connection)
        {
            string lSql = GetSelectSql(_tableName) + "order by id";

            List<T> res = base.GetItems(lSql, connection);
            return res;
        }

        //public List<TodayExperement> GetItemsByDate(DateTime date, string tableName, MsSqlConnection connection)
        //{
        //    DateTime dateMin = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        //    DateTime dateMax = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

        //    string lSql = GetSelectSql(tableName);
        //    lSql += String.Format(@"and CreatedOn > {0} and CreatedOn < {1}",
        //                                                              SafeReader.Get(dateMin),
        //                                                              SafeReader.Get(dateMax));

        //    List<TodayExperement> res = Select(lSql, connection);
        //    return res;
        //}

        public T GetItemById(object ID,IMsSqlConnection connection)
        {
            string lSql = GetSelectSql(_tableName);
            lSql += String.Format(@"and ID = {0} order by id", SafeReader.Get(ID));

            T res = GetItem(lSql, connection);
            return res;
        }
    }
}
