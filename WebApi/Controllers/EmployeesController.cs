using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
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

        public EmployeesController(IEmployeeService employeeService, IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        // GET api/employees
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            var items = await _employeeService.GetAllEmployeesAsync();
            var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(items);
            return Ok(employeeDtos);
        }

        // GET api/employees/{employeeCode}
        [HttpGet, Route("{employeeCode}", Name = "GetEmployeeByCode")]
        public async Task<IHttpActionResult> Get(string employeeCode)
        {
            var item = await _employeeService.GetEmployeeByCodeAsync(employeeCode);
            if (item == null)
            {
                return NotFound();
            }
            var employeeDto = _mapper.Map<EmployeeDto>(item);
            return Ok(employeeDto);
        }

        // POST api/employees
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Request is null");
            }

            var employeeInfo = _mapper.Map<EmployeeInfo>(employeeDto);
            var result = await _employeeService.AddEmployeeAsync(employeeInfo);

            if (!result.Success)
            {
                return Content(HttpStatusCode.Conflict, result.Message);
            }

            return CreatedAtRoute("GetEmployeeByCode", new { employeeCode = employeeInfo.EmployeeCode }, employeeDto);
        }

        // PUT api/employees/{employeeCode}
        [HttpPut, Route("{employeeCode}")]
        public async Task<IHttpActionResult> Put(string employeeCode, [FromBody] EmployeeDto employeeDto)
        {
            if (employeeDto == null)
            {
                return BadRequest("Employee data is null");
            }

            var employeeInfo = _mapper.Map<EmployeeInfo>(employeeDto);
            var result = await _employeeService.UpdateEmployeeByCodeAsync(employeeCode, employeeInfo);
            if (!result)
            {
                return NotFound();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE api/employees/{employeeCode}
        [HttpDelete, Route("{employeeCode}")]
        public async Task<IHttpActionResult> Delete(string employeeCode)
        {
            var result = await _employeeService.DeleteEmployeeByCodeAsync(employeeCode);
            if (!result)
            {
                return NotFound();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}