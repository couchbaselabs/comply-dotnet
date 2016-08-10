using System;
using System.Web.Http;
using ComplyWebApi.Models;

namespace ComplyWebApi.Controllers
{
    public class ProjectController : ApiController
    {
        [HttpGet]
        [Route("project/get/{projectId}")]
        public IHttpActionResult GetProjectById(string projectId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("project/getAll/{ownerId}")]
        public IHttpActionResult GetProjectsByOwnerId(string ownerId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("project/getAll")]
        public IHttpActionResult GetProjects()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("project/getOther/{userId}")]
        public IHttpActionResult GetOtherProjectsByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("project/getOther")]
        public IHttpActionResult GetOtherProjects()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("project/create")]
        public IHttpActionResult CreateProject(Project project)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("project/addUser")]
        public IHttpActionResult ProjectAddUser(ProjectUser projectUser)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskController : ApiController
    {
        [HttpGet]
        [Route("task/get/{taskId}")]
        public IHttpActionResult GetTaskById(string taskId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("task/getAssignedTo/{userId}")]
        public IHttpActionResult GetTasksAssignedToUserId(string userId)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("task/create/{projectId}")]
        public IHttpActionResult ProjectAddUser(string projectId, ProjectTask projectTask)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("task/addUser")]
        public IHttpActionResult TaskAddUser(TaskAddUser userTask)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("task/assignUser")]
        public IHttpActionResult TaskAssignUser(TaskAssignUser assignUser)
        {
            throw new NotImplementedException();
        }


        [HttpPost]
        [Route("task/addHistory")]
        public IHttpActionResult TaskAddHistory(TaskAddHistory addHistory)
        {
            throw new NotImplementedException();
        }

    }
}