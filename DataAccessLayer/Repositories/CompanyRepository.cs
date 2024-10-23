using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbWrapper<Company> _companyDbWrapper;
        private readonly ILogger _logger;

        public CompanyRepository(IDbWrapper<Company> companyDbWrapper, ILogger logger)
        {
            _companyDbWrapper = companyDbWrapper ?? throw new ArgumentNullException(nameof(companyDbWrapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await ExecuteDbOperationAsync(() => _companyDbWrapper.FindAllAsync(), "Error occurred while getting all companies.");
        }

        public async Task<Company> GetByCodeAsync(string companyCode)
        {
            return (await ExecuteDbOperationAsync(() => _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)),
                $"Error occurred while getting company by code: {companyCode}"))?.FirstOrDefault();
        }

        public async Task<SaveResultData> SaveCompanyAsync(Company company)
        {
            return await ExecuteDbOperationAsync(async () =>
            {
                var existingCompany = (await _companyDbWrapper.FindAsync(t => t.SiteId.Equals(company.SiteId) && t.CompanyCode.Equals(company.CompanyCode)))?.FirstOrDefault();
                if (existingCompany != null)
                {
                    return new SaveResultData { Success = false, Message = "Company already exists with the same code." };
                }

                var insertResult = await _companyDbWrapper.InsertAsync(company);
                return new SaveResultData { Success = insertResult, Message = insertResult ? "Company saved successfully." : "Failed to save company." };
            }, $"Error occurred while saving company: {company}");
        }

        public async Task<bool> UpdateByCodeAsync(string companyCode, Company company)
        {
            return await ExecuteDbOperationAsync(async () =>
            {
                var existingCompany = (await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode)))?.FirstOrDefault();
                if (existingCompany == null)
                {
                    return false;
                }

                UpdateCompanyDetails(existingCompany, company);
                return await _companyDbWrapper.UpdateAsync(existingCompany);
            }, $"Error occurred while updating company by code: {companyCode}");
        }

        public async Task<bool> DeleteByCodeAsync(string companyCode)
        {
            return await ExecuteDbOperationAsync(() => _companyDbWrapper.DeleteAsync(t => t.CompanyCode.Equals(companyCode)),
                $"Error occurred while deleting company by code: {companyCode}");
        }

        public async Task<Company> GetByNameAsync(string companyName)
        {
            return (await ExecuteDbOperationAsync(() => _companyDbWrapper.FindAsync(t => t.CompanyName.Equals(companyName)),
                $"Error occurred while getting company by name: {companyName}"))?.FirstOrDefault();
        }

        private async Task<T> ExecuteDbOperationAsync<T>(Func<Task<T>> dbOperation, string errorMessage)
        {
            try
            {
                return await dbOperation();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, errorMessage);
                throw;
            }
        }

        private void UpdateCompanyDetails(Company existingCompany, Company newCompany)
        {
            existingCompany.CompanyName = newCompany.CompanyName;
            existingCompany.AddressLine1 = newCompany.AddressLine1;
            existingCompany.AddressLine2 = newCompany.AddressLine2;
            existingCompany.AddressLine3 = newCompany.AddressLine3;
            existingCompany.Country = newCompany.Country;
            existingCompany.EquipmentCompanyCode = newCompany.EquipmentCompanyCode;
            existingCompany.FaxNumber = newCompany.FaxNumber;
            existingCompany.PhoneNumber = newCompany.PhoneNumber;
            existingCompany.PostalZipCode = newCompany.PostalZipCode;
            existingCompany.LastModified = newCompany.LastModified;
        }
    }
}
