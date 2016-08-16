using System;
using System.Collections.Generic;
using System.Security.Authentication;
using ComplyWebApi.Models.DocumentModels;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;
using DevOne.Security.Cryptography.BCrypt;

namespace ComplyWebApi.Models.DataAccess
{
    public class UserDataAccess : BaseDataAccess
    {
        private readonly IBucket _bucket;

        public UserDataAccess(IBucket bucket)
        {
            _bucket = bucket;
        }

        public List<User> GetUserById(string userId)
        {
            var queryStr = @"SELECT users.*
                            FROM `" + _bucket.Name + @"` AS users
                            WHERE _type = 'User' AND META(users).id = $1";
            var query = QueryRequest.Create(queryStr);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            query.AddPositionalParameter(userId);
            var queryResult = _bucket.Query<User>(query);
            return ExtractResultOrThrow(queryResult);
        }

        public List<User> GetUsers()
        {
            var queryStr = "SELECT u.* FROM `" + _bucket.Name + @"` u WHERE _type = 'User'";
            var query = QueryRequest.Create(queryStr);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            var queryResult = _bucket.Query<User>(query);
            return ExtractResultOrThrow(queryResult);
        }

        /// <summary>
        /// Attempt to get a document by the username which also represents a document key.If the document is found, use Bcrypt to
        /// validate the password.  If everything succeeds, return the user document itself.  The password returned will be Bcrypted, not plain text.
        /// </summary>
        public User Login(string username, string password)
        {
            var user = _bucket.Get<User>(username);
            if (!user.Success)
                throw new ArgumentException("The username provided does not exist or was not correct");

            if (BCryptHelper.CheckPassword(password, user.Value.Password))
                return user.Value;

            throw new AuthenticationException("The password provided is not correct");
        }

        public User CreateUser(User user)
        {
            user.Password = BCryptHelper.HashPassword(user.Password, BCryptHelper.GenerateSalt());
            user._type = "User";
            user._id = user.Username;   
            var userDocument = new Document<User>
            {
                Id = user.Username,
                Content = user 
            };
            _bucket.Insert(userDocument);
            return user;
        }
    }
}