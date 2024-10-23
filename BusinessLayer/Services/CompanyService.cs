using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper, ILogger logger)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CompanyInfo>> GetAllCompaniesAsync()
        {
            try
            {
                var res = await _companyRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<CompanyInfo>>(res);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting all companies.");
                throw;
            }
        }

        public async Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode)
        {
            try
            {
                var result = await _companyRepository.GetByCodeAsync(companyCode);
                return _mapper.Map<CompanyInfo>(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while getting company by code: {companyCode}");
                throw;
            }
        }

        public async Task<SaveResult> AddCompanyAsync(CompanyInfo companyInfo)
        {
            try
            {
                var company = _mapper.Map<Company>(companyInfo);
                var resultData = await _companyRepository.SaveCompanyAsync(company);

                return new SaveResult
                {
                    Success = resultData.Success,
                    Message = resultData.Message
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while adding a new company.");
                throw;
            }
        }

        public async Task<bool> UpdateCompanyByCodeAsync(string companyCode, CompanyInfo companyInfo)
        {
            try
            {
                var company = _mapper.Map<Company>(companyInfo);
                return await _companyRepository.UpdateByCodeAsync(companyCode, company);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while updating company by code: {companyCode}");
                throw;
            }
        }

        public async Task<bool> DeleteCompanyByCodeAsync(string companyCode)
        {
            try
            {
                return await _companyRepository.DeleteByCodeAsync(companyCode);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while deleting company by code: {companyCode}");
                throw;
            }
        }
    }
}
