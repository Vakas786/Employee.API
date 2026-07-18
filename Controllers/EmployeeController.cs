using Asp.Versioning;
using AutoMapper;
using Employee.API.DTOs;
using Employee.API.Interfaces;
using Employee.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Employee.API.Controllers
{
    //[Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableRateLimiting("fixed")]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMapper _mapper;

        public EmployeeController(
            IUnitOfWork unitOfWork,
            IMapper mapper, IBlobStorageService blobStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _blobStorageService = blobStorageService;
        }

        // ===========================================
        // GET : api/Employee
        // ===========================================

        [HttpGet]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetEmployees(
    [FromQuery] PaginationParameters pagination)
        {
            var employees =
                await _unitOfWork.Employees.GetAllAsync(pagination);

            return Ok(employees);
        }

        // ===========================================
        // GET : api/Employee/5
        // ===========================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee == null)
                return NotFound("Employee not found.");

            var employeeDto = _mapper.Map<EmployeeDto>(employee);

            return Ok(employeeDto);
        }

        // ===========================================
        // POST : api/Employee
        // ===========================================

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var employee = _mapper.Map<Models.Employee>(dto);

            var createdEmployee =
                await _unitOfWork.Employees.AddAsync(employee);

            var employeeDto =
                _mapper.Map<EmployeeDto>(createdEmployee);

            return CreatedAtAction(
                nameof(GetEmployee),
                new { id = employeeDto.EmployeeId },
                employeeDto);
        }

        // ===========================================
        // PUT : api/Employee/5
        // ===========================================

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(
            int id,
            UpdateEmployeeDto dto)
        {
            if (id != dto.EmployeeId)
                return BadRequest("Employee Id mismatch.");

            var employee =
                await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee == null)
                return NotFound("Employee not found.");

            _mapper.Map(dto, employee);

            await _unitOfWork.Employees.UpdateAsync(employee);

            return Ok("Employee updated successfully.");
        }

        // ===========================================
        // DELETE : api/Employee/5
        // ===========================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee =
                await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee == null)
                return NotFound("Employee not found.");

            await _unitOfWork.Employees.DeleteAsync(id);

            return Ok("Employee deleted successfully.");
        }



        [HttpPost("{id}/UploadPhoto")]
        public async Task<IActionResult> UploadPhoto(
    int id,
    IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Please select a file.");

            var employee =
                await _unitOfWork.Employees.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            string photoUrl =
                await _blobStorageService.UploadFileAsync(file);

            employee.PhotoUrl = photoUrl;

            await _unitOfWork.Employees.UpdateAsync(employee);

            return Ok(new
            {
                Message = "Photo uploaded successfully.",
                PhotoUrl = photoUrl
            });
        }
    }
}