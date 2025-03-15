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
        private readonly string? webRootPath;

        public DocumentsController(IWebHostEnvironment webHostEnvironment)
        {
            webRootPath = webHostEnvironment.WebRootPath + "\\docs";
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetList()
        {
            var documentResponseModels = new DirectoryInfo(webRootPath).GetFiles()
                .Select(x =>
                    new DocumentResponseModel
                    {
                        Name = x.Name,
                        Path = "https://deloprosit.azurewebsites.net/docs/",
                    }
                );

            return Ok(documentResponseModels);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Upload(IEnumerable<IFormFile> files)
        {
            foreach (IFormFile file in files)
            {
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(webRootPath, file.FileName);
                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                }
            }

            return Ok();
        }
    }
}
