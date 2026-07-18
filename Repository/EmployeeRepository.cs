using Employee.API.Data;
using Employee.API.Helpers;
using Employee.API.Interfaces;
using Employee.API.Models; // Ensure this brings in the Employee class, not a namespace
using Microsoft.EntityFrameworkCore;

namespace Employee.API.Repository
{
    public class EmployeeRepository : GenericRepository<Employee.API.Models.Employee>,
        IEmployeeRepository
    {
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(ApplicationDbContext context, ILogger<EmployeeRepository> logger)
            : base(context)
        {
            _logger = logger;
        }
        public async Task<PagedResult<Employee.API.Models.Employee>> GetAllAsync(
    PaginationParameters pagination)
        {
            var query = _context.Employees.AsQueryable();

            int totalRecords =
                await query.CountAsync();

            var employees =
                await query
                    .Skip((pagination.PageNumber - 1)
                    * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync();

            return new PagedResult<Employee.API.Models.Employee>
            {
                Data = employees,

                TotalRecords = totalRecords,

                PageNumber = pagination.PageNumber,

                PageSize = pagination.PageSize
            };
        }
        public async Task<Employee.API.Models.Employee?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Getting employee with Id {EmployeeId}", id);
            return await _context.Employees.FindAsync(id);
        }

        public async Task<Employee.API.Models.Employee> AddAsync(Employee.API.Models.Employee employee)
        {
            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return employee;
        }

        public async Task UpdateAsync(Employee.API.Models.Employee employee)
        {
            _context.Employees.Update(employee);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee != null)
            {
                _context.Employees.Remove(employee);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Employees.AnyAsync(x => x.EmployeeId == id);
        }

    }
}