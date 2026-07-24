using Microsoft.Data.SqlClient;
using System.Data;
using TorayEMS.Models;

namespace TorayEMS.Data
{
    /// <summary>
    /// Implementasi akses data Department menggunakan ADO.NET murni + Stored Procedure.
    /// Sengaja tidak memakai Entity Framework agar sesuai requirement pekerjaan:
    /// "Manage and optimize SQL Server databases and Stored Procedures".
    /// </summary>
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DepartmentRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            var departments = new List<Department>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Department_GetAll", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                departments.Add(MapDepartment(reader));
            }

            return departments;
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Department_GetById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int) { Value = id });

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapDepartment(reader);
            }

            return null;
        }

        public async Task<int> CreateAsync(Department department)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Department_Insert", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 100) { Value = department.DepartmentName });
            command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255) { Value = (object?)department.Description ?? DBNull.Value });

            var newIdParam = new SqlParameter("@NewDepartmentId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(newIdParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return (int)newIdParam.Value;
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Department_Update", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int) { Value = department.DepartmentId });
            command.Parameters.Add(new SqlParameter("@DepartmentName", SqlDbType.NVarChar, 100) { Value = department.DepartmentName });
            command.Parameters.Add(new SqlParameter("@Description", SqlDbType.NVarChar, 255) { Value = (object?)department.Description ?? DBNull.Value });

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Department_Delete", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int) { Value = id });

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<List<Department>> GetEmployeeCountByDepartmentAsync()
        {
            var result = new List<Department>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Dashboard_EmployeeCountByDepartment", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                result.Add(new Department
                {
                    DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                    DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                    EmployeeCount = reader.GetInt32(reader.GetOrdinal("EmployeeCount"))
                });
            }

            return result;
        }

        private static Department MapDepartment(SqlDataReader reader)
        {
            return new Department
            {
                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? null : reader.GetString(reader.GetOrdinal("Description"))
            };
        }
    }
}
