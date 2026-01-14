using Microsoft.AspNetCore.Mvc;

namespace Universum_OnlineCourses.API.Controllers
{
      [ApiController]
        [Route("api/test")]
        public class TestController : ControllerBase
        {
            [HttpGet]
            public IActionResult Get()
            {
                return Ok("PostgreSQL API is running 🚀");
            }
        }
    
}
