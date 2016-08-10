using System.Collections.Generic;
using System.Data;
using System.Linq;
using Couchbase.N1QL;

namespace ComplyWebApi.Models.DataAccess
{
    public abstract class BaseDataAccess
    {
        protected List<T> ExtractResultOrThrow<T>(IQueryResult<T> queryResult)
        {
            if (!queryResult.Success)
                throw new DataException("Query error: " + string.Join(",",queryResult.Errors.Select(e => e.Message)));
            return queryResult.Rows;
        }
    }
}