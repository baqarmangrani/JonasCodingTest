using AutoMapper;
using BusinessLayer.Exceptions;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
            catch (SqlException ex)
            {
                _logger.Error(ex, "Database error occurred while getting all companies.");
                throw new DatabaseException(DatabaseException.DefaultMessage, ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting all companies.");
                throw new CompanyServiceException(CompanyServiceException.DefaultMessage, ex);
            }
        }

        public async Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode)
        {
            try
            {
                var result = await _companyRepository.GetByCodeAsync(companyCode);
                if (result == null)
                {
                    throw new NotFoundException($"Company with code {companyCode} not found.");
                }
                return _mapper.Map<CompanyInfo>(result);
            }
            catch (NotFoundException ex)
            {
                _logger.Warning(ex, $"Company with code {companyCode} not found.");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.Error(ex, $"Database error occurred while getting company by code: {companyCode}");
                throw new DatabaseException(DatabaseException.DefaultMessage, ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while getting company by code: {companyCode}");
                throw new CompanyServiceException(CompanyServiceException.DefaultMessage, ex);
            }
        }

        public async Task<SaveResult> AddCompanyAsync(CompanyInfo companyInfo)
        {
            try
            {
                var company = _mapper.Map<Company>(companyInfo);
                var resultData = await _companyRepository.SaveCompanyAsync(company);

                if (!resultData.Success)
                {
                    throw new ConflictException(resultData.Message ?? ConflictException.DefaultMessage);
                }

                return new SaveResult(resultData.Success, resultData.Message);
            }
            catch (ConflictException ex)
            {
                _logger.Warning(ex, "Conflict occurred while adding a new company.");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.Error(ex, "Database error occurred while adding a new company.");
                throw new DatabaseException(DatabaseException.DefaultMessage, ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while adding a new company.");
                throw new CompanyServiceException(CompanyServiceException.DefaultMessage, ex);
            }
        }

        public async Task<SaveResult> UpdateCompanyByCodeAsync(string companyCode, CompanyInfo companyInfo)
        {
            try
            {
                var company = _mapper.Map<Company>(companyInfo);
                var result = await _companyRepository.UpdateByCodeAsync(companyCode, company);

                if (!result)
                {
                    return new SaveResult(false, $"Company with code {companyCode} not found.");
                }

                return new SaveResult(true, "Company updated successfully.");
            }
            catch (NotFoundException ex)
            {
                _logger.Warning(ex, $"Company with code {companyCode} not found.");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.Error(ex, $"Database error occurred while updating company by code: {companyCode}");
                throw new DatabaseException(DatabaseException.DefaultMessage, ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while updating company by code: {companyCode}");
                throw new CompanyServiceException(CompanyServiceException.DefaultMessage, ex);
            }
        }

        public async Task<bool> DeleteCompanyByCodeAsync(string companyCode)
        {
            try
            {
                var result = await _companyRepository.DeleteByCodeAsync(companyCode);

                if (!result)
                {
                    throw new NotFoundException($"Company with code {companyCode} not found.");
                }

                return result;
            }
            catch (NotFoundException ex)
            {
                _logger.Warning(ex, $"Company with code {companyCode} not found.");
                throw;
            }
            catch (SqlException ex)
            {
                _logger.Error(ex, $"Database error occurred while deleting company by code: {companyCode}");
                throw new DatabaseException(DatabaseException.DefaultMessage, ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error occurred while deleting company by code: {companyCode}");
                throw new CompanyServiceException(CompanyServiceException.DefaultMessage, ex);
            }
        }
    }
}
