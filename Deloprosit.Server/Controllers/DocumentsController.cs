using Deloprosit.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Deloprosit.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly string? webRootPath;

        public DocumentsController()
        {
            webRootPath = ConfigurationHelper.WebRootPath;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetList()
        {
            var documentResponseModels = Enumerable.Empty<DocumentResponseModel>();

            try
            {
                documentResponseModels = new DirectoryInfo(webRootPath ?? throw new NullReferenceException("Не задан путь к фалу")).GetFiles()
                .Select(x =>
                    new DocumentResponseModel
                    {
                        Name = x.Name,
                        Path = webRootPath
                    }
                );
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }            

            return Ok(documentResponseModels);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetNodes()
        {
            List<DirectoryNode> directoryNodes = [];

            try
            {
                var directoryInfo = new DirectoryInfo(webRootPath ?? throw new NullReferenceException("Не задан путь к файлу"));
                var files = directoryInfo.GetFiles();

                if (files.Any())
                {
                    directoryNodes.Add(new DirectoryNode
                    {
                        Key = "../",
                        Label = "",
                        Icon = "pi pi-ellipsis-h",
                        Children = files.Select(f => new DocumentNode
                        {
                            Key = $"{webRootPath}-{f.FullName}",
                            Label = f.Name,
                            Data = $"docs/{f.Name}"
                        }).ToArray()
                    });
                }

                directoryNodes.AddRange(directoryInfo
                    .GetDirectories()
                        .Select(d => new DirectoryNode
                        {
                            Key = d.FullName,
                            Label = d.Name,
                            Children = d.GetFiles().Select(f => new DocumentNode
                            {
                                Key = $"{d.FullName}-{f.Name}",
                                Label = f.Name,
                                Data = $"docs/{d.Name}/{f.Name}"
                            }).ToArray()
                        }).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }

            return Ok(directoryNodes);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Delete()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var filePath = JsonConvert.DeserializeObject<FilePathModel>(await reader.ReadToEndAsync())?.FilePath;

            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath ?? throw new NullReferenceException());
                }
                else
                {
                    return NotFound();
                }
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
        public async Task<IActionResult> AddFolder()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var folderFullName = webRootPath + JsonConvert.DeserializeObject<FolderNameModel>(await reader.ReadToEndAsync())?.FolderName;

            try
            {
                if (!Directory.Exists(folderFullName))
                {
                    Directory.CreateDirectory(folderFullName ?? throw new NullReferenceException());
                }
                else
                {
                    return BadRequest(new { errorText = "Папка уже существует" });
                }
            }
            catch
            {
                return Problem(statusCode: 500, detail: "Ошибка при создании папки");
            }

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Upload([FromForm] UploadFileModel? uploadFileModel)
        {
            if (uploadFileModel == null || uploadFileModel.Files == null || !uploadFileModel.Files.Any())
            {
                return BadRequest(new { errorText = "Нет выбранных файлов" });
            }

            try 
            {
                foreach (IFormFile file in uploadFileModel.Files)
                {
                    if (file.Length > 0)
                    {
                        string filePath = Path.Combine((webRootPath ?? throw new NullReferenceException("Не задан путь к фалу")) + uploadFileModel.FolderName, file.FileName);
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
