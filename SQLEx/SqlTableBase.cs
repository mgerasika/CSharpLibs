using System;
using System.Collections.Generic;
using System.Data.OleDb;
using CoreEx;

namespace SQLEx
{
    public interface ISqlTable<T> 
    {
        void AddOrUpdate(T item, IMsSqlConnection connection);

        void Add(T item, IMsSqlConnection connection);

        void Update(T item, IMsSqlConnection connection);

        void Remove(T item, IMsSqlConnection connection);

        T GetItem(string columnName, object columnValue, IMsSqlConnection connection);

        T GetItem(string sql, IMsSqlConnection connection);

        List<T> GetItems(string columnName, object columnValue, IMsSqlConnection connection);

        List<T> GetItems(string sql, IMsSqlConnection connection);

        List<T> GetItemsByDate(string columnName, object columnValue, DateTime date, IMsSqlConnection connection);

        List<T> GetItemsFromDate(string columnName, object columnValue, DateTime dateMin, DateTime dateMax,
            IMsSqlConnection connection);

        List<T> GetAllItems(IMsSqlConnection connection);
    }

    public abstract class SqlTableBase<T> : ISqlTable<T> where T : SqlObjectBase
    {
        protected string _tableName;

        internal SqlTableBase(string tableName)
        {
            _tableName = tableName;
        }

        public void Add(T item, IMsSqlConnection connection)
        {
            string sql = item.GetInsertSql(_tableName);
            if (!string.IsNullOrEmpty(sql))
            {
                using (connection.Open())
                {
                    OleDbCommand command = new OleDbCommand(sql);
                    command.Connection = connection.Connection;
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                    }
                }
            }
        }

        public void Update(T item, IMsSqlConnection connection)
        {
            string sql = item.GetUpdateSql(_tableName);
            if (!string.IsNullOrEmpty(sql))
            {
                using (connection.Open())
                {
                    OleDbCommand command = new OleDbCommand(sql);
                    command.Connection = connection.Connection;
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                    }
                }
            }
        }

        public void AddOrUpdate(T item, IMsSqlConnection connection)
        {
            if (null == GetItem("ID", item.GetValue("ID"), connection))
            {
                Add(item, connection);
            }
            else
            {
                Update(item, connection);
            }
        }

        public void Remove(T item, IMsSqlConnection connection)
        {
            string sql = item.GetDeleteSql(item.GetValue("ID"),_tableName);
            if (!string.IsNullOrEmpty(sql))
            {
                using (connection.Open())
                {
                    OleDbCommand command = new OleDbCommand(sql);
                    command.Connection = connection.Connection;
                    using (OleDbDataReader reader = command.ExecuteReader())
                    {
                    }
                }
            }
        }

        public T GetItem(string columnName, object columnValue, IMsSqlConnection connection)
        {
            string sql = String.Format("select * from {0} where {1} = {2}",
                _tableName,
                columnName,
                SafeReader.Get(columnValue));

            T res = GetItem(sql, connection);
            return res;
        }

        public T GetItem(string sql, IMsSqlConnection connection)
        {
            List<T> items = ProcessSelect(sql, true, connection);
            return items.Count > 0 ? (T)items[0] : null;
        }

        public List<T> GetItems(string columnName, object columnValue, IMsSqlConnection connection)
        {
            string sql = String.Format("select * from {0} where {1} = {2}",
                _tableName,
                columnName,
                SafeReader.Get(columnValue));
            List<T> list = ProcessSelect(sql, false, connection);
            return list;
        }

        public List<T> GetItems(string sql, IMsSqlConnection connection)
        {
            return ProcessSelect(sql, false, connection);
        }

        public List<T> GetItemsByDate(string columnName, object columnValue, DateTime date, IMsSqlConnection connection)
        {
            DateTime dateMin = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime dateMax = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);

            return GetItemsFromDate(columnName, columnValue, dateMin, dateMax, connection);
        }

        public List<T> GetItemsFromDate(string columnName, object columnValue, DateTime dateMin, DateTime dateMax, IMsSqlConnection connection)
        {
            string sql = String.Format("select * from {0} where {1} = {2} and Date > {3} and Date < {4}",
                _tableName,
                columnName,
                SafeReader.Get(columnValue),
                SafeReader.Get(dateMin),
                SafeReader.Get(dateMax));
            List<T> list = ProcessSelect(sql, false, connection);
            return list;
        }

        public List<T> GetAllItems(IMsSqlConnection connection)
        {
            string sql = String.Format("select * from {0}", _tableName);
            List<T> list = ProcessSelect(sql, false, connection);
            return list;
        }

        public void Dispose()
        {
            
        }

        protected abstract List<T> ProcessSelect(string sql, bool single, IMsSqlConnection connection);
    }
}