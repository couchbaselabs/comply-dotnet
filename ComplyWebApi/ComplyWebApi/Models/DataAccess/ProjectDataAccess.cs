using System;
using System.Collections.Generic;
using ComplyWebApi.Models.DocumentModels;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;

namespace ComplyWebApi.Models.DataAccess
{
    public class ProjectDataAccess : BaseDataAccess
    {
        private readonly IBucket _bucket;

        public ProjectDataAccess(IBucket bucket)
        {
            _bucket = bucket;
        }

        public List<dynamic> GetProjectById(string projectId)
        {
            var queryStr = @"SELECT
                                _id,
                                createdON,
                                description,
                                name,
                                (SELECT o.* FROM `" + _bucket.Name + @"` o USE KEYS c.owner)[0] as owner,
                                (SELECT u.* FROM `" + _bucket.Name + @"` u USE KEYS c.users) AS users,
                                (SELECT t.* FROM `" + _bucket.Name + @"` t USE KEYS c.tasks) as tasks,
                                permalink
                            FROM `" + _bucket.Name + @"` c
                            WHERE c._id = $1";
            var query = QueryRequest.Create(queryStr);
            query.AddPositionalParameter(projectId);
            var queryResult = _bucket.Query<dynamic>(query);
            return ExtractResultOrThrow(queryResult);
        }

        public List<Project> GetProjectsByOwnerId(string ownerId)
        {
            var queryStr = "SELECT p.* FROM `" + _bucket.Name + "` p WHERE _type = 'Project' and owner = $1";
            var query = QueryRequest.Create(queryStr);
            query.AddPositionalParameter(ownerId);
            var queryResult = _bucket.Query<Project>(query);
            return ExtractResultOrThrow(queryResult);
        }

        public List<Project> GetProjects()
        {
            var queryStr = "SELECT projects.* FROM `" + _bucket.Name + "` AS projects WHERE _type = 'Project'";
            var query = QueryRequest.Create(queryStr);
            return ExtractResultOrThrow(_bucket.Query<Project>(query));
        }

        public List<Project> GetOtherProjectsByUserId(string userId)
        {
            var queryStr = "SELECT p.* FROM `" + _bucket.Name + "` p WHERE _type = 'Project' AND ANY x IN users SATISFIES x = $1 END";
            var query = QueryRequest.Create(queryStr);
            query.AddPositionalParameter(userId);
            var queryResult = _bucket.Query<Project>(query);
            return ExtractResultOrThrow(queryResult);
        }

        public Project CreateProject(Project project)
        {
            var documentId = Guid.NewGuid();
            project.Users = new List<string> { project.Owner };
            project._id = documentId.ToString();
            project.CreatedOn = DateTime.Now.ToShortDateString();
            project._type = "Project";
            var document = new Document<Project>
            {
                Id = documentId.ToString(),
                Content = project
            };
            _bucket.Upsert(document);
            return project;
        }

        public User ProjectAddUser(string username, string projectId)
        {
            var userDocument = _bucket.Get<User>(username);
            if(userDocument == null || !userDocument.Success)
                throw new ArgumentException("The user does not exists");
            var projectDocument = _bucket.Get<Project>(projectId);
            if(projectDocument == null || !projectDocument.Success)
                throw new ArgumentException("The project does not exist");

            var project = projectDocument.Value;
            if(!project.Users.Contains(username))
                project.Users.Add(username);

            _bucket.Upsert(new Document<Project>
            {
                Id = projectDocument.Id,
                Content = project
            });

            return userDocument.Value;
        }
    }
}