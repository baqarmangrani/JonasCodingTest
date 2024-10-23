using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using DataAccessLayer.Repositories;
using Moq;
using Serilog;
using System.Linq.Expressions;

namespace Tests
{
    public class EmployeeRepositoryTests
    {
        private readonly Mock<IDbWrapper<Employee>> _mockDbWrapper;
        private readonly Mock<ILogger> _mockLogger;
        private readonly EmployeeRepository _repository;

        public EmployeeRepositoryTests()
        {
            _mockDbWrapper = new Mock<IDbWrapper<Employee>>();
            _mockLogger = new Mock<ILogger>();
            _repository = new EmployeeRepository(_mockDbWrapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEmployees()
        {

            var employees = new List<Employee> { new Employee { EmployeeCode = "E1" }, new Employee { EmployeeCode = "E2" } };
            _mockDbWrapper.Setup(db => db.FindAllAsync()).ReturnsAsync(employees);


            var result = await _repository.GetAllAsync();


            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnEmployee_WhenEmployeeExists()
        {

            var employeeCode = "E1";
            var employees = new List<Employee> { new Employee { EmployeeCode = employeeCode } };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(employees);


            var result = await _repository.GetByCodeAsync(employeeCode);


            Assert.NotNull(result);
            Assert.Equal(employeeCode, result.EmployeeCode);
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnNull_WhenEmployeeDoesNotExist()
        {

            var employeeCode = "E1";
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync((IEnumerable<Employee>)null);


            var result = await _repository.GetByCodeAsync(employeeCode);


            Assert.Null(result);
        }

        [Fact]
        public async Task SaveEmployeeAsync_ShouldReturnSuccess_WhenEmployeeIsSaved()
        {

            var employee = new Employee { EmployeeCode = "E1", SiteId = "S1" };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync((IEnumerable<Employee>)null);
            _mockDbWrapper.Setup(db => db.InsertAsync(employee)).ReturnsAsync(true);


            var result = await _repository.SaveEmployeeAsync(employee);


            Assert.True(result.IsSuccess);
            Assert.Equal("Employee saved successfully.", result.Message);
        }

        [Fact]
        public async Task SaveEmployeeAsync_ShouldReturnFailure_WhenEmployeeAlreadyExists()
        {

            var employee = new Employee { EmployeeCode = "E1", SiteId = "S1" };
            var existingEmployees = new List<Employee> { employee };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(existingEmployees);


            var result = await _repository.SaveEmployeeAsync(employee);


            Assert.False(result.IsSuccess);
            Assert.Equal("Employee already exists with the same code.", result.Message);
        }

        [Fact]
        public async Task UpdateByCodeAsync_ShouldReturnSuccess_WhenEmployeeIsUpdated()
        {

            var employeeCode = "E1";
            var existingEmployee = new Employee { EmployeeCode = employeeCode, SiteId = "S1" };
            var updatedEmployee = new Employee { EmployeeCode = employeeCode, SiteId = "S1", EmployeeName = "UpdatedName" };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(new List<Employee> { existingEmployee });
            _mockDbWrapper.Setup(db => db.UpdateAsync(It.IsAny<Employee>())).ReturnsAsync(true);


            var result = await _repository.UpdateByCodeAsync(employeeCode, updatedEmployee);


            Assert.True(result.IsSuccess);
            Assert.Equal("Employee updated successfully.", result.Message);
        }

        [Fact]
        public async Task DeleteByCodeAsync_ShouldReturnSuccess_WhenEmployeeIsDeleted()
        {

            var employeeCode = "E1";
            var existingEmployee = new Employee { EmployeeCode = employeeCode };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(new List<Employee> { existingEmployee });
            _mockDbWrapper.Setup(db => db.DeleteAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync(true);


            var result = await _repository.DeleteByCodeAsync(employeeCode);


            Assert.True(result);
        }

        [Fact]
        public async Task DeleteByCodeAsync_ShouldReturnFalse_WhenEmployeeDoesNotExist()
        {

            var employeeCode = "E1";
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Employee, bool>>>())).ReturnsAsync((IEnumerable<Employee>)null);


            var result = await _repository.DeleteByCodeAsync(employeeCode);


            Assert.False(result);
        }
    }
}
