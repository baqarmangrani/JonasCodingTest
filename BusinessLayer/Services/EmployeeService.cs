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
        _employeeRepository = employeeRepository;
        _companyRepository = companyRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<EmployeeInfo>> GetAllEmployeesAsync()
    {
        IEnumerable<EmployeeInfo> employeeInfos = Enumerable.Empty<EmployeeInfo>();

        try
        {
            var employees = await _employeeRepository.GetAllAsync();
            employeeInfos = _mapper.Map<IEnumerable<EmployeeInfo>>(employees).ToList();

            foreach (var employee in employees)
            {
                var company = await _companyRepository.GetByCodeAsync(employee.CompanyCode);
                employeeInfos.First(e => e.EmployeeCode.Equals(employee.EmployeeCode)).CompanyName = company?.CompanyName;
            }
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(new DatabaseException("ArgumentNullException occurred while getting all employees.", ex), ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while getting all employees.", ex), ex.Message);
        }
        catch (EmployeeServiceException ex)
        {
            _logger.Error(new DatabaseException("Error occurred while getting all employees.", ex), ex.Message);
        }

        return employeeInfos;
    }

    public async Task<EmployeeInfo> GetEmployeeByCodeAsync(string employeeCode)
    {
        EmployeeInfo employeeInfo = null;

        try
        {
            var employee = await _employeeRepository.GetByCodeAsync(employeeCode);
            if (employee == null)
            {
                _logger.Warning($"Employee with code {employeeCode} not found.");
            }
            else
            {
                employeeInfo = _mapper.Map<EmployeeInfo>(employee);
                var company = await _companyRepository.GetByCodeAsync(employee.CompanyCode);
                employeeInfo.CompanyName = company?.CompanyName;
            }
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(new DatabaseException("ArgumentNullException occurred while getting employee by code.", ex), ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while getting employee by code.", ex), ex.Message);
        }
        catch (EmployeeServiceException ex)
        {
            _logger.Error(new DatabaseException($"Error occurred while getting employee by code: {employeeCode}", ex), ex.Message);
        }

        return employeeInfo;
    }

    public async Task<SaveResult> AddEmployeeAsync(EmployeeInfo employeeInfo)
    {
        SaveResult saveResult = new SaveResult(false, "An error occurred while adding the employee.");

        try
        {
            var company = await _companyRepository.GetByNameAsync(employeeInfo.CompanyName);

            if (company == null)
            {
                saveResult = new SaveResult(false, "Company of that Employee is not found.");
            }
            else
            {
                var employee = _mapper.Map<Employee>(employeeInfo);

                employee.CompanyCode = company.CompanyCode;
                employee.SiteId = company.SiteId;

                var result = await _employeeRepository.SaveEmployeeAsync(employee);

                saveResult = new SaveResult(result.Success, result.Message);
            }
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(new DatabaseException("ArgumentNullException occurred while adding an employee.", ex), ex.Message);
            saveResult = new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while adding an employee.", ex), ex.Message);
            saveResult = new SaveResult(false, "Operation could not be completed.");
        }
        catch (EmployeeServiceException ex)
        {
            _logger.Error(new DatabaseException("Error occurred while adding an employee.", ex), ex.Message);
        }

        return saveResult;
    }

    public async Task<SaveResult> UpdateEmployeeByCodeAsync(string employeeCode, EmployeeInfo employeeInfo)
    {
        SaveResult saveResult = new SaveResult(false, "An error occurred while updating the employee.");

        try
        {
            var company = await _companyRepository.GetByNameAsync(employeeInfo.CompanyName);

            if (company == null)
            {
                saveResult = new SaveResult(false, $"Company with name {employeeInfo.CompanyName} not found.");
            }
            else
            {
                var employee = _mapper.Map<Employee>(employeeInfo);

                employee.CompanyCode = company.CompanyCode;
                employee.SiteId = company.SiteId;

                var result = await _employeeRepository.UpdateByCodeAsync(employeeCode, employee);

                if (!result.Success)
                {
                    _logger.Warning($"Employee with code {employeeCode} not found.");
                }

                saveResult = new SaveResult(result.Success, result.Message);
            }
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(new DatabaseException("ArgumentNullException occurred while updating the employee.", ex), ex.Message);
            saveResult = new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while updating the employee.", ex), ex.Message);
            saveResult = new SaveResult(false, "Operation could not be completed.");
        }
        catch (EmployeeServiceException ex)
        {
            _logger.Error(new DatabaseException("Error occurred while updating the employee.", ex), ex.Message);
        }

        return saveResult;
    }

    public async Task<bool> DeleteEmployeeByCodeAsync(string employeeCode)
    {
        bool result = false;
        try
        {
            result = await _employeeRepository.DeleteByCodeAsync(employeeCode);
            if (!result)
            {
                _logger.Warning($"Employee with code {employeeCode} not found.");
            }
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(new DatabaseException("ArgumentNullException occurred while deleting employee by code.", ex), ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(new DatabaseException("InvalidOperationException occurred while deleting employee by code.", ex), ex.Message);
        }
        catch (EmployeeServiceException ex)
        {
            _logger.Error(new DatabaseException($"Error occurred while deleting employee by code: {employeeCode}", ex), ex.Message);
        }

        return result;
    }
}
