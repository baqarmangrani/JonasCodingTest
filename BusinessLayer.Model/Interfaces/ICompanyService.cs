using BusinessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Model.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyInfo>> GetAllCompaniesAsync();
        Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode);
        Task<Result> AddCompanyAsync(CompanyInfo companyInfo);
        Task<Result> UpdateCompanyByCodeAsync(string companyCode, CompanyInfo companyInfo);
        Task<Result> DeleteCompanyByCodeAsync(string companyCode);
    }
}
