using BusinessLayer.Model.Models;
using System.Collections.Generic;

namespace BusinessLayer.Model.Interfaces
{
    public interface ICompanyService
    {
        IEnumerable<CompanyInfo> GetAllCompanies();
        CompanyInfo GetCompanyByCode(string companyCode);
        bool AddCompany(CompanyInfo companyInfo);
        bool UpdateCompanyByCode(string companyCode, CompanyInfo companyInfo);
        bool DeleteCompanyByCode(string companyCode);
    }
}
