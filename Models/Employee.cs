using System.ComponentModel.DataAnnotations;

namespace Employee.API.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string EmployeeCode { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Department { get; set; }

        public decimal Salary { get; set; }

        public bool IsActive { get; set; }

        public string? PhotoUrl { get; set; }
    }
}