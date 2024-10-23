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
        try
        {
            var employees = await _employeeRepository.GetAllAsync();
            var employeeInfos = _mapper.Map<IEnumerable<EmployeeInfo>>(employees).ToList();

            foreach (var employee in employees)
            {
                var company = await _companyRepository.GetByCodeAsync(employee.CompanyCode);
                employeeInfos.First(e => e.EmployeeCode.Equals(employee.EmployeeCode)).CompanyName = company?.CompanyName;
            }

            return employeeInfos;
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(ex, "ArgumentNullException occurred while getting all employees.");
            return Enumerable.Empty<EmployeeInfo>();
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while getting all employees.");
            return Enumerable.Empty<EmployeeInfo>();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while getting all employees.");
            return Enumerable.Empty<EmployeeInfo>();
        }
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
            _logger.Error(ex, "ArgumentNullException occurred while getting employee by code.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while getting employee by code.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred while getting employee by code: {employeeCode}");
        }

        return employeeInfo;
    }

    public async Task<SaveResult> AddEmployeeAsync(EmployeeInfo employeeInfo)
    {
        try
        {
            var company = await _companyRepository.GetByNameAsync(employeeInfo.CompanyName);

            if (company == null)
            {
                return new SaveResult(false, "Company of that Employee is not found.");
            }

            var employee = _mapper.Map<Employee>(employeeInfo);

            employee.CompanyCode = company.CompanyCode;
            employee.SiteId = company.SiteId;

            var result = await _employeeRepository.SaveEmployeeAsync(employee);

            return new SaveResult(result.Success, result.Message);
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(ex, "ArgumentNullException occurred while adding an employee.");
            return new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while adding an employee.");
            return new SaveResult(false, "Operation could not be completed.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while adding an employee.");
            return new SaveResult(false, "An error occurred while adding the employee.");
        }
    }

    public async Task<SaveResult> UpdateEmployeeByCodeAsync(string employeeCode, EmployeeInfo employeeInfo)
    {
        try
        {
            var company = await _companyRepository.GetByNameAsync(employeeInfo.CompanyName);

            if (company == null)
            {
                return new SaveResult(false, $"Company with name {employeeInfo.CompanyName} not found.");
            }

            var employee = _mapper.Map<Employee>(employeeInfo);

            employee.CompanyCode = company.CompanyCode;
            employee.SiteId = company.SiteId;

            var result = await _employeeRepository.UpdateByCodeAsync(employeeCode, employee);

            if (!result.Success)
            {
                _logger.Warning($"Employee with code {employeeCode} not found.");
            }

            return new SaveResult(result.Success, result.Message);
        }
        catch (ArgumentNullException ex)
        {
            _logger.Error(ex, "ArgumentNullException occurred while updating the employee.");
            return new SaveResult(false, "Invalid input provided.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while updating the employee.");
            return new SaveResult(false, "Operation could not be completed.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while updating the employee.");
            return new SaveResult(false, "An error occurred while updating the employee.");
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
        catch (ArgumentNullException ex)
        {
            _logger.Error(ex, "ArgumentNullException occurred while deleting employee by code.");
            return false;
        }
        catch (InvalidOperationException ex)
        {
            _logger.Error(ex, "InvalidOperationException occurred while deleting employee by code.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred while deleting employee by code: {employeeCode}");
            return false;
        }
    }
}
