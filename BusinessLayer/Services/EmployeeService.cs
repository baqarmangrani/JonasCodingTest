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
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while getting all employees.");
            throw;
        }
    }

    public async Task<EmployeeInfo> GetEmployeeByCodeAsync(string employeeCode)
    {
        try
        {
            var employee = await _employeeRepository.GetByCodeAsync(employeeCode);
            if (employee == null)
            {
                return null;
            }

            var employeeInfo = _mapper.Map<EmployeeInfo>(employee);
            var company = await _companyRepository.GetByCodeAsync(employee.CompanyCode);
            employeeInfo.CompanyName = company?.CompanyName;

            return employeeInfo;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred while getting employee by code: {employeeCode}");
            throw;
        }
    }

    public async Task<SaveResult> AddEmployeeAsync(EmployeeInfo employeeInfo)
    {
        try
        {
            var company = await _companyRepository.GetByNameAsync(employeeInfo.CompanyName);

            if (company == null)
            {
                return new SaveResult
                {
                    Success = false,
                    Message = "Company of that Employeed is not found."
                };
            }

            var employee = _mapper.Map<Employee>(employeeInfo);

            employee.CompanyCode = company.CompanyCode;
            employee.SiteId = company.SiteId;

            var resultData = await _employeeRepository.SaveEmployeeAsync(employee);

            return new SaveResult
            {
                Success = resultData.Success,
                Message = resultData.Message
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while adding an employee.");
            throw;
        }
    }

    public async Task<bool> UpdateEmployeeByCodeAsync(string employeeCode, EmployeeInfo employeeInfo)
    {
        try
        {
            var company = await _companyRepository.GetByNameAsync(employeeInfo.CompanyName);

            if (company == null)
            {
                return false;
            }

            var employee = _mapper.Map<Employee>(employeeInfo);

            employee.CompanyCode = company.CompanyCode;
            employee.SiteId = company.SiteId;

            return await _employeeRepository.UpdateByCodeAsync(employeeCode, employee);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred while updating employee by code: {employeeCode}");
            throw;
        }
    }

    public async Task<bool> DeleteEmployeeByCodeAsync(string employeeCode)
    {
        try
        {
            return await _employeeRepository.DeleteByCodeAsync(employeeCode);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error occurred while deleting employee by code: {employeeCode}");
            throw;
        }
    }
}
