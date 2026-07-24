using Microsoft.Data.SqlClient;

namespace TorayEMS.Data
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public SqlConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' tidak ditemukan di appsettings.json");
        }

        public SqlConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
