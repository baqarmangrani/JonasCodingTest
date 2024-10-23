using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using Moq;
using Serilog;
using System.Net;
using System.Web.Http.Results;
using WebApi.Controllers;
using WebApi.Models;

namespace Tests
{
    public class EmployeesControllerTests
    {
        private readonly Mock<IEmployeeService> _mockEmployeeService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger> _mockLogger;
        private readonly EmployeesController _controller;

        public EmployeesControllerTests()
        {
            _mockEmployeeService = new Mock<IEmployeeService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger>();
            _controller = new EmployeesController(_mockEmployeeService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllEmployees()
        {

            var employees = new List<EmployeeInfo> { new EmployeeInfo { EmployeeCode = "E1" }, new EmployeeInfo { EmployeeCode = "E2" } };
            var employeeDtos = new List<EmployeeDto> { new EmployeeDto { EmployeeCode = "E1" }, new EmployeeDto { EmployeeCode = "E2" } };

            _mockEmployeeService.Setup(service => service.GetAllEmployeesAsync()).ReturnsAsync(employees);
            _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeDto>>(employees)).Returns(employeeDtos);


            var result = await _controller.GetAll() as OkNegotiatedContentResult<IEnumerable<EmployeeDto>>;


            Assert.NotNull(result);
            Assert.Equal(2, result.Content.Count());
        }

        [Fact]
        public async Task Get_ShouldReturnEmployee_WhenEmployeeExists()
        {

            var employeeCode = "E1";
            var employee = new EmployeeInfo { EmployeeCode = employeeCode };
            var employeeDto = new EmployeeDto { EmployeeCode = employeeCode };

            _mockEmployeeService.Setup(service => service.GetEmployeeByCodeAsync(employeeCode)).ReturnsAsync(employee);
            _mockMapper.Setup(m => m.Map<EmployeeDto>(employee)).Returns(employeeDto);


            var result = await _controller.Get(employeeCode) as OkNegotiatedContentResult<EmployeeDto>;


            Assert.NotNull(result);
            Assert.Equal(employeeCode, result.Content.EmployeeCode);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
        {

            var employeeCode = "E1";
            _mockEmployeeService.Setup(service => service.GetEmployeeByCodeAsync(employeeCode)).ReturnsAsync((EmployeeInfo)null);


            var result = await _controller.Get(employeeCode);


            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_ShouldReturnCreated_WhenEmployeeIsAdded()
        {

            var employeeDto = new EmployeeDto { EmployeeCode = "E1", EmployeeName = "Employee1" };
            var employeeInfo = new EmployeeInfo { EmployeeCode = "E1", EmployeeName = "Employee1" };
            var result = new Result(true, "Employee added successfully.");

            _mockMapper.Setup(m => m.Map<EmployeeInfo>(employeeDto)).Returns(employeeInfo);
            _mockEmployeeService.Setup(service => service.AddEmployeeAsync(employeeInfo)).ReturnsAsync(result);


            var response = await _controller.Post(employeeDto) as CreatedAtRouteNegotiatedContentResult<EmployeeDto>;


            Assert.NotNull(response);
            Assert.Equal("GetEmployeeByCode", response.RouteName);
            Assert.Equal(employeeDto.EmployeeCode, response.RouteValues["employeeCode"]);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenEmployeeDtoIsNull()
        {

            var result = await _controller.Post(null) as BadRequestErrorMessageResult;


            Assert.NotNull(result);
            Assert.Equal("Request is null", result.Message);
        }

        [Fact]
        public async Task Put_ShouldReturnNoContent_WhenEmployeeIsUpdated()
        {

            var employeeCode = "E1";
            var employeeDto = new EmployeeDto { EmployeeCode = employeeCode, EmployeeName = "Employee1" };
            var employeeInfo = new EmployeeInfo { EmployeeCode = employeeCode, EmployeeName = "Employee1" };
            var result = new Result(true, "Employee updated successfully.");

            _mockMapper.Setup(m => m.Map<EmployeeInfo>(employeeDto)).Returns(employeeInfo);
            _mockEmployeeService.Setup(service => service.UpdateEmployeeByCodeAsync(employeeCode, employeeInfo)).ReturnsAsync(result);


            var response = await _controller.Put(employeeCode, employeeDto) as StatusCodeResult;


            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenEmployeeDtoIsNull()
        {

            var result = await _controller.Put("E1", null) as BadRequestErrorMessageResult;


            Assert.NotNull(result);
            Assert.Equal("Employee data is null", result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenEmployeeDoesNotExist()
        {

            var employeeCode = "E1";
            var result = new Result(false, "Employee not found.");

            _mockEmployeeService.Setup(service => service.DeleteEmployeeByCodeAsync(employeeCode)).ReturnsAsync(result);


            var response = await _controller.Delete(employeeCode) as NegotiatedContentResult<string>;


            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("Employee not found.", response.Content);
        }
    }
}
