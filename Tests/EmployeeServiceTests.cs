using AutoMapper;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Moq;
using Serilog;

namespace Tests
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _mockEmployeeRepository;
        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger> _mockLogger;
        private readonly EmployeeService _employeeService;

        public EmployeeServiceTests()
        {
            _mockEmployeeRepository = new Mock<IEmployeeRepository>();
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger>();
            _employeeService = new EmployeeService(_mockEmployeeRepository.Object, _mockCompanyRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllEmployeesAsync_ShouldReturnAllEmployees()
        {

            var employees = new List<Employee> { new Employee { EmployeeCode = "E1", CompanyCode = "C1" }, new Employee { EmployeeCode = "E2", CompanyCode = "C2" } };
            var employeeInfos = new List<EmployeeInfo> { new EmployeeInfo { EmployeeCode = "E1" }, new EmployeeInfo { EmployeeCode = "E2" } };
            var companies = new List<Company> { new Company { CompanyCode = "C1", CompanyName = "Company1" }, new Company { CompanyCode = "C2", CompanyName = "Company2" } };

            _mockEmployeeRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(employees);
            _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeInfo>>(employees)).Returns(employeeInfos);
            _mockCompanyRepository.Setup(repo => repo.GetByCodeAsync(It.IsAny<string>())).ReturnsAsync((string code) => companies.FirstOrDefault(c => c.CompanyCode == code));


            var result = await _employeeService.GetAllEmployeesAsync();


            Assert.Equal(2, result.Count());
            Assert.Equal("Company1", result.First().CompanyName);
        }

        [Fact]
        public async Task GetEmployeeByCodeAsync_ShouldReturnEmployee_WhenEmployeeExists()
        {

            var employeeCode = "E1";
            var employee = new Employee { EmployeeCode = employeeCode, CompanyCode = "C1" };
            var employeeInfo = new EmployeeInfo { EmployeeCode = employeeCode };
            var company = new Company { CompanyCode = "C1", CompanyName = "Company1" };

            _mockEmployeeRepository.Setup(repo => repo.GetByCodeAsync(employeeCode)).ReturnsAsync(employee);
            _mockMapper.Setup(m => m.Map<EmployeeInfo>(employee)).Returns(employeeInfo);
            _mockCompanyRepository.Setup(repo => repo.GetByCodeAsync("C1")).ReturnsAsync(company);


            var result = await _employeeService.GetEmployeeByCodeAsync(employeeCode);


            Assert.NotNull(result);
            Assert.Equal(employeeCode, result.EmployeeCode);
            Assert.Equal("Company1", result.CompanyName);
        }

        [Fact]
        public async Task GetEmployeeByCodeAsync_ShouldReturnNull_WhenEmployeeDoesNotExist()
        {

            var employeeCode = "E1";
            _mockEmployeeRepository.Setup(repo => repo.GetByCodeAsync(employeeCode)).ReturnsAsync((Employee)null);


            var result = await _employeeService.GetEmployeeByCodeAsync(employeeCode);


            Assert.Null(result);
        }

        [Fact]
        public async Task AddEmployeeAsync_ShouldReturnSuccess_WhenEmployeeIsAdded()
        {

            var employeeInfo = new EmployeeInfo { EmployeeCode = "E1", CompanyName = "Company1" };
            var employee = new Employee { EmployeeCode = "E1" };
            var company = new Company { CompanyCode = "C1", CompanyName = "Company1" };
            var resultData = new ResultData { IsSuccess = true, Message = "Employee added successfully." };

            _mockCompanyRepository.Setup(repo => repo.GetByNameAsync("Company1")).ReturnsAsync(company);
            _mockMapper.Setup(m => m.Map<Employee>(employeeInfo)).Returns(employee);
            _mockEmployeeRepository.Setup(repo => repo.SaveEmployeeAsync(employee)).ReturnsAsync(resultData);


            var result = await _employeeService.AddEmployeeAsync(employeeInfo);


            Assert.True(result.IsSuccess);
            Assert.Equal("Employee added successfully.", result.Message);
        }

        [Fact]
        public async Task UpdateEmployeeByCodeAsync_ShouldReturnSuccess_WhenEmployeeIsUpdated()
        {

            var employeeCode = "E1";
            var employeeInfo = new EmployeeInfo { EmployeeCode = employeeCode, CompanyName = "Company1" };
            var employee = new Employee { EmployeeCode = employeeCode };
            var company = new Company { CompanyCode = "C1", CompanyName = "Company1" };
            var resultData = new ResultData { IsSuccess = true, Message = "Employee updated successfully." };

            _mockCompanyRepository.Setup(repo => repo.GetByNameAsync("Company1")).ReturnsAsync(company);
            _mockMapper.Setup(m => m.Map<Employee>(employeeInfo)).Returns(employee);
            _mockEmployeeRepository.Setup(repo => repo.UpdateByCodeAsync(employeeCode, employee)).ReturnsAsync(resultData);


            var result = await _employeeService.UpdateEmployeeByCodeAsync(employeeCode, employeeInfo);


            Assert.True(result.IsSuccess);
            Assert.Equal("Employee updated successfully.", result.Message);
        }

        [Fact]
        public async Task DeleteEmployeeByCodeAsync_ShouldReturnSuccess_WhenEmployeeIsDeleted()
        {

            var employeeCode = "E1";
            _mockEmployeeRepository.Setup(repo => repo.DeleteByCodeAsync(employeeCode)).ReturnsAsync(true);


            var result = await _employeeService.DeleteEmployeeByCodeAsync(employeeCode);


            Assert.True(result.IsSuccess);
            Assert.Equal($"Company with code {employeeCode} was successfully deleted.", result.Message);
        }

        [Fact]
        public async Task DeleteEmployeeByCodeAsync_ShouldReturnFailure_WhenEmployeeDoesNotExist()
        {

            var employeeCode = "E1";
            _mockEmployeeRepository.Setup(repo => repo.DeleteByCodeAsync(employeeCode)).ReturnsAsync(false);


            var result = await _employeeService.DeleteEmployeeByCodeAsync(employeeCode);


            Assert.False(result.IsSuccess);
            Assert.Equal("Company not found.", result.Message);
        }
    }
}
