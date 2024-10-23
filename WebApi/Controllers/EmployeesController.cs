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
            _employeeService = employeeService;
            _mapper = mapper;
            _logger = logger;
        }

        // GET api/employees
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var items = await _employeeService.GetAllEmployeesAsync();
                var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(items);
                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting all employees.");
                return InternalServerError(new Exception("An error occurred while processing your request."));
            }
        }

        // GET api/employees/{employeeCode}
        [HttpGet, Route("{employeeCode}", Name = "GetEmployeeByCode")]
        public async Task<IHttpActionResult> Get(string employeeCode)
        {
            try
            {
                var item = await _employeeService.GetEmployeeByCodeAsync(employeeCode);
                if (item == null)
                {
                    return NotFound();
                }
                var employeeDto = _mapper.Map<EmployeeDto>(item);
                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while getting employee with code {employeeCode}.");
                return InternalServerError(new Exception("An error occurred while processing your request."));
            }
        }

        // POST api/employees
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Request is null");
            }

            try
            {
                var employeeInfo = _mapper.Map<EmployeeInfo>(employeeDto);
                var result = await _employeeService.AddEmployeeAsync(employeeInfo);

                if (!result.IsSuccess)
                {
                    return Content(HttpStatusCode.Conflict, result.Message);
                }

                return CreatedAtRoute("GetEmployeeByCode", new { employeeCode = employeeInfo.EmployeeCode }, employeeDto);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while adding a new employee.");
                return InternalServerError(new Exception("An error occurred while processing your request."));
            }
        }

        // PUT api/employees/{employeeCode}
        [HttpPut, Route("{employeeCode}")]
        public async Task<IHttpActionResult> Put(string employeeCode, [FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Employee data is null");
            }

            try
            {
                var employeeInfo = _mapper.Map<EmployeeInfo>(employeeDto);
                var result = await _employeeService.UpdateEmployeeByCodeAsync(employeeCode, employeeInfo);

                if (!result.IsSuccess)
                {
                    return Content(HttpStatusCode.NotFound, result.Message);
                }

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while updating employee with code {employeeCode}.");
                return InternalServerError(new Exception("An error occurred while processing your request."));
            }
        }

        // DELETE api/employees/{employeeCode}
        [HttpDelete, Route("{employeeCode}")]
        public async Task<IHttpActionResult> Delete(string employeeCode)
        {
            try
            {
                var result = await _employeeService.DeleteEmployeeByCodeAsync(employeeCode);
                if (!result)
                {
                    return NotFound();
                }
                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while deleting employee with code {employeeCode}.");
                return InternalServerError(new Exception("An error occurred while processing your request."));
            }
        }
    }
}
