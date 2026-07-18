using System.ComponentModel.DataAnnotations;

namespace Employee.API.DTOs
{
    public class UpdateEmployeeDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public string EmployeeCode { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public bool IsActive { get; set; }
    }
}