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

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public EmployeeService(IEmployeeRepository employeeRepository, ICompanyRepository companyRepository, IMapper mapper, ILogger logger)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<EmployeeInfo>> GetAllEmployeesAsync()
    {
        try
        {
            var employees = await _employeeRepository.GetAllAsync();
            var employeeInfos = _mapper.Map<IEnumerable<EmployeeInfo>>(employees).ToList();

            foreach (var employee in employees)
            {
                var company = await _companyRepository.GetByCodeAsync(employee.CompanyCode);
                var employeeInfo = employeeInfos.First(e => e.EmployeeCode.Equals(employee.EmployeeCode));
                employeeInfo.CompanyName = company?.CompanyName;
            }

            return employeeInfos;
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is EmployeeServiceException)
        {
            _logger.Error(new DatabaseException("Error occurred while getting all employees.", ex), ex.Message);
            return Enumerable.Empty<EmployeeInfo>();
        }
    }

    public async Task<EmployeeInfo> GetEmployeeByCodeAsync(string employeeCode)
    {
        try
        {
            var employee = await _employeeRepository.GetByCodeAsync(employeeCode);
            if (employee == null)
            {
                _logger.Warning($"Employee with code {employeeCode} not found.");
                return null;
            }

            var employeeInfo = _mapper.Map<EmployeeInfo>(employee);
            var company = await _companyRepository.GetByCodeAsync(employee.CompanyCode);
            employeeInfo.CompanyName = company?.CompanyName;

            return employeeInfo;
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is EmployeeServiceException)
        {
            _logger.Error(new DatabaseException($"Error occurred while getting employee by code: {employeeCode}", ex), ex.Message);
            return null;
        }
    }

    public async Task<Result> AddEmployeeAsync(EmployeeInfo employeeInfo)
    {
        if (employeeInfo == null)
        {
            throw new ArgumentNullException(nameof(employeeInfo));
        }

        try
        {
            var company = await _companyRepository.GetByNameAsync(employeeInfo.CompanyName);
            if (company == null)
            {
                return new Result(false, "Company of that Employee is not found.");
            }

            var employee = _mapper.Map<Employee>(employeeInfo);
            employee.CompanyCode = company.CompanyCode;
            employee.SiteId = company.SiteId;

            var result = await _employeeRepository.SaveEmployeeAsync(employee);
            return new Result(result.IsSuccess, result.Message);
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is EmployeeServiceException)
        {
            _logger.Error(new DatabaseException("Error occurred while adding an employee.", ex), ex.Message);
            return new Result(false, "An error occurred while adding the employee.");
        }
    }

    public async Task<Result> UpdateEmployeeByCodeAsync(string employeeCode, EmployeeInfo employeeInfo)
    {
        if (employeeInfo == null)
        {
            throw new ArgumentNullException(nameof(employeeInfo));
        }

        try
        {
            var company = await _companyRepository.GetByNameAsync(employeeInfo.CompanyName);
            if (company == null)
            {
                return new Result(false, $"Company with name {employeeInfo.CompanyName} not found.");
            }

            var employee = _mapper.Map<Employee>(employeeInfo);
            employee.CompanyCode = company.CompanyCode;
            employee.SiteId = company.SiteId;

            var result = await _employeeRepository.UpdateByCodeAsync(employeeCode, employee);
            if (!result.IsSuccess)
            {
                _logger.Warning($"Employee with code {employeeCode} not found.");
            }

            return new Result(result.IsSuccess, result.Message);
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is EmployeeServiceException)
        {
            _logger.Error(new DatabaseException("Error occurred while updating the employee.", ex), ex.Message);
            return new Result(false, "An error occurred while updating the employee.");
        }
    }

    public async Task<bool> DeleteEmployeeByCodeAsync(string employeeCode)
    {
        try
        {
            var result = await _employeeRepository.DeleteByCodeAsync(employeeCode);
            if (!result)
            {
                _logger.Warning($"Employee with code {employeeCode} not found.");
            }
            return result;
        }
        catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException || ex is EmployeeServiceException)
        {
            _logger.Error(new DatabaseException($"Error occurred while deleting employee by code: {employeeCode}", ex), ex.Message);
            return false;
        }
    }
}
