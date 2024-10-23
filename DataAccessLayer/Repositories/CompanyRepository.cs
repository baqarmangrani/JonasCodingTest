using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbWrapper<Company> _companyDbWrapper;

        public CompanyRepository(IDbWrapper<Company> companyDbWrapper)
        {
            _companyDbWrapper = companyDbWrapper;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _companyDbWrapper.FindAllAsync();
        }

        public async Task<Company> GetByCodeAsync(string companyCode)
        {
            var companies = await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode));
            return companies?.FirstOrDefault();
        }

        public async Task<SaveResultData> SaveCompanyAsync(Company company)
        {
            var itemRepo = (await _companyDbWrapper.FindAsync(t =>
                t.SiteId.Equals(company.SiteId) && t.CompanyCode.Equals(company.CompanyCode)))?.FirstOrDefault();

            if (itemRepo != null)
            {
                return new SaveResultData
                {
                    Success = false,
                    Message = "Company already exists with the same code."
                };
            }

            var insertResult = await _companyDbWrapper.InsertAsync(company);

            return new SaveResultData
            {
                Success = insertResult,
                Message = insertResult ? "Company saved successfully." : "Failed to save company."
            };
        }

        public async Task<bool> UpdateByCodeAsync(string companyCode, Company company)
        {
            var itemRepo = (await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)))?.FirstOrDefault();

            if (itemRepo != null)
            {
                itemRepo.CompanyName = company.CompanyName;
                itemRepo.AddressLine1 = company.AddressLine1;
                itemRepo.AddressLine2 = company.AddressLine2;
                itemRepo.AddressLine3 = company.AddressLine3;
                itemRepo.Country = company.Country;
                itemRepo.EquipmentCompanyCode = company.EquipmentCompanyCode;
                itemRepo.FaxNumber = company.FaxNumber;
                itemRepo.PhoneNumber = company.PhoneNumber;
                itemRepo.PostalZipCode = company.PostalZipCode;
                itemRepo.LastModified = company.LastModified;

                return await _companyDbWrapper.UpdateAsync(itemRepo);
            }

            return false;
        }

        public async Task<bool> DeleteByCodeAsync(string companyCode)
        {
            return await _companyDbWrapper.DeleteAsync(t => t.CompanyCode.Equals(companyCode));
        }
    }
}
