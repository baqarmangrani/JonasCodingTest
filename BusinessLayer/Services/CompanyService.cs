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
        IEnumerable<CompanyInfo> result;

        try
        {
            var companies = await _companyRepository.GetAllAsync();
            result = _mapper.Map<IEnumerable<CompanyInfo>>(companies);
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(new DatabaseException("ArgumentNullException occurred while getting all companies.", ex), "ArgumentNullException occurred while getting all companies.");
            result = Enumerable.Empty<CompanyInfo>();
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while getting all companies.", ex), "InvalidOperationException occurred while getting all companies.");
            result = Enumerable.Empty<CompanyInfo>();
        }
        catch (CompanyServiceException ex)
        {
            _logger.Error(new DatabaseException("Error occurred while getting all companies.", ex), "Error occurred while getting all companies.");
            result = Enumerable.Empty<CompanyInfo>();
        }

        return result;
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
            _logger.Error(new DatabaseException("ArgumentNullException occurred while getting company by code.", ex), "ArgumentNullException occurred while getting company by code.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while getting company by code.", ex), "InvalidOperationException occurred while getting company by code.");
        }
        catch (CompanyServiceException ex)
        {
            _logger.Error(new DatabaseException($"Error occurred while getting company by code: {companyCode}", ex), $"Error occurred while getting company by code: {companyCode}");
        }

        return companyInfo;
    }

    public async Task<SaveResult> AddCompanyAsync(CompanyInfo companyInfo)
    {
        SaveResult result;

        try
        {
            var company = _mapper.Map<Company>(companyInfo);
            var saveResult = await _companyRepository.SaveCompanyAsync(company);
            result = new SaveResult(saveResult.Success, saveResult.Message);
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(new DatabaseException("ArgumentNullException occurred while adding a company.", ex), "ArgumentNullException occurred while adding a company.");
            result = new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while adding a company.", ex), "InvalidOperationException occurred while adding a company.");
            result = new SaveResult(false, "Operation could not be completed.");
        }
        catch (CompanyServiceException ex)
        {
            _logger.Error(new DatabaseException("Error occurred while adding a company.", ex), "Error occurred while adding a company.");
            result = new SaveResult(false, "An error occurred while adding the company.");
        }

        return result;
    }

    public async Task<SaveResult> UpdateCompanyByCodeAsync(string companyCode, CompanyInfo companyInfo)
    {
        SaveResult result;

        try
        {
            var company = _mapper.Map<Company>(companyInfo);
            var updateResult = await _companyRepository.UpdateByCodeAsync(companyCode, company);
            if (!updateResult)
            {
                _logger.Warning($"Company with code {companyCode} not found.");
            }
            result = new SaveResult(updateResult, updateResult ? "Company updated successfully." : "Failed to update company.");
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(new DatabaseException("ArgumentNullException occurred while updating the company.", ex), "ArgumentNullException occurred while updating the company.");
            result = new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while updating the company.", ex), "InvalidOperationException occurred while updating the company.");
            result = new SaveResult(false, "Operation could not be completed.");
        }
        catch (CompanyServiceException ex)
        {
            _logger.Error(new DatabaseException("Error occurred while updating the company.", ex), "Error occurred while updating the company.");
            result = new SaveResult(false, "An error occurred while updating the company.");
        }

        return result;
    }

    public async Task<bool> DeleteCompanyByCodeAsync(string companyCode)
    {
        bool result;

        try
        {
            result = await _companyRepository.DeleteByCodeAsync(companyCode);
            if (!result)
            {
                _logger.Warning($"Company with code {companyCode} not found.");
            }
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(new DatabaseException("ArgumentNullException occurred while deleting company by code.", ex), "ArgumentNullException occurred while deleting company by code.");
            result = false;
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while deleting company by code.", ex), "InvalidOperationException occurred while deleting company by code.");
            result = false;
        }
        catch (CompanyServiceException ex)
        {
            _logger.Error(new DatabaseException($"Error occurred while deleting company by code: {companyCode}", ex), $"Error occurred while deleting company by code: {companyCode}");
            result = false;
        }

        return result;
    }
}
