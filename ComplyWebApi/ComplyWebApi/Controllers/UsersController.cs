using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Web.Http;
using System.Web.Http.Cors;
using ComplyWebApi.Models;
using ComplyWebApi.Models.DataAccess;
using ComplyWebApi.Models.DocumentModels;
using Couchbase;

namespace ComplyWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UsersController : ApiController
    {
        private readonly UserDataAccess _dataAccess;

        public UsersController()
        {
            var bucket = ClusterHelper.GetBucket(ConfigurationManager.AppSettings["CouchbaseBucket"]);
            _dataAccess = new UserDataAccess(bucket);
        }

        [HttpGet]
        [Route("api/user/get/{userId}")]
        public IHttpActionResult GetUserById(string userId)
        {
            return Ok(_dataAccess.GetUserById(userId));
        }

        [HttpGet]
        [Route("api/user/getAll")]
        public IHttpActionResult GetUsers()
        {
            return Ok(_dataAccess.GetUsers());
        }

        [HttpGet]
        [Route("api/user/login/{username}/{password}")]
        public IHttpActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest("A username must exist");
            if (string.IsNullOrEmpty(password))
                return BadRequest("A password must exist");

            try
            {
                return Ok(_dataAccess.Login(username, password));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (AuthenticationException ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Unauthorized, ex.Message));
            }
        }

        [HttpPost]
        [Route("api/user/create")]
        public IHttpActionResult CreateUser([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Username))
                return BadRequest("A username must exist");
            if (string.IsNullOrEmpty(user.Password))
                return BadRequest("A password must exist");

            try
            {
                return Ok(_dataAccess.CreateUser(user));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, ex.Message));
            }
        }
    }
}