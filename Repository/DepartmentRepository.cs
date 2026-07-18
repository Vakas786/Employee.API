using Employee.API.Data;
using Employee.API.Interfaces;
using Employee.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Employee.API.Repository
{
    public class DepartmentRepository(ApplicationDbContext context) : GenericRepository<Department>(context), IDepartmentRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<IEnumerable<Employee.API.Models.Department>> GetAllAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Employee.API.Models.Department?> GetByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<Employee.API.Models.Department> AddAsync(Employee.API.Models.Department employee)
        {
            _context.Departments.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task UpdateAsync(Employee.API.Models.Department employee)
        {
            _context.Departments.Update(employee);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Departments.FindAsync(id);

            if (employee != null)
            {
                _context.Departments.Remove(employee);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(x => x.EmployeeId == id);
        }
    }
}
