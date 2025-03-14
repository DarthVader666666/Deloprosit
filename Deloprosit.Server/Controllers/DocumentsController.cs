using Deloprosit.Server.Models;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Deloprosit.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DocumentsController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetList()
        {
            var documentResponseModels = new DirectoryInfo(_webHostEnvironment.WebRootPath).GetFiles()
                .Select(x => 
                    new DocumentResponseModel 
                    {
                        Name = x.Name,
                        Path = x.FullName
                    }
                );

            return Ok(documentResponseModels);
        }
    }
}
