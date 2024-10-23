using BusinessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Model.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyInfo>> GetAllCompaniesAsync();
        Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode);
        Task<SaveCompanyResult> AddCompanyAsync(CompanyInfo companyInfo);
        Task<bool> UpdateCompanyByCodeAsync(string companyCode, CompanyInfo companyInfo);
        Task<bool> DeleteCompanyByCodeAsync(string companyCode);
    }
}
