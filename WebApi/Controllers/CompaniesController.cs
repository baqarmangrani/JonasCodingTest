using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    [RoutePrefix("api/companies")]
    public class CompaniesController : ApiController
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CompaniesController(ICompanyService companyService, IMapper mapper, ILogger logger)
        {
            _companyService = companyService ?? throw new ArgumentNullException(nameof(companyService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/companies
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            return await ExecuteAsync(async () =>
            {
                var items = await _companyService.GetAllCompaniesAsync();
                var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(items);
                return Ok(companyDtos);
            }, "Error occurred while getting all companies.");
        }

        // GET api/companies/{companyCode}
        [HttpGet, Route("{companyCode}", Name = "GetCompanyByCode")]
        public async Task<IHttpActionResult> Get(string companyCode)
        {
            return await ExecuteAsync(async () =>
            {
                var item = await _companyService.GetCompanyByCodeAsync(companyCode);
                if (item == null)
                {
                    return NotFound();
                }
                var companyDto = _mapper.Map<CompanyDto>(item);
                return Ok(companyDto);
            }, $"Error occurred while getting company with code {companyCode}.");
        }

        // POST api/companies
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return BadRequest("Request is null");
            }

            return await ExecuteAsync(async () =>
            {
                var companyInfo = _mapper.Map<CompanyInfo>(companyDto);
                var result = await _companyService.AddCompanyAsync(companyInfo);

                if (!result.IsSuccess)
                {
                    return Content(HttpStatusCode.Conflict, result.Message);
                }

                return CreatedAtRoute("GetCompanyByCode", new { companyCode = companyInfo.CompanyCode }, companyDto);
            }, "Error occurred while adding a new company.");
        }

        // PUT api/companies/{companyCode}
        [HttpPut, Route("{companyCode}")]
        public async Task<IHttpActionResult> Put(string companyCode, [FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return BadRequest("Request is null");
            }

            return await ExecuteAsync(async () =>
            {
                var companyInfo = _mapper.Map<CompanyInfo>(companyDto);
                var result = await _companyService.UpdateCompanyByCodeAsync(companyCode, companyInfo);

                if (!result.IsSuccess)
                {
                    return Content(HttpStatusCode.NotFound, result.Message);
                }

                return StatusCode(HttpStatusCode.NoContent);
            }, $"Error occurred while updating company with code {companyCode}.");
        }

        // DELETE api/companies/{companyCode}
        [HttpDelete, Route("{companyCode}")]
        public async Task<IHttpActionResult> Delete(string companyCode)
        {
            return await ExecuteAsync(async () =>
            {
                var result = await _companyService.DeleteCompanyByCodeAsync(companyCode);
                if (!result)
                {
                    return NotFound();
                }
                return StatusCode(HttpStatusCode.NoContent);
            }, $"Error occurred while deleting company with code {companyCode}.");
        }

        private async Task<IHttpActionResult> ExecuteAsync(Func<Task<IHttpActionResult>> action, string errorMessage)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, errorMessage);
                return InternalServerError(new Exception("An error occurred while processing your request."));
            }
        }
    }
}
