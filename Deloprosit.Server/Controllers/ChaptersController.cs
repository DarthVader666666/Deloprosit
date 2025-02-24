using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        public ChaptersController()
        {
            
        }
    }
}
