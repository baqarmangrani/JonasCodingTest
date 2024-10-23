using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbWrapper<Employee> _employeeDbWrapper;
        private readonly ILogger _logger;

        public EmployeeRepository(IDbWrapper<Employee> employeeDbWrapper, ILogger logger)
        {
            _employeeDbWrapper = employeeDbWrapper ?? throw new ArgumentNullException(nameof(employeeDbWrapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await ExecuteDbOperationAsync(() => _employeeDbWrapper.FindAllAsync(), "getting all employees");
        }

        public async Task<Employee> GetByCodeAsync(string employeeCode)
        {
            return (await ExecuteDbOperationAsync(() => _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)),
                $"getting employee by code: {employeeCode}"))?.FirstOrDefault();
        }

        public async Task<SaveResultData> SaveEmployeeAsync(Employee employee)
        {
            return await ExecuteDbOperationAsync(async () =>
            {
                var existingEmployee = (await _employeeDbWrapper.FindAsync(t =>
                    t.SiteId.Equals(employee.SiteId) && t.EmployeeCode.Equals(employee.EmployeeCode)))?.FirstOrDefault();

                if (existingEmployee != null)
                {
                    return new SaveResultData
                    {
                        Success = false,
                        Message = "Employee already exists with the same code."
                    };
                }

                var insertResult = await _employeeDbWrapper.InsertAsync(employee);
                return new SaveResultData
                {
                    Success = insertResult,
                    Message = insertResult ? "Employee saved successfully." : "Failed to save employee."
                };
            }, $"saving employee: {employee}");
        }

        public async Task<SaveResultData> UpdateByCodeAsync(string employeeCode, Employee employee)
        {
            return await ExecuteDbOperationAsync(async () =>
            {
                var existingEmployee = (await _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)))?.FirstOrDefault();
                if (existingEmployee == null)
                {
                    return new SaveResultData
                    {
                        Success = false,
                        Message = "Employee not found."
                    };
                }

                UpdateEmployeeDetails(existingEmployee, employee);
                var updateResult = await _employeeDbWrapper.UpdateAsync(existingEmployee);

                return new SaveResultData
                {
                    Success = updateResult,
                    Message = updateResult ? "Employee updated successfully." : "Failed to update employee."
                };
            }, $"updating employee by code: {employeeCode}");
        }

        public async Task<bool> DeleteByCodeAsync(string employeeCode)
        {
            return await ExecuteDbOperationAsync(() => _employeeDbWrapper.DeleteAsync(t => t.EmployeeCode.Equals(employeeCode)),
                $"deleting employee by code: {employeeCode}");
        }

        private async Task<T> ExecuteDbOperationAsync<T>(Func<Task<T>> dbOperation, string operationDescription)
        {
            try
            {
                return await dbOperation();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while {operationDescription}.");
                throw;
            }
        }

        private void UpdateEmployeeDetails(Employee existingEmployee, Employee newEmployee)
        {
            existingEmployee.EmployeeName = newEmployee.EmployeeName;
            existingEmployee.Occupation = newEmployee.Occupation;
            existingEmployee.EmployeeStatus = newEmployee.EmployeeStatus;
            existingEmployee.EmailAddress = newEmployee.EmailAddress;
            existingEmployee.Phone = newEmployee.Phone;
            existingEmployee.LastModified = newEmployee.LastModified;
        }
    }
}

