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
            _companyDbWrapper = companyDbWrapper;
            _logger = logger;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            try
            {
                return await _companyDbWrapper.FindAllAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting all companies.");
                throw;
            }
        }

        public async Task<Company> GetByCodeAsync(string companyCode)
        {
            try
            {
                var companies = await _companyDbWrapper.FindAsync(t => t.CompanyCode.Equals(companyCode));
                return companies?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting company by code: {CompanyCode}", companyCode);
                throw;
            }
        }

        public async Task<SaveResultData> SaveCompanyAsync(Company company)
        {
            try
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
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while saving company: {Company}", company);
                throw;
            }
        }

        public async Task<bool> UpdateByCodeAsync(string companyCode, Company company)
        {
            try
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
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while updating company by code: {CompanyCode}", companyCode);
                throw;
            }
        }

        public async Task<bool> DeleteByCodeAsync(string companyCode)
        {
            try
            {
                return await _companyDbWrapper.DeleteAsync(t => t.CompanyCode.Equals(companyCode));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while deleting company by code: {CompanyCode}", companyCode);
                throw;
            }
        }

        public async Task<Company> GetByNameAsync(string companyName)
        {
            try
            {
                var companies = await _companyDbWrapper.FindAsync(t => t.CompanyName.Equals(companyName));
                return companies?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting company by name: {CompanyName}", companyName);
                throw;
            }
        }
    }
}
