using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbWrapper<Employee> _employeeDbWrapper;

        public EmployeeRepository(IDbWrapper<Employee> employeeDbWrapper)
        {
            _employeeDbWrapper = employeeDbWrapper;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _employeeDbWrapper.FindAllAsync();
        }

        public async Task<Employee> GetByCodeAsync(string employeeCode)
        {
            var result = await _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode));
            return result?.FirstOrDefault();
        }

        public async Task<SaveResultData> SaveEmployeeAsync(Employee employee)
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

        public async Task<bool> UpdateByCodeAsync(string employeeCode, Employee employee)
        {
            var itemRepo = (await _employeeDbWrapper.FindAsync(t => t.EmployeeCode.Equals(employeeCode)))?.FirstOrDefault();
            if (itemRepo != null)
            {
                itemRepo.EmployeeName = employee.EmployeeName;
                itemRepo.Occupation = employee.Occupation;
                itemRepo.EmployeeStatus = employee.EmployeeStatus;
                itemRepo.EmailAddress = employee.EmailAddress;
                itemRepo.Phone = employee.Phone;
                itemRepo.LastModified = employee.LastModified;
                return await _employeeDbWrapper.UpdateAsync(itemRepo);
            }
            return false;
        }

        public async Task<bool> DeleteByCodeAsync(string employeeCode)
        {
            return await _employeeDbWrapper.DeleteAsync(t => t.EmployeeCode.Equals(employeeCode));
        }
    }
}

