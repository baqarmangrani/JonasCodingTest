using DataAccessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Model.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company> GetByCodeAsync(string companyCode);
        Task<Company> GetByNameAsync(string companyName);
        Task<ResultData> SaveCompanyAsync(Company company);
        Task<ResultData> UpdateByCodeAsync(string companyCode, Company company);
        Task<bool> DeleteByCodeAsync(string companyCode);
    }
}

