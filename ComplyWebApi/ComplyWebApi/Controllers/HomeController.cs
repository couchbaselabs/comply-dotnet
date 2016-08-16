using System.Web.Http;

namespace ComplyWebApi.Controllers
{
    public class HomeController : ApiController
    {
        [Route("")]
        [HttpGet]
        public string Index()
        {
            return "The WebAPI service is running.";
        }         
    }
}