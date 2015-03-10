using System;
using System.Data.OleDb;

namespace CoreEx
{
    public interface IMsSqlConnection : IDisposable
    {
        OleDbConnection Connection { get; }

        IMsSqlConnection Open();
    }
}
