using DataAccessLayer.Model.Models;
using System.Collections.Generic;

namespace DataAccessLayer.Model.Interfaces
{
    public interface ICompanyRepository
    {
        IEnumerable<Company> GetAll();
        Company GetByCode(string companyCode);
        bool SaveCompany(Company company);
        bool UpdateByCode(string companyCode, Company company);
        bool DeleteByCode(string companyCode);
    }
}
