using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using Moq;
using Serilog;
using System.Net;
using System.Web.Http.Results;
using WebApi.Controllers;
using WebApi.Models;

namespace Tests
{
    public class CompaniesControllerTests
    {
        private readonly Mock<ICompanyService> _mockCompanyService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger> _mockLogger;
        private readonly CompaniesController _controller;

        public CompaniesControllerTests()
        {
            _mockCompanyService = new Mock<ICompanyService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger>();
            _controller = new CompaniesController(_mockCompanyService.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCompanies()
        {

            var companies = new List<CompanyInfo> { new CompanyInfo { CompanyCode = "C1" }, new CompanyInfo { CompanyCode = "C2" } };
            var companyDtos = new List<CompanyDto> { new CompanyDto { CompanyCode = "C1" }, new CompanyDto { CompanyCode = "C2" } };

            _mockCompanyService.Setup(service => service.GetAllCompaniesAsync()).ReturnsAsync(companies);
            _mockMapper.Setup(m => m.Map<IEnumerable<CompanyDto>>(companies)).Returns(companyDtos);


            var result = await _controller.GetAll() as OkNegotiatedContentResult<IEnumerable<CompanyDto>>;


            Assert.NotNull(result);
            Assert.Equal(2, result.Content.Count());
        }

        [Fact]
        public async Task Get_ShouldReturnCompany_WhenCompanyExists()
        {

            var companyCode = "C1";
            var company = new CompanyInfo { CompanyCode = companyCode };
            var companyDto = new CompanyDto { CompanyCode = companyCode };

            _mockCompanyService.Setup(service => service.GetCompanyByCodeAsync(companyCode)).ReturnsAsync(company);
            _mockMapper.Setup(m => m.Map<CompanyDto>(company)).Returns(companyDto);


            var result = await _controller.Get(companyCode) as OkNegotiatedContentResult<CompanyDto>;


            Assert.NotNull(result);
            Assert.Equal(companyCode, result.Content.CompanyCode);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenCompanyDoesNotExist()
        {

            var companyCode = "C1";
            _mockCompanyService.Setup(service => service.GetCompanyByCodeAsync(companyCode)).ReturnsAsync((CompanyInfo)null);


            var result = await _controller.Get(companyCode);


            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_ShouldReturnCreated_WhenCompanyIsAdded()
        {

            var companyDto = new CompanyDto { CompanyCode = "C1", CompanyName = "Company1" };
            var companyInfo = new CompanyInfo { CompanyCode = "C1", CompanyName = "Company1" };
            var result = new Result(true, "Company added successfully.");

            _mockMapper.Setup(m => m.Map<CompanyInfo>(companyDto)).Returns(companyInfo);
            _mockCompanyService.Setup(service => service.AddCompanyAsync(companyInfo)).ReturnsAsync(result);


            var response = await _controller.Post(companyDto) as CreatedAtRouteNegotiatedContentResult<CompanyDto>;


            Assert.NotNull(response);
            Assert.Equal("GetCompanyByCode", response.RouteName);
            Assert.Equal(companyDto.CompanyCode, response.RouteValues["companyCode"]);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenCompanyDtoIsNull()
        {

            var result = await _controller.Post(null) as BadRequestErrorMessageResult;


            Assert.NotNull(result);
            Assert.Equal("Request is null", result.Message);
        }

        [Fact]
        public async Task Put_ShouldReturnBadRequest_WhenCompanyDtoIsNull()
        {

            var result = await _controller.Put("C1", null) as BadRequestErrorMessageResult;


            Assert.NotNull(result);
            Assert.Equal("Request is null", result.Message);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenCompanyDoesNotExist()
        {

            var companyCode = "C1";
            var result = new Result(false, "Company not found.");

            _mockCompanyService.Setup(service => service.DeleteCompanyByCodeAsync(companyCode)).ReturnsAsync(result);


            var response = await _controller.Delete(companyCode) as NegotiatedContentResult<string>;


            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Equal("Company not found.", response.Content);
        }
    }
}