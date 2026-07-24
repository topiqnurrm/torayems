using Microsoft.Data.SqlClient;
using System.Data;
using TorayEMS.Models;

namespace TorayEMS.Data
{
    /// <summary>
    /// Implementasi akses data Employee menggunakan ADO.NET murni + Stored Procedure.
    /// Dipakai bersama oleh MVC Controller (Views) maupun Web API Controller (JSON),
    /// sehingga logic akses data hanya ada di satu tempat (Single Responsibility / DRY).
    /// </summary>
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EmployeeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
            var employees = new List<Employee>();

            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Employee_GetAll", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                employees.Add(MapEmployee(reader));
            }

            return employees;
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Employee_GetById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = id });

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapEmployee(reader);
            }

            return null;
        }

        public async Task<int> CreateAsync(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Employee_Insert", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            AddCommonParameters(command, employee);

            var newIdParam = new SqlParameter("@NewEmployeeId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(newIdParam);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();

            return (int)newIdParam.Value;
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Employee_Update", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = employee.EmployeeId });
            AddCommonParameters(command, employee);

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("sp_Employee_Delete", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add(new SqlParameter("@EmployeeId", SqlDbType.Int) { Value = id });

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<int> GetTotalCountAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("SELECT COUNT(*) FROM Employees", connection);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<int> GetActiveCountAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            using var command = new SqlCommand("SELECT COUNT(*) FROM Employees WHERE IsActive = 1", connection);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        private static void AddCommonParameters(SqlCommand command, Employee employee)
        {
            command.Parameters.Add(new SqlParameter("@FullName", SqlDbType.NVarChar, 150) { Value = employee.FullName });
            command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 150) { Value = employee.Email });
            command.Parameters.Add(new SqlParameter("@Position", SqlDbType.NVarChar, 100) { Value = employee.Position });
            command.Parameters.Add(new SqlParameter("@DepartmentId", SqlDbType.Int) { Value = employee.DepartmentId });
            command.Parameters.Add(new SqlParameter("@HireDate", SqlDbType.Date) { Value = employee.HireDate });
            command.Parameters.Add(new SqlParameter("@Salary", SqlDbType.Decimal) { Value = employee.Salary });
            command.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.NVarChar, 20) { Value = (object?)employee.PhoneNumber ?? DBNull.Value });
            command.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = employee.IsActive });
        }

        private static Employee MapEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                Position = reader.GetString(reader.GetOrdinal("Position")),
                DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                DepartmentName = reader.IsDBNull(reader.GetOrdinal("DepartmentName")) ? null : reader.GetString(reader.GetOrdinal("DepartmentName")),
                HireDate = reader.GetDateTime(reader.GetOrdinal("HireDate")),
                Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            };
        }
    }
}
