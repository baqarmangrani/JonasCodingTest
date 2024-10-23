using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public EmployeeService(IEmployeeRepository employeeRepository, ICompanyRepository companyRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeInfo>> GetAllEmployeesAsync()
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

    public async Task<EmployeeInfo> GetEmployeeByCodeAsync(string employeeCode)
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

    public async Task<SaveResult> AddEmployeeAsync(EmployeeInfo employeeInfo)
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

    public async Task<bool> UpdateEmployeeByCodeAsync(string employeeCode, EmployeeInfo employeeInfo)
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

    public async Task<bool> DeleteEmployeeByCodeAsync(string employeeCode)
    {
        return await _employeeRepository.DeleteByCodeAsync(employeeCode);
    }
}
