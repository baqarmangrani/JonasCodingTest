using AutoMapper;
using BusinessLayer.Exceptions;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var companies = await _companyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyInfo>>(companies);
        }
        catch (ArgumentNullException ex)
        {
            var customEx = new DatabaseException("ArgumentNullException occurred while getting all companies.", ex);
            _logger.Error(customEx, customEx.Message);
            return Enumerable.Empty<CompanyInfo>();
        }
        catch (InvalidOperationException ex)
        {
            var customEx = new DatabaseException("InvalidOperationException occurred while getting all companies.", ex);
            _logger.Error(customEx, customEx.Message);
            return Enumerable.Empty<CompanyInfo>();
        }
        catch (CompanyServiceException ex)
        {
            var customEx = new DatabaseException("Error occurred while getting all companies.", ex);
            _logger.Error(customEx, customEx.Message);
            return Enumerable.Empty<CompanyInfo>();
        }
    }

    public async Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode)
    {
        CompanyInfo companyInfo = null;

        try
        {
            var company = await _companyRepository.GetByCodeAsync(companyCode);
            if (company == null)
            {
                _logger.Warning($"Company with code {companyCode} not found.");
            }
            else
            {
                companyInfo = _mapper.Map<CompanyInfo>(company);
            }
        }
        catch (ArgumentNullException ex)
        {
            var customEx = new DatabaseException("ArgumentNullException occurred while getting company by code.", ex);
            _logger.Error(customEx, customEx.Message);
        }
        catch (InvalidOperationException ex)
        {
            var customEx = new DatabaseException("InvalidOperationException occurred while getting company by code.", ex);
            _logger.Error(customEx, customEx.Message);
        }
        catch (CompanyServiceException ex)
        {
            var customEx = new DatabaseException($"Error occurred while getting company by code: {companyCode}", ex);
            _logger.Error(customEx, customEx.Message);
        }

        return companyInfo;
    }

    public async Task<SaveResult> AddCompanyAsync(CompanyInfo companyInfo)
    {
        try
        {
            var company = _mapper.Map<Company>(companyInfo);
            var result = await _companyRepository.SaveCompanyAsync(company);

            return new SaveResult(result.Success, result.Message);
        }
        catch (ArgumentNullException ex)
        {
            var customEx = new DatabaseException("ArgumentNullException occurred while adding a company.", ex);
            _logger.Error(customEx, customEx.Message);
            return new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            var customEx = new DatabaseException("InvalidOperationException occurred while adding a company.", ex);
            _logger.Error(customEx, customEx.Message);
            return new SaveResult(false, "Operation could not be completed.");
        }
        catch (CompanyServiceException ex)
        {
            var customEx = new DatabaseException("Error occurred while adding a company.", ex);
            _logger.Error(customEx, customEx.Message);
            return new SaveResult(false, "An error occurred while adding the company.");
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
                _logger.Warning($"Company with code {companyCode} not found.");
            }

            return new SaveResult(result, result ? "Company updated successfully." : "Failed to update company.");
        }
        catch (ArgumentNullException ex)
        {
            var customEx = new DatabaseException("ArgumentNullException occurred while updating the company.", ex);
            _logger.Error(customEx, customEx.Message);
            return new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            var customEx = new DatabaseException("InvalidOperationException occurred while updating the company.", ex);
            _logger.Error(customEx, customEx.Message);
            return new SaveResult(false, "Operation could not be completed.");
        }
        catch (CompanyServiceException ex)
        {
            var customEx = new DatabaseException("Error occurred while updating the company.", ex);
            _logger.Error(customEx, customEx.Message);
            return new SaveResult(false, "An error occurred while updating the company.");
        }
    }

    public async Task<bool> DeleteCompanyByCodeAsync(string companyCode)
    {
        try
        {
            var result = await _companyRepository.DeleteByCodeAsync(companyCode);
            if (!result)
            {
                _logger.Warning($"Company with code {companyCode} not found.");
            }

            return result;
        }
        catch (ArgumentNullException ex)
        {
            var customEx = new DatabaseException("ArgumentNullException occurred while deleting company by code.", ex);
            _logger.Error(customEx, customEx.Message);
            return false;
        }
        catch (InvalidOperationException ex)
        {
            var customEx = new DatabaseException("InvalidOperationException occurred while deleting company by code.", ex);
            _logger.Error(customEx, customEx.Message);
            return false;
        }
        catch (CompanyServiceException ex)
        {
            var customEx = new DatabaseException($"Error occurred while deleting company by code: {companyCode}", ex);
            _logger.Error(customEx, customEx.Message);
            return false;
        }
    }
}
