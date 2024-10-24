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
            return await ExecuteDbOperationAsync(() => RetryAsync(() => _employeeDbWrapper.FindAllAsync()), "Error occurred while getting all employees.");
        }

        public async Task<Employee> GetByCodeAsync(string employeeCode)
        {
            var employees = await ExecuteDbOperationAsync(() => RetryAsync(() => _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode))),
                $"Error occurred while getting employee by code: {employeeCode}");
            return employees?.FirstOrDefault();
        }

        public async Task<ResultData> SaveEmployeeAsync(Employee employee)
        {
            return await ExecuteDbOperationAsync(async () =>
            {
                var existingEmployee = (await RetryAsync(() => _employeeDbWrapper.FindAsync(t => t.SiteId.Equals(employee.SiteId) && t.EmployeeCode.Equals(employee.EmployeeCode))))?.FirstOrDefault();
                if (existingEmployee != null)
                {
                    return new ResultData { IsSuccess = false, Message = "Employee already exists with the same code." };
                }

                var insertResult = await RetryAsync(() => _employeeDbWrapper.InsertAsync(employee));
                return new ResultData { IsSuccess = insertResult, Message = insertResult ? "Employee saved successfully." : "Failed to save employee." };
            }, $"Error occurred while saving employee: {employee}");
        }

        public async Task<ResultData> UpdateByCodeAsync(string employeeCode, Employee employee)
        {
            return await ExecuteDbOperationAsync(async () =>
            {
                var existingEmployee = (await RetryAsync(() => _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode))))?.FirstOrDefault();
                if (existingEmployee == null)
                {
                    return new ResultData { IsSuccess = false, Message = "Employee not found." };
                }

                UpdateEmployeeDetails(existingEmployee, employee);
                var updateResult = await RetryAsync(() => _employeeDbWrapper.UpdateAsync(existingEmployee));

                return new ResultData { IsSuccess = updateResult, Message = updateResult ? "Employee updated successfully." : "Failed to update employee." };
            }, $"Error occurred while updating employee by code: {employeeCode}");
        }

        public async Task<bool> DeleteByCodeAsync(string employeeCode)
        {
            return await ExecuteDbOperationAsync(async () =>
            {
                var existingEmployee = (await RetryAsync(() => _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode))))?.FirstOrDefault();
                if (existingEmployee == null)
                {
                    _logger.Warning($"Employee with code {employeeCode} not found for deletion.");
                    return false;
                }

                var deleteResult = await RetryAsync(() => _employeeDbWrapper.DeleteAsync(t => t.EmployeeCode.Equals(employeeCode)));
                if (!deleteResult)
                {
                    _logger.Error($"Failed to delete employee with code {employeeCode}.");
                }

                return deleteResult;
            }, $"Error occurred while deleting employee by code: {employeeCode}");
        }

        private async Task<T> ExecuteDbOperationAsync<T>(Func<Task<T>> dbOperation, string errorMessage)
        {
            try
            {
                return await dbOperation();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, errorMessage);
                throw;
            }
        }

        private async Task<IEnumerable<T>> RetryAsync<T>(Func<Task<IEnumerable<T>>> operation, int maxRetries = 3, int delayMilliseconds = 1000)
        {
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                var result = await operation();
                if (result != null)
                {
                    return result;
                }
                await Task.Delay(delayMilliseconds);
            }
            return null;
        }

        private async Task<bool> RetryAsync(Func<Task<bool>> operation, int maxRetries = 3, int delayMilliseconds = 1000)
        {
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                if (await operation())
                {
                    return true;
                }
                await Task.Delay(delayMilliseconds);
            }
            return false;
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
