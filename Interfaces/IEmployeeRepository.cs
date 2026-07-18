using Employee.API.Helpers;
using Employee.API.Models;

namespace Employee.API.Interfaces
{
    public interface IEmployeeRepository
    {
        
        Task<PagedResult<Employee.API.Models.Employee>> GetAllAsync(
    PaginationParameters paginationParameters);
        Task<Employee.API.Models.Employee?> GetByIdAsync(int id);

        Task<Employee.API.Models.Employee> AddAsync(Employee.API.Models.Employee employee);

        Task UpdateAsync(Employee.API.Models.Employee employee);

        Task DeleteAsync(int id);

        Task<bool> ExistsAsync(int id);
    }
}