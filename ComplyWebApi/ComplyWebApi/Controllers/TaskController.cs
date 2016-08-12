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
    public class TaskController : ApiController
    {
        private readonly TaskDataAccess _dataAccess;

        public TaskController()
        {
            var bucket = ClusterHelper.GetBucket(ConfigurationManager.AppSettings["CouchbaseBucket"]);
            _dataAccess = new TaskDataAccess(bucket);
        }


        [HttpGet]
        [Route("api/task/get/{taskId}")]
        public IHttpActionResult GetTaskById(string taskId)
        {
            if(string.IsNullOrEmpty(taskId))
                return BadRequest("A task id must exist");
            return Ok(_dataAccess.GetTaskById(taskId));
        }

        [HttpGet]
        [Route("api/task/getAssignedTo/{userId}")]
        public IHttpActionResult GetTasksAssignedToUserId(string userId)
        {
            if(string.IsNullOrEmpty(userId))
                return BadRequest("A user id must exist");
            return Ok(_dataAccess.GetTasksAssignedToUserId(userId));
        }

        [HttpPost]
        [Route("api/task/create/{projectId}")]
        public IHttpActionResult CreateTaskForProjectId(string projectId, Task task)
        {
            if (string.IsNullOrEmpty(task.Owner))
                return BadRequest("Users must exist");
            if (string.IsNullOrEmpty(projectId))
                return BadRequest("A project id must exist");
            return Ok(_dataAccess.CreateTask(projectId, task));
        }

        [HttpPost]
        [Route("api/task/addUser")]
        public IHttpActionResult TaskAddUser(TaskAddUser taskUser)
        {
            if (string.IsNullOrEmpty(taskUser.Username))
                return BadRequest("A username must exist");
            if (string.IsNullOrEmpty(taskUser.TaskId))
                return BadRequest("A task id must exist");
            try
            {
                return Ok(_dataAccess.TaskAddUser(taskUser.TaskId, taskUser.Username));
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

        [HttpPost]
        [Route("api/task/assignUser")]
        public IHttpActionResult TaskAssignUser(TaskAssignUser assignUser)
        {
            if (string.IsNullOrEmpty(assignUser.TaskId))
                return BadRequest("A task id must exist");
            if (string.IsNullOrEmpty(assignUser.UserId))
                return BadRequest("A user id must exist");
            try
            {
                return Ok(_dataAccess.TaskAssignUser(assignUser.TaskId, assignUser.UserId));
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


        [HttpPost]
        [Route("api/task/addHistory")]
        public IHttpActionResult TaskAddHistory(TaskAddHistory addHistory)
        {
            if (string.IsNullOrEmpty(addHistory.TaskId))
                return BadRequest("A task id must exist");
            if (string.IsNullOrEmpty(addHistory.UserId))
                return BadRequest("A user id must exist");
            if (string.IsNullOrEmpty(addHistory.Log))
                return BadRequest("A log must exist");
            try
            {
                return Ok(_dataAccess.TaskAddHistory(addHistory));
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

    }
}