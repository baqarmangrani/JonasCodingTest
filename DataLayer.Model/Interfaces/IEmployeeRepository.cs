using DataAccessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Model.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByCodeAsync(string employeeCode);
        Task<SaveResultData> SaveEmployeeAsync(Employee employee);
        Task<bool> UpdateByCodeAsync(string employeeCode, Employee employee);
        Task<bool> DeleteByCodeAsync(string employeeCode);
    }
}
