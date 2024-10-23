using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    [RoutePrefix("api/employees")]
    public class EmployeesController : ApiController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public EmployeesController(IEmployeeService employeeService, IMapper mapper, ILogger logger)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/employees
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var items = await _employeeService.GetAllEmployeesAsync();
                var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(items);
                return Ok(employeeDtos);
            }, "Error occurred while getting all employees.");
        }

        // GET api/employees/{employeeCode}
        [HttpGet, Route("{employeeCode}", Name = "GetEmployeeByCode")]
        public async Task<IHttpActionResult> Get(string employeeCode)
        {
            return await ExecuteAsync(async () =>
            {
                var item = await _employeeService.GetEmployeeByCodeAsync(employeeCode);
                if (item == null)
                {
                    _logger.Warning($"Employee with code {employeeCode} not found.");
                    return NotFound();
                }
                var employeeDto = _mapper.Map<EmployeeDto>(item);
                return Ok(employeeDto);
            }, $"Error occurred while getting employee with code {employeeCode}.");
        }

        // POST api/employees
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Request is null");
            }

            return await ExecuteAsync(async () =>
            {
                var employeeInfo = _mapper.Map<EmployeeInfo>(employeeDto);
                var result = await _employeeService.AddEmployeeAsync(employeeInfo);

                if (!result.IsSuccess)
                {
                    _logger.Warning($"Failed to add employee: {result.Message}");
                    return Content(HttpStatusCode.Conflict, result.Message);
                }

                return CreatedAtRoute("GetEmployeeByCode", new { employeeCode = employeeInfo.EmployeeCode }, employeeDto);
            }, "Error occurred while adding a new employee.");
        }

        // PUT api/employees/{employeeCode}
        [HttpPut, Route("{employeeCode}")]
        public async Task<IHttpActionResult> Put(string employeeCode, [FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Employee data is null");
            }

            return await ExecuteAsync(async () =>
            {
                var employeeInfo = _mapper.Map<EmployeeInfo>(employeeDto);
                var result = await _employeeService.UpdateEmployeeByCodeAsync(employeeCode, employeeInfo);

                if (!result.IsSuccess)
                {
                    _logger.Warning($"Failed to update employee with code {employeeCode}: {result.Message}");
                    return Content(HttpStatusCode.NotFound, result.Message);
                }

                return StatusCode(HttpStatusCode.NoContent);
            }, $"Error occurred while updating employee with code {employeeCode}.");
        }

        // DELETE api/employees/{employeeCode}
        [HttpDelete, Route("{employeeCode}")]
        public async Task<IHttpActionResult> Delete(string employeeCode)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _employeeService.DeleteEmployeeByCodeAsync(employeeCode);

                if (!result.IsSuccess)
                {
                    _logger.Warning($"Employee with code {employeeCode} not found for deletion.");
                    return Content(HttpStatusCode.NotFound, result.Message);
                }
                return Ok(new { Message = result.Message });
            }, $"Error occurred while deleting employee with code {employeeCode}.");
        }

        private async Task<IHttpActionResult> ExecuteAsync(Func<Task<IHttpActionResult>> action, string errorMessage)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, errorMessage);
                return InternalServerError(new Exception("An error occurred while processing your request."));
            }
        }
    }
}
