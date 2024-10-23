﻿using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<CompanyInfo>> GetAllCompaniesAsync()
        {
            var res = await _companyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyInfo>>(res);
        }

        public async Task<CompanyInfo> GetCompanyByCodeAsync(string companyCode)
        {
            var result = await _companyRepository.GetByCodeAsync(companyCode);
            return _mapper.Map<CompanyInfo>(result);
        }

        public async Task<SaveCompanyResult> AddCompanyAsync(CompanyInfo companyInfo)
        {
            var company = _mapper.Map<Company>(companyInfo);

            var resultData = await _companyRepository.SaveCompanyAsync(company);

            return new SaveCompanyResult
            {
                Success = resultData.Success,
                Message = resultData.Message
            };
        }

        public async Task<bool> UpdateCompanyByCodeAsync(string companyCode, CompanyInfo companyInfo)
        {
            var company = _mapper.Map<Company>(companyInfo);
            return await _companyRepository.UpdateByCodeAsync(companyCode, company);
        }

        public async Task<bool> DeleteCompanyByCodeAsync(string companyCode)
        {
            return await _companyRepository.DeleteByCodeAsync(companyCode);
        }
    }
}
