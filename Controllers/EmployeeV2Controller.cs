using Asp.Versioning;
using AutoMapper;
using Employee.API.DTOs;
using Employee.API.Interfaces;
using Employee.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee.API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class EmployeeV2Controller : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMapper _mapper;

        public EmployeeV2Controller(
            IEmployeeRepository employeeRepository,
            IMapper mapper, IBlobStorageService blobStorageService)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _blobStorageService = blobStorageService;
        }

        // ===========================================
        // GET : api/Employee
        // ===========================================

        [HttpGet]
        public async Task<IActionResult> GetEmployees(
     [FromQuery] PaginationParameters pagination)
        {
            var employees =
                await _employeeRepository.GetAllAsync(pagination);

            return Ok(employees);
        }

        // ===========================================
        // GET : api/Employee/5
        // ===========================================

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

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
                await _employeeRepository.AddAsync(employee);

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
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound("Employee not found.");

            _mapper.Map(dto, employee);

            await _employeeRepository.UpdateAsync(employee);

            return Ok("Employee updated successfully.");
        }

        // ===========================================
        // DELETE : api/Employee/5
        // ===========================================

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee =
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound("Employee not found.");

            await _employeeRepository.DeleteAsync(id);

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
                await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            string photoUrl =
                await _blobStorageService.UploadFileAsync(file);

            employee.PhotoUrl = photoUrl;

            await _employeeRepository.UpdateAsync(employee);

            return Ok(new
            {
                Message = "Photo uploaded successfully.",
                PhotoUrl = photoUrl
            });
        }
    }
}