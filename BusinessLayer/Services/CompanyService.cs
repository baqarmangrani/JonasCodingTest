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
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<CompanyInfo>> GetAllCompaniesAsync()
    {
        try
        {
            var companies = await _companyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyInfo>>(companies);
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is CompanyServiceException)
        {
            _logger.Error(new DatabaseException("Error occurred while getting all companies.", ex), "Error occurred while getting all companies.");
            return Enumerable.Empty<CompanyInfo>();
        }
    }

    public async Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode)
    {
        if (string.IsNullOrWhiteSpace(companyCode))
        {
            throw new ArgumentException("Company code cannot be null or empty.", nameof(companyCode));
        }

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
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is CompanyServiceException)
        {
            _logger.Error(new DatabaseException($"Error occurred while getting company by code: {companyCode}", ex), $"Error occurred while getting company by code: {companyCode}");
            return null;
        }
    }

    public async Task<Result> AddCompanyAsync(CompanyInfo companyInfo)
    {
        if (companyInfo == null)
        {
            throw new ArgumentNullException(nameof(companyInfo));
        }

        try
        {
            var company = _mapper.Map<Company>(companyInfo);
            var saveResult = await _companyRepository.SaveCompanyAsync(company);
            return new Result(saveResult.IsSuccess, saveResult.Message);
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is CompanyServiceException)
        {
            _logger.Error(new DatabaseException("Error occurred while adding a company.", ex), "Error occurred while adding a company.");
            return new Result(false, "An error occurred while adding the company.");
        }
    }

    public async Task<Result> UpdateCompanyByCodeAsync(string companyCode, CompanyInfo companyInfo)
    {
        if (string.IsNullOrWhiteSpace(companyCode))
        {
            throw new ArgumentException("Company code cannot be null or empty.", nameof(companyCode));
        }

        if (companyInfo == null)
        {
            throw new ArgumentNullException(nameof(companyInfo));
        }

        try
        {
            var company = _mapper.Map<Company>(companyInfo);
            var updateResult = await _companyRepository.UpdateByCodeAsync(companyCode, company);
            if (!updateResult.IsSuccess)
            {
                _logger.Warning($"Company with code {companyCode} not found.");
                return new Result(false, "Failed to update company.");
            }
            return new Result(true, "Company updated successfully.");
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is CompanyServiceException)
        {
            _logger.Error(new DatabaseException("Error occurred while updating the company.", ex), "Error occurred while updating the company.");
            return new Result(false, "An error occurred while updating the company.");
        }
    }

    public async Task<Result> DeleteCompanyByCodeAsync(string companyCode)
    {
        if (string.IsNullOrWhiteSpace(companyCode))
        {
            throw new ArgumentException("Company code cannot be null or empty.", nameof(companyCode));
        }

        try
        {
            var result = await _companyRepository.DeleteByCodeAsync(companyCode);

            if (!result)
            {
                _logger.Warning($"Company with code {companyCode} not found.");
                return new Result(false, "Company not found.");
            }

            return new Result(true, $"Company with code {companyCode} was successfully deleted.");
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is CompanyServiceException)
        {
            _logger.Error(new DatabaseException($"Error occurred while deleting company by code: {companyCode}", ex), $"Error occurred while deleting company by code: {companyCode}");
            return new Result(false, "An error occurred while deleting the company.");
        }
    }
}
