using AutoMapper;
using BusinessLayer.Model.Interfaces;
using BusinessLayer.Model.Models;
using System.Collections.Generic;
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
        public IHttpActionResult GetAll()
        {
            var items = _companyService.GetAllCompanies();
            var companyDtos = _mapper.Map<IEnumerable<CompanyDto>>(items);
            return Ok(companyDtos);
        }

        // GET api/company/{companyCode}
        [HttpGet, Route("{companyCode}", Name = "GetCompanyByCode")]
        public IHttpActionResult Get(string companyCode)
        {
            var item = _companyService.GetCompanyByCode(companyCode);
            if (item == null)
            {
                return NotFound();
            }
            var companyDto = _mapper.Map<CompanyDto>(item);
            return Ok(companyDto);
        }

        // POST api/company
        [HttpPost]
        public IHttpActionResult Post([FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return BadRequest("Request is null");
            }

            var companyInfo = _mapper.Map<CompanyInfo>(companyDto);
            var result = _companyService.AddCompany(companyInfo);
            if (!result)
            {
                return InternalServerError();
            }

            // Use the named route "GetCompanyByCode"
            return CreatedAtRoute("GetCompanyByCode", new { companyCode = companyInfo.CompanyCode }, companyDto);
        }

        // PUT api/company/{companyCode}
        [HttpPut, Route("{companyCode}")]
        public IHttpActionResult Put(string companyCode, [FromBody] CompanyDto companyDto)
        {
            if (companyDto == null)
            {
                return BadRequest("Request is null");
            }

            var companyInfo = _mapper.Map<CompanyInfo>(companyDto);
            var result = _companyService.UpdateCompanyByCode(companyCode, companyInfo);
            if (!result)
            {
                return NotFound();
            }
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        // DELETE api/company/{companyCode}
        [HttpDelete, Route("{companyCode}")]
        public IHttpActionResult Delete(string companyCode)
        {
            var result = _companyService.DeleteCompanyByCode(companyCode);
            if (!result)
            {
                return NotFound();
            }
            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }
    }
}
