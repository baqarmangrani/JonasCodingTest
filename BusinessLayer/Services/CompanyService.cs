using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using System.Collections.Generic;

namespace BusinessLayer.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public IEnumerable<CompanyInfo> GetAllCompanies()
        {
            var res = _companyRepository.GetAll();
            return _mapper.Map<IEnumerable<CompanyInfo>>(res);
        }

        public CompanyInfo GetCompanyByCode(string companyCode)
        {
            var result = _companyRepository.GetByCode(companyCode);
            return _mapper.Map<CompanyInfo>(result);
        }

        public bool AddCompany(CompanyInfo companyInfo)
        {
            var company = _mapper.Map<Company>(companyInfo);
            return _companyRepository.SaveCompany(company);
        }

        public bool UpdateCompanyByCode(string companyCode, CompanyInfo companyInfo)
        {
            var company = _mapper.Map<Company>(companyInfo);
            return _companyRepository.UpdateByCode(companyCode, company);
        }

        public bool DeleteCompanyByCode(string companyCode)
        {
            return _companyRepository.DeleteByCode(companyCode);
        }
    }
}
