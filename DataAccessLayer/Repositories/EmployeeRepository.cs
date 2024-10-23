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
            _employeeDbWrapper = employeeDbWrapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            try
            {
                return await _employeeDbWrapper.FindAllAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting all employees.");
                throw;
            }
        }

        public async Task<Employee> GetByCodeAsync(string employeeCode)
        {
            try
            {
                var result = await _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode));
                return result?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting employee by code: {EmployeeCode}", employeeCode);
                throw;
            }
        }

        public async Task<SaveResultData> SaveEmployeeAsync(Employee employee)
        {
            try
            {
                var itemRepo = (await _employeeDbWrapper.FindAsync(t =>
                    t.SiteId.Equals(employee.SiteId) && t.EmployeeCode.Equals(employee.EmployeeCode)))?.FirstOrDefault();

                if (itemRepo != null)
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
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while saving employee: {Employee}", employee);
                throw;
            }
        }

        public async Task<SaveResultData> UpdateByCodeAsync(string employeeCode, Employee employee)
        {
            try
            {
                bool updateResult = false;
                var itemRepo = (await _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)))?.FirstOrDefault();
                if (itemRepo != null)
                {
                    itemRepo.EmployeeName = employee.EmployeeName;
                    itemRepo.Occupation = employee.Occupation;
                    itemRepo.EmployeeStatus = employee.EmployeeStatus;
                    itemRepo.EmailAddress = employee.EmailAddress;
                    itemRepo.Phone = employee.Phone;
                    itemRepo.LastModified = employee.LastModified;

                    updateResult = await _employeeDbWrapper.UpdateAsync(itemRepo);
                }

                return new SaveResultData
                {
                    Success = updateResult,
                    Message = updateResult ? "Employee updated successfully." : "Failed to update employee."
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating employee by code: {EmployeeCode}", employeeCode);
                throw;
            }
        }

        public async Task<bool> DeleteByCodeAsync(string employeeCode)
        {
            try
            {
                return await _employeeDbWrapper.DeleteAsync(t => t.EmployeeCode.Equals(employeeCode));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while deleting employee by code: {EmployeeCode}", employeeCode);
                throw;
            }
        }
    }
}

