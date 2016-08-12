using System;
using System.Collections.Generic;
using System.Reflection;
using ComplyWebApi.Models.DocumentModels;
using ComplyWebApi.Models.EditModels;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;

namespace ComplyWebApi.Models.DataAccess
{
    public class TaskDataAccess : BaseDataAccess
    {
        private readonly IBucket _bucket;

        public TaskDataAccess(IBucket bucket)
        {
            _bucket = bucket;
        }

        public List<dynamic> GetTaskById(string taskId)
        {
            var queryStr = @"SELECT _id, 
                                (SELECT a.* FROM `" + _bucket.Name + @"` a USE KEYS c.assignedTo)[0] AS assignedTo,
                                createdON,
                                description,
                                history,
                                name,
                                (SELECT o.* FROM `" + _bucket.Name + @"` o USE KEYS c.owner)[0] as owner,
                                (SELECT u.* FROM `" + _bucket.Name + @"` u USE KEYS c.users) AS users,
                                permalink
                            FROM `" + _bucket.Name + @"` c
                            WHERE c.assignedTo = $1";
            var query = QueryRequest.Create(queryStr);
            query.AddPositionalParameter(taskId);
            var queryResult = _bucket.Query<dynamic>(query);
            return ExtractResultOrThrow(queryResult);
        }

        public List<dynamic> GetTasksAssignedToUserId(string userId)
        {
            var queryStr = @"SELECT
                                p._id AS projectId,
                                (SELECT _id, 
                                    (SELECT a.* FROM `" + _bucket.Name + @"` a USE KEYS c.assignedTo)[0] AS assignedTo,
                                    createdON, 
                                    description,
                                    (select
                                        t.log,
                                        t.createdAt, 
                                        (SELECT u.* FROM `" + _bucket.Name + @"` u USE KEYS t.`user`)[0] AS `user` FROM `" + _bucket.Name + @"` r UNNEST r.history t WHERE r._id = $1) AS history,
                                        name, 
                                        (SELECT o.*FROM `" + _bucket.Name + @"` o USE KEYS c.owner)[0] as owner,
                                        (SELECT u.* FROM `" + _bucket.Name + @"` u USE KEYS c.users) AS users, permalink
                                    FROM `" + _bucket.Name + @"` c
                                    WHERE c._id= $1)[0] AS task
                            FROM `" + _bucket.Name + @"` p
                            WHERE ANY x IN tasks SATISFIES x = $1 END ";
            var query = QueryRequest.Create(queryStr);
            query.AddPositionalParameter(userId);
            var queryResult = _bucket.Query<dynamic>(query);
            return ExtractResultOrThrow(queryResult);
        }

        public List<dynamic> CreateTask(string projectId, Task task)
        {
            // create task
            var taskId = Guid.NewGuid().ToString();
            task.Users.Add(task.Owner);
            task._id = taskId;
            task._type = "Task";
            task.CreatedOn = DateTime.Now.ToShortDateString();
            var taskDocument = new Document<dynamic>
            {
                Id = taskId,
                Content = task
            };
            _bucket.Upsert(taskDocument);

            // add task to project
            var projectDocument = _bucket.Get<Project>(projectId);
            if(projectDocument == null || !projectDocument.Success)
                throw new ArgumentException("The project id does not exist");
            var project = projectDocument.Value;
            project.Tasks.Add(taskId);
            var updatedProjectDocument = new Document<Project>
            {
                Id = projectDocument.Id,
                Content = project
            };
            _bucket.Upsert(updatedProjectDocument);

            // return task with full owner/users information
            String queryStr = @"SELECT c._id,
                                        c.createdON,
                                        c.name,
                                        c.description,
                                        (SELECT o.* FROM `" + _bucket.Name + @"` o USE KEYS c.owner)[0] AS owner,
                                        c.status,
                                        (SELECT u.* FROM `" + _bucket.Name + @"` u USE KEYS c.users) AS users,
                                        c.permalink
                                FROM `" + _bucket.Name + @"` c
                                WHERE c._id = $1";
            var query = QueryRequest.Create(queryStr);
            query.AddPositionalParameter(taskId);
            var queryResult = _bucket.Query<dynamic>(query);
            return ExtractResultOrThrow(queryResult);
        }

        public User TaskAddUser(string taskId, string username)
        {
            // get the task/user documents
            var userDocument = _bucket.Get<User>(username);
            if(userDocument == null || !userDocument.Success)
                throw new ArgumentException("The user id does not exist");
            var taskDocument = _bucket.Get<Task>(taskId);
            if(taskDocument == null || !taskDocument.Success)
                throw new ArgumentException("The task id does not exist");

            var user = userDocument.Value;
            var task = taskDocument.Value;

            // add the user to the task
            if(!task.Users.Contains(username))
                task.Users.Add(username);

            // save the task
            var updatedTaskDocument = new Document<Task>
            {
                Id = taskDocument.Id,
                Content = task
            };
            _bucket.Upsert(updatedTaskDocument);
            return user;
        }

        public User TaskAssignUser(string taskId, string userId)
        {
            // get documents
            var userDocument = _bucket.Get<User>(userId);
            if(userDocument == null || !userDocument.Success)
                throw new ArgumentException("The user id does not exist");

            var taskDocument = _bucket.Get<Task>(taskId);
            if (taskDocument == null || !taskDocument.Success)
                throw new ArgumentException("The task id does not exist");

            // update task
            var task = taskDocument.Value;
            task.AssignedTo = userId;
            var updatedTaskDocument = new Document<Task>
            {
                Id = taskDocument.Id,
                Content = task
            };
            _bucket.Upsert(updatedTaskDocument);

            // return user
            return userDocument.Value;
        }

        public List<dynamic> TaskAddHistory(TaskAddHistory addHistory)
        {
            // get the task
            var taskDocument = _bucket.Get<Task>(addHistory.TaskId);
            if(taskDocument == null || !taskDocument.Success)
                throw new ArgumentException("The task id does not exist");

            // add a new history item to it
            var createdAt = DateTime.Now.ToShortDateString();
            var task = taskDocument.Value;
            task.History.Add(new History
            {
                Log = addHistory.Log,
                User = addHistory.UserId,
                CreatedAt = createdAt
            });

            // save the task
            var updatedTask = new Document<Task>
            {
                Id = taskDocument.Id,
                Content = task
            };
            _bucket.Upsert(updatedTask);

            // return
            var queryStr = @"SELECT ($1) AS log, 
                                    (SELECT u.* FROM `" + _bucket.Name + @"` u USE KEYS c._id)[0] AS `user`,
                                    ($2) AS createdAt
                            FROM `" + _bucket.Name + @"` c
                            WHERE c._id = $3";
            var query = QueryRequest.Create(queryStr);
            query.AddPositionalParameter(addHistory.Log);
            query.AddPositionalParameter(createdAt);
            query.AddPositionalParameter(addHistory.UserId);
            var queryResult = _bucket.Query<dynamic>(query);
            return ExtractResultOrThrow(queryResult);
        }
    }
}