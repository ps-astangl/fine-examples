using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Mediator.Controllers
{
    [ApiController]
    [Route("/api/")]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        [HttpGet, Route("[action]")]
        public async Task<IActionResult> Patient()
        {
            return await Task.Factory.StartNew(() => Ok("Hello World"));
        }
    }
}