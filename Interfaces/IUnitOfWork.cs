using Employee.API.Repository;

namespace Employee.API.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }

        IDepartmentRepository Departments { get; }

        IAuthRepository Auth { get; }

        Task<int> CompleteAsync();
    }
}