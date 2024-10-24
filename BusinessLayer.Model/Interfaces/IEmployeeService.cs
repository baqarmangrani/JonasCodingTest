using BusinessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Model.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeInfo>> GetAllEmployeesAsync();
        Task<EmployeeInfo> GetEmployeeByCodeAsync(string employeeCode);
        Task<Result> AddEmployeeAsync(EmployeeInfo employeeInfo);
        Task<Result> UpdateEmployeeByCodeAsync(string employeeCode, EmployeeInfo employeeInfo);
        Task<Result> DeleteEmployeeByCodeAsync(string employeeCode);
    }
}
