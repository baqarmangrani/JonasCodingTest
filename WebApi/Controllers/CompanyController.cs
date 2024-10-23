using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    [RoutePrefix("api/company")]
    public class CompanyController : ApiController
    {
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;

        public CompanyController(ICompanyService companyService, IMapper mapper)
        {
            _companyService = companyService;
            _mapper = mapper;
        }

        // GET api/company
        [HttpGet]
        public async Task<IHttpActionResult> GetAll()
        {
            var items = await _companyService.GetAllCompaniesAsync();
            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(items);
            return Ok(companyDtos);
        }

        // GET api/company/{companyCode}
        [HttpGet, Route("{companyCode}", Name = "GetCompanyByCode")]
        public async Task<IHttpActionResult> Get(string companyCode)
        {
            var item = await _companyService.GetCompanyByCodeAsync(companyCode);
            if (item == null)
            {
                return NotFound();
            }
            var companyDto = _mapper.Map<CompanyDto>(item);
            return Ok(companyDto);
        }

        // POST api/company
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return BadRequest("Request is null");
            }

            var companyInfo = _mapper.Map<CompanyInfo>(companyDto);
            var result = await _companyService.AddCompanyAsync(companyInfo);

            if (!result.Success)
            {
                return Content(HttpStatusCode.Conflict, result.Message);
            }

            return CreatedAtRoute("GetCompanyByCode", new { companyCode = companyInfo.CompanyCode }, companyDto);
        }

        // PUT api/company/{companyCode}
        [HttpPut, Route("{companyCode}")]
        public async Task<IHttpActionResult> Put(string companyCode, [FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return BadRequest("Request is null");
            }

            var companyInfo = _mapper.Map<CompanyInfo>(companyDto);
            var result = await _companyService.UpdateCompanyByCodeAsync(companyCode, companyInfo);
            if (!result)
            {
                return NotFound();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE api/company/{companyCode}
        [HttpDelete, Route("{companyCode}")]
        public async Task<IHttpActionResult> Delete(string companyCode)
        {
            var result = await _companyService.DeleteCompanyByCodeAsync(companyCode);
            if (!result)
            {
                return NotFound();
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
