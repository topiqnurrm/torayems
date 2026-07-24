using TorayEMS.Models;

namespace TorayEMS.Data
{
    public interface IEmployeeRepository
    {
        Task<List<Employee>> GetAllAsync();
        Task<Employee?> GetByIdAsync(int id);
        Task<int> CreateAsync(Employee employee);
        Task<bool> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<int> GetTotalCountAsync();
        Task<int> GetActiveCountAsync();
    }
}
