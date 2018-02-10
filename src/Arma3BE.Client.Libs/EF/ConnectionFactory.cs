using System.Configuration;
using System.Data.Common;
using System.Data.SqlServerCe;

namespace Arma3BEClient.Libs.EF
{
    public class ConnectionFactory: IConnectionFactory
    {
        public DbConnection Create()
        {
            var connString = ConfigurationManager.ConnectionStrings["Arma3BEClientEntities"].ConnectionString;
            var connection =  new SqlCeConnection(connString);
            return connection;
        }
    }

    public interface IConnectionFactory
    {
        DbConnection Create();
    }
}
