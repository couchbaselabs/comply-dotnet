using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using ComplyWebApi.Models.DataAccess;
using ComplyWebApi.Models.DocumentModels;
using Couchbase;

namespace ComplyWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompanyController : ApiController
    {
        private readonly CompanyDataAccess _dataAccess;

        public CompanyController()
        {
            var bucket = ClusterHelper.GetBucket(ConfigurationManager.AppSettings["CouchbaseBucket"]);
            _dataAccess = new CompanyDataAccess(bucket);
        }

        [HttpGet]
        [Route("api/company/get/{*companyId}")]
        public IHttpActionResult GetCompanyById(string companyId)
        {
            if(string.IsNullOrEmpty(companyId))
                return BadRequest("A company id must exist");
            return Ok(_dataAccess.GetCompanyById(companyId));
        }

        [HttpGet]
        [Route("api/company/getAll")]
        public IHttpActionResult GetCompanies()
        {
            return Ok(_dataAccess.GetCompanies());
        }

        [HttpPost]
        [Route("api/company/create")]
        public IHttpActionResult CreateCompany(Company company)
        {
            if (string.IsNullOrEmpty(company.Website))
                return BadRequest("A website must exist");
            if (string.IsNullOrEmpty(company.Name))
                return BadRequest("A name must exist");
            try
            {
                return Ok(_dataAccess.CreateCompany(company));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, ex.Message));
            }
        }
    }
}