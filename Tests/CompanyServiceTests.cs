using AutoMapper;
using BusinessLayer.Model.Models;
using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using Moq;
using Serilog;

namespace Tests
{
    public class CompanyServiceTests
    {

        private readonly Mock<ICompanyRepository> _mockCompanyRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger> _mockLogger;
        private readonly CompanyService _companyService;

        public CompanyServiceTests()
        {
            _mockCompanyRepository = new Mock<ICompanyRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger>();
            _companyService = new CompanyService(_mockCompanyRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllCompaniesAsync_ShouldReturnAllCompanies()
        {

            var companies = new List<Company> { new Company { CompanyCode = "C1" }, new Company { CompanyCode = "C2" } };
            var companyInfos = new List<CompanyInfo> { new CompanyInfo { CompanyCode = "C1" }, new CompanyInfo { CompanyCode = "C2" } };
            _mockCompanyRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(companies);
            _mockMapper.Setup(m => m.Map<IEnumerable<CompanyInfo>>(companies)).Returns(companyInfos);


            var result = await _companyService.GetAllCompaniesAsync();


            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetCompanyByCodeAsync_ShouldReturnCompany_WhenCompanyExists()
        {

            var companyCode = "C1";
            var company = new Company { CompanyCode = companyCode };
            var companyInfo = new CompanyInfo { CompanyCode = companyCode };
            _mockCompanyRepository.Setup(repo => repo.GetByCodeAsync(companyCode)).ReturnsAsync(company);
            _mockMapper.Setup(m => m.Map<CompanyInfo>(company)).Returns(companyInfo);


            var result = await _companyService.GetCompanyByCodeAsync(companyCode);


            Assert.NotNull(result);
            Assert.Equal(companyCode, result.CompanyCode);
        }

        [Fact]
        public async Task GetCompanyByCodeAsync_ShouldReturnNull_WhenCompanyDoesNotExist()
        {

            var companyCode = "C1";
            _mockCompanyRepository.Setup(repo => repo.GetByCodeAsync(companyCode)).ReturnsAsync((Company)null);


            var result = await _companyService.GetCompanyByCodeAsync(companyCode);


            Assert.Null(result);
        }

        [Fact]
        public async Task AddCompanyAsync_ShouldReturnSuccess_WhenCompanyIsAdded()
        {

            var companyInfo = new CompanyInfo { CompanyCode = "C1" };
            var company = new Company { CompanyCode = "C1" };
            var resultData = new ResultData { IsSuccess = true, Message = "Company added successfully." };
            _mockMapper.Setup(m => m.Map<Company>(companyInfo)).Returns(company);
            _mockCompanyRepository.Setup(repo => repo.SaveCompanyAsync(company)).ReturnsAsync(resultData);


            var result = await _companyService.AddCompanyAsync(companyInfo);


            Assert.True(result.IsSuccess);
            Assert.Equal("Company added successfully.", result.Message);
        }

        [Fact]
        public async Task UpdateCompanyByCodeAsync_ShouldReturnSuccess_WhenCompanyIsUpdated()
        {

            var companyCode = "C1";
            var companyInfo = new CompanyInfo { CompanyCode = companyCode };
            var company = new Company { CompanyCode = companyCode };
            var resultData = new ResultData { IsSuccess = true, Message = "Company updated successfully." };
            _mockMapper.Setup(m => m.Map<Company>(companyInfo)).Returns(company);
            _mockCompanyRepository.Setup(repo => repo.UpdateByCodeAsync(companyCode, company)).ReturnsAsync(resultData);


            var result = await _companyService.UpdateCompanyByCodeAsync(companyCode, companyInfo);


            Assert.True(result.IsSuccess);
            Assert.Equal("Company updated successfully.", result.Message);
        }

        [Fact]
        public async Task DeleteCompanyByCodeAsync_ShouldReturnSuccess_WhenCompanyIsDeleted()
        {

            var companyCode = "C1";
            _mockCompanyRepository.Setup(repo => repo.DeleteByCodeAsync(companyCode)).ReturnsAsync(true);


            var result = await _companyService.DeleteCompanyByCodeAsync(companyCode);


            Assert.True(result.IsSuccess);
            Assert.Equal($"Company with code {companyCode} was successfully deleted.", result.Message);
        }

        [Fact]
        public async Task DeleteCompanyByCodeAsync_ShouldReturnFailure_WhenCompanyDoesNotExist()
        {

            var companyCode = "C1";
            _mockCompanyRepository.Setup(repo => repo.DeleteByCodeAsync(companyCode)).ReturnsAsync(false);


            var result = await _companyService.DeleteCompanyByCodeAsync(companyCode);


            Assert.False(result.IsSuccess);
            Assert.Equal("Company not found.", result.Message);
        }



        [Fact]
        public async Task GetAllCompaniesAsync_ReturnsMappedCompanyInfos()
        {

            var companies = new List<Company> { new Company { CompanyCode = "C1" } };
            var companyInfos = new List<CompanyInfo> { new CompanyInfo { CompanyCode = "C1" } };
            _mockCompanyRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(companies);
            _mockMapper.Setup(m => m.Map<IEnumerable<CompanyInfo>>(companies)).Returns(companyInfos);


            var result = await _companyService.GetAllCompaniesAsync();


            Assert.Equal(companyInfos, result);
        }

        [Fact]
        public async Task GetCompanyByCodeAsync_ExistingCompany_ReturnsMappedCompanyInfo()
        {

            string companyCode = "C1";
            var company = new Company { CompanyCode = companyCode };
            var companyInfo = new CompanyInfo { CompanyCode = companyCode };
            _mockCompanyRepository.Setup(r => r.GetByCodeAsync(companyCode)).ReturnsAsync(company);
            _mockMapper.Setup(m => m.Map<CompanyInfo>(company)).Returns(companyInfo);


            var result = await _companyService.GetCompanyByCodeAsync(companyCode);


            Assert.Equal(companyInfo, result);
        }

        [Fact]
        public async Task AddCompanyAsync_ValidCompany_ReturnsSuccessResult()
        {

            var companyInfo = new CompanyInfo { CompanyCode = "C1" };
            var company = new Company { CompanyCode = "C1" };
            _mockMapper.Setup(m => m.Map<Company>(companyInfo)).Returns(company);
            _mockCompanyRepository.Setup(r => r.SaveCompanyAsync(company)).ReturnsAsync(new ResultData { IsSuccess = true, Message = "Company added" });


            var result = await _companyService.AddCompanyAsync(companyInfo);


            Assert.True(result.IsSuccess);
            Assert.Equal("Company added", result.Message);
        }

        [Fact]
        public async Task DeleteCompanyByCodeAsync_ExistingCompany_ReturnsSuccessResult()
        {

            string companyCode = "C1";
            _mockCompanyRepository.Setup(r => r.DeleteByCodeAsync(companyCode)).ReturnsAsync(true);


            var result = await _companyService.DeleteCompanyByCodeAsync(companyCode);


            Assert.True(result.IsSuccess);
            Assert.Equal($"Company with code {companyCode} was successfully deleted.", result.Message);
        }
    }
}
