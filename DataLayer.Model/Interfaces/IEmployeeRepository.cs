using DataAccessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Model.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByCodeAsync(string employeeCode);
        Task<ResultData> SaveEmployeeAsync(Employee employee);
        Task<ResultData> UpdateByCodeAsync(string employeeCode, Employee employee);
        Task<bool> DeleteByCodeAsync(string employeeCode);
    }
}
