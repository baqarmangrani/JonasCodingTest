using AutoMapper;
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
            _logger.Error(ex, "ArgumentNullException occurred while getting all companies.");
            return Enumerable.Empty<CompanyInfo>();
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while getting all companies.");
            return Enumerable.Empty<CompanyInfo>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while getting all companies.");
            return Enumerable.Empty<CompanyInfo>();
        }
    }

    public async Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode)
    {
        try
        {
            var company = await _companyRepository.GetByCodeAsync(companyCode);
            if (company == null)
            {
                _logger.Warning($"Company with code {companyCode} not found.");
                return null;
            }

            return _mapper.Map<CompanyInfo>(company);
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(ex, "ArgumentNullException occurred while getting company by code.");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while getting company by code.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred while getting company by code: {companyCode}");
            return null;
        }
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
            _logger.Error(ex, "ArgumentNullException occurred while adding a company.");
            return new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while adding a company.");
            return new SaveResult(false, "Operation could not be completed.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while adding a company.");
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
            _logger.Error(ex, "ArgumentNullException occurred while updating the company.");
            return new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while updating the company.");
            return new SaveResult(false, "Operation could not be completed.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating the company.");
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
            _logger.Error(ex, "ArgumentNullException occurred while deleting company by code.");
            return false;
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while deleting company by code.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred while deleting company by code: {companyCode}");
            return false;
        }
    }
}
