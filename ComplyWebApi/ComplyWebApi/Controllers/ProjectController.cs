using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using ComplyWebApi.Models.DataAccess;
using ComplyWebApi.Models.DocumentModels;
using ComplyWebApi.Models.EditModels;
using Couchbase;

namespace ComplyWebApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ProjectController : ApiController
    {
        private readonly ProjectDataAccess _dataAccess;

        public ProjectController()
        {
            var bucket = ClusterHelper.GetBucket(ConfigurationManager.AppSettings["CouchbaseBucket"]);
            _dataAccess = new ProjectDataAccess(bucket);
        }

        [HttpGet]
        [Route("api/project/get/{projectId}")]
        public IHttpActionResult GetProjectById(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
                return BadRequest("A project is must exist");
            return Ok(_dataAccess.GetProjectById(projectId));
        }

        [HttpGet]
        [Route("api/project/getAll/{ownerId}")]
        public IHttpActionResult GetProjectsByOwnerId(string ownerId)
        {
            if (string.IsNullOrEmpty(ownerId))
                return BadRequest("An owner id must exist");
            return Ok(_dataAccess.GetProjectsByOwnerId(ownerId));
        }

        [HttpGet]
        [Route("api/project/getAll")]
        public IHttpActionResult GetProjects()
        {
            return Ok(_dataAccess.GetProjects());
        }

        [HttpGet]
        [Route("api/project/getOther/{userId}")]
        public IHttpActionResult GetOtherProjectsByUserId(string userId)
        {
            if(string.IsNullOrEmpty(userId))
                return BadRequest("A user id must exist");
            return Ok(_dataAccess.GetOtherProjectsByUserId(userId));
        }

        [HttpPost]
        [Route("api/project/create")]
        public IHttpActionResult CreateProject([FromBody] Project project)
        {
            if (string.IsNullOrEmpty(project.Owner))
                return BadRequest("An owner must exist");
            if (string.IsNullOrEmpty(project.Name))
                return BadRequest("A name must exist");
            if (string.IsNullOrEmpty(project.Description))
                return BadRequest("A description must exist");
            try
            {
                return Ok(_dataAccess.CreateProject(project));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, ex.Message));
            }
        }

        [HttpPost]
        [Route("api/project/addUser")]
        public IHttpActionResult ProjectAddUser([FromBody] ProjectAddUser projectAddUser)
        {
            if (string.IsNullOrEmpty(projectAddUser.Username))
                return BadRequest("A username must exist");
            if (string.IsNullOrEmpty(projectAddUser.ProjectId))
                return BadRequest("A project id must exist");
            try
            {
                return Ok(_dataAccess.ProjectAddUser(projectAddUser.Username, projectAddUser.ProjectId));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.Conflict, ex.Message));
            }
        }

        [HttpGet]
        [Route("api/project/getOther")]
        public IHttpActionResult GetOtherProjects()
        {
            return Ok(_dataAccess.GetProjects());
        }
    }
}