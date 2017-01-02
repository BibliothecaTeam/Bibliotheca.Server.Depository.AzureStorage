using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Depository.AzureStorage.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/health")]
    public class HealthController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "I'm alive and reachable";
        }
    }
}