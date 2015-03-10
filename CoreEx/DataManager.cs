using System.Configuration;

namespace CoreEx
{
    public abstract class DataManagerBase
    {
        public DataManagerBase()
        {
            
        }
        private IMsSqlConnection _connection;
        public IMsSqlConnection Connection
        {
            get
            {
                if(null == _connection)
                {
                    string str = "";
                    ConnectionStringSettings conn = ConfigurationManager.ConnectionStrings["dbConnectionString"];
                    if (null != conn)
                    {
                        str =conn.ConnectionString;
                    }
                    _connection = CreateMsSqlConnection(str); 
                }
                return _connection;
            }
        }

        protected virtual IMsSqlConnection CreateMsSqlConnection(string str)
        {
            IMsSqlConnection sqlConnection = new MsSqlConnection(str);
            return sqlConnection;
        }
    }
}
