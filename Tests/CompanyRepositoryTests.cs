using DataAccessLayer.Model.Interfaces;
using DataAccessLayer.Model.Models;
using DataAccessLayer.Repositories;
using Moq;
using Serilog;
using System.Linq.Expressions;

namespace Tests
{
    public class CompanyRepositoryTests
    {
        private readonly Mock<IDbWrapper<Company>> _mockDbWrapper;
        private readonly Mock<ILogger> _mockLogger;
        private readonly CompanyRepository _repository;

        public CompanyRepositoryTests()
        {
            _mockDbWrapper = new Mock<IDbWrapper<Company>>();
            _mockLogger = new Mock<ILogger>();
            _repository = new CompanyRepository(_mockDbWrapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllCompanies()
        {

            var companies = new List<Company> { new Company { CompanyCode = "C1" }, new Company { CompanyCode = "C2" } };
            _mockDbWrapper.Setup(db => db.FindAllAsync()).ReturnsAsync(companies);


            var result = await _repository.GetAllAsync();


            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnCompany_WhenCompanyExists()
        {

            var companyCode = "C1";
            var companies = new List<Company> { new Company { CompanyCode = companyCode } };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Company, bool>>>())).ReturnsAsync(companies);


            var result = await _repository.GetByCodeAsync(companyCode);


            Assert.NotNull(result);
            Assert.Equal(companyCode, result.CompanyCode);
        }

        [Fact]
        public async Task GetByCodeAsync_ShouldReturnNull_WhenCompanyDoesNotExist()
        {

            var companyCode = "C1";
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Company, bool>>>())).ReturnsAsync((IEnumerable<Company>)null);


            var result = await _repository.GetByCodeAsync(companyCode);


            Assert.Null(result);
        }

        [Fact]
        public async Task SaveCompanyAsync_ShouldReturnSuccess_WhenCompanyIsSaved()
        {

            var company = new Company { CompanyCode = "C1", SiteId = "S1" };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Company, bool>>>())).ReturnsAsync((IEnumerable<Company>)null);
            _mockDbWrapper.Setup(db => db.InsertAsync(company)).ReturnsAsync(true);


            var result = await _repository.SaveCompanyAsync(company);


            Assert.True(result.IsSuccess);
            Assert.Equal("Company saved successfully.", result.Message);
        }

        [Fact]
        public async Task SaveCompanyAsync_ShouldReturnFailure_WhenCompanyAlreadyExists()
        {

            var company = new Company { CompanyCode = "C1", SiteId = "S1" };
            var existingCompanies = new List<Company> { company };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Company, bool>>>())).ReturnsAsync(existingCompanies);


            var result = await _repository.SaveCompanyAsync(company);


            Assert.False(result.IsSuccess);
            Assert.Equal("Company already exists with the same code.", result.Message);
        }

        [Fact]
        public async Task UpdateByCodeAsync_ShouldReturnSuccess_WhenCompanyIsUpdated()
        {

            var companyCode = "C1";
            var existingCompany = new Company { CompanyCode = companyCode, SiteId = "S1" };
            var updatedCompany = new Company { CompanyCode = companyCode, SiteId = "S1", CompanyName = "UpdatedName" };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Company, bool>>>())).ReturnsAsync(new List<Company> { existingCompany });
            _mockDbWrapper.Setup(db => db.UpdateAsync(It.IsAny<Company>())).ReturnsAsync(true);


            var result = await _repository.UpdateByCodeAsync(companyCode, updatedCompany);


            Assert.True(result.IsSuccess);
            Assert.Equal("Company updated successfully.", result.Message);
        }

        [Fact]
        public async Task DeleteByCodeAsync_ShouldReturnSuccess_WhenCompanyIsDeleted()
        {

            var companyCode = "C1";
            var existingCompany = new Company { CompanyCode = companyCode };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Company, bool>>>())).ReturnsAsync(new List<Company> { existingCompany });
            _mockDbWrapper.Setup(db => db.DeleteAsync(It.IsAny<Expression<Func<Company, bool>>>())).ReturnsAsync(true);


            var result = await _repository.DeleteByCodeAsync(companyCode);


            Assert.True(result);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnCompany_WhenCompanyExists()
        {

            var companyName = "TestCompany";
            var companies = new List<Company> { new Company { CompanyName = companyName } };
            _mockDbWrapper.Setup(db => db.FindAsync(It.IsAny<Expression<Func<Company, bool>>>())).ReturnsAsync(companies);


            var result = await _repository.GetByNameAsync(companyName);


            Assert.NotNull(result);
            Assert.Equal(companyName, result.CompanyName);
        }
    }
}