using Microsoft.Data.SqlClient;

namespace TorayEMS.Data
{
    /// <summary>
    /// Abstraksi untuk membuat koneksi ke SQL Server.
    /// Memudahkan unit testing dan mengikuti prinsip Dependency Inversion.
    /// </summary>
    public interface IDbConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
