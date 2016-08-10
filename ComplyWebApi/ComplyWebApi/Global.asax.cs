using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using Couchbase;
using Couchbase.Configuration.Client;

namespace ComplyWebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            var config = new ClientConfiguration();
            config.Servers = new List<Uri> { new Uri(ConfigurationManager.AppSettings["CouchbaseServer"]) };
            config.UseSsl = false;
            ClusterHelper.Initialize(config);
        }
        protected void Application_End()
        {
            ClusterHelper.Close();
        }
    }
}
