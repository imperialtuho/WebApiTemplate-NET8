using Asp.Versioning;

namespace Web.Api.Controllers
{
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ExampleController : BaseController
    {
        [HttpGet]
        [MapToApiVersion("1.0")]
        [AllowAnonymous]
        public IActionResult GetV1()
        {
            return Result($"Api version 1.0", HttpStatusCode.OK);
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public IActionResult GetV2()
        {
            return Result($"Api version 2.0 with authorize", HttpStatusCode.OK);
        }
    }
}