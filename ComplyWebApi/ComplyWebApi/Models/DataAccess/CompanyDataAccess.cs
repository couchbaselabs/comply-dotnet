﻿using System;
using System.Collections.Generic;
using ComplyWebApi.Models.DocumentModels;
using Couchbase;
using Couchbase.Core;
using Couchbase.N1QL;

namespace ComplyWebApi.Models.DataAccess
{
    public class CompanyDataAccess : BaseDataAccess
    {
        private readonly IBucket _bucket;

        public CompanyDataAccess(IBucket bucket)
        {
            _bucket = bucket;
        }

        public List<Company> GetCompanyById(string companyId)
        {
            var queryStr = "SELECT companies.* FROM `" + _bucket.Name + @"` AS companies
                           WHERE _type = 'Company' AND META(companies).id = $1";
            var query = QueryRequest.Create(queryStr);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            query.AddPositionalParameter(companyId);
            var queryResult = _bucket.Query<Company>(query);
            return ExtractResultOrThrow(queryResult);
        }

        public List<Company> GetCompanies()
        {
            var queryStr = "SELECT c.* FROM `" + _bucket.Name + "` c WHERE _type = 'Company'";
            var query = QueryRequest.Create(queryStr);
            query.ScanConsistency(ScanConsistency.RequestPlus);
            return ExtractResultOrThrow(_bucket.Query<Company>(query));
        }

        public Company CreateCompany(Company company)
        {
            company._id = company.Website;
            company._type = "Company";
            var companyDoc = new Document<Company>
            {
                Id = company.Website,
                Content = company
            };
            _bucket.Insert(companyDoc);
            return company;
        }
    }
}