using BusinessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Model.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeInfo>> GetAllEmployeesAsync();
        Task<EmployeeInfo> GetEmployeeByCodeAsync(string employeeCode);
        Task<SaveResult> AddEmployeeAsync(EmployeeInfo employeeInfo);
        Task<bool> UpdateEmployeeByCodeAsync(string employeeCode, EmployeeInfo employeeInfo);
        Task<bool> DeleteEmployeeByCodeAsync(string employeeCode);
    }
}
