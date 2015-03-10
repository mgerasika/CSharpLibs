using System;
using System.Data.OleDb;

namespace CoreEx
{
    public class MsSqlConnection : IDisposable, IMsSqlConnection
    {
        private OleDbConnection _connection;

        public OleDbConnection Connection
        {
            get { return _connection; }
        }

        public MsSqlConnection(string str)
        {
            _connection =
                new OleDbConnection(str);
        }

        public IMsSqlConnection Open()
        {
            _connection.Open();
            return this;
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }
    }
}
