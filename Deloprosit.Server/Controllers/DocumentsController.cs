using Deloprosit.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IO;

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
            var documentResponseModels = new DirectoryInfo(webRootPath ?? throw new NullReferenceException("Не задан путь к фалу")).GetFiles()
                .Select(x =>
                    new DocumentResponseModel
                    {
                        Name = x.Name,
                        Path = "https://deloprosit.azurewebsites.net/docs/",
                    }
                );

            return Ok(documentResponseModels);
        }

        [HttpDelete]
        [Route("[action]/{fileName}")]
        [Authorize(Roles = "Owner, Admin")]
        public IActionResult Delete(string fileName)
        {
            try
            {
                System.IO.File.Delete(webRootPath + "\\" + fileName);
            }
            catch
            {
                return Problem(statusCode: 500, detail: "Ошибка при удалении файла");
            }

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            try 
            {
                foreach (IFormFile file in files)
                {
                    if (file.Length > 0)
                    {
                        string filePath = Path.Combine(webRootPath ?? throw new NullReferenceException("Не задан путь к фалу"), file.FileName);
                        using Stream fileStream = new FileStream(filePath, FileMode.Create);
                        await file.CopyToAsync(fileStream);
                    }
                }
            }
            catch
            {
                return Problem(statusCode: 500, detail: "Ошибка загрузки файла");
            }
            
            return Ok();
        }
    }
}
