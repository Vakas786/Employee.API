using Employee.API.Data;
using Employee.API.Interfaces;

namespace Employee.API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IEmployeeRepository Employees { get; }

        public IDepartmentRepository Departments { get; }

        public IAuthRepository Auth { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository,
            IAuthRepository authRepository)
        {
            _context = context;

            Employees = employeeRepository;

            Departments = departmentRepository;

            Auth = authRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}