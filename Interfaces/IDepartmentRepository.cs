using Employee.API.Models;

namespace Employee.API.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();

        Task<Department?> GetByIdAsync(int id);

        Task<Department> AddAsync(Department department);

        Task UpdateAsync(Department department);

        Task DeleteAsync(int id);
    }
}