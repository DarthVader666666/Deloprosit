using Deloprosit.Server.Enums;
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
        private readonly string? docsPath;
        private readonly string? documentsDirectoryName;
        private readonly string? webRootPath;

        public DocumentsController()
        {
            docsPath = ConfigurationHelper.DocsPath;
            webRootPath = ConfigurationHelper.WebRootPath;
            documentsDirectoryName = ConfigurationHelper.DocumentsDirectoryName;
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetList()
        {
            var documentResponseModels = Enumerable.Empty<DocumentResponseModel>();

            try
            {
                documentResponseModels = new DirectoryInfo(docsPath ?? throw new NullReferenceException("Не задан путь к фалу")).GetFiles()
                .Select(x =>
                    new DocumentResponseModel
                    {
                        Name = x.Name,
                        Path = docsPath
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
            List<DocumentNode> documentNodes = [];

            try
            {
                var directoryInfo = new DirectoryInfo(docsPath ?? throw new NullReferenceException("Не задан путь к файлу"));
                var files = directoryInfo.GetFiles();

                documentNodes.Add(new DocumentNode
                {
                    Key = $"{documentsDirectoryName}",
                    Icon = "pi pi-ellipsis-h",
                    Data = new TreeNode 
                    {
                        Path = $"{documentsDirectoryName}",
                        Type = nameof(DocumentType.Root).ToLower(),
                    },
                    Children = files.Select(f => new DocumentNode
                    {
                        Key = $"{documentsDirectoryName}-{f.FullName}",
                        Icon = "pi pi-file",
                        Data = new TreeNode 
                        {
                            Name = f.Name,
                            Path = $"{documentsDirectoryName}/{f.Name}",
                            Type = nameof(DocumentType.File).ToLower(),
                            Size = ByteLengthToSizeString(f.Length), 
                        }
                    }).ToArray()
                });

                documentNodes.AddRange(directoryInfo
                    .GetDirectories()
                        .Select(d => new DocumentNode
                        {
                            Key = d.Name,
                            Icon = "pi pi-folder",
                            Data = new TreeNode 
                            { 
                                Name = d.Name,
                                Path = $"{documentsDirectoryName}/{d.Name}",
                                Type = nameof(DocumentType.Folder).ToLower(),
                            },
                            Children = d.GetFiles().Select(f => new DocumentNode
                            {
                                Key = $"{d.Name}-{f.FullName}",
                                Icon = "pi pi-file",
                                Data = new TreeNode 
                                { 
                                    Name = f.Name,
                                    Path = $"{documentsDirectoryName}/{d.Name}/{f.Name}",
                                    Type = nameof(DocumentType.File).ToLower(),
                                    Size = ByteLengthToSizeString(f.Length), 
                                }
                            }).ToArray()
                        }).ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }

            return Ok(documentNodes);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Delete()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var documentPathModel = JsonConvert.DeserializeObject<DocumentPathModel>(await reader.ReadToEndAsync());

            if (documentPathModel == null || documentPathModel.Path == null || documentPathModel.Type == null)
            {
                return Problem(statusCode: 500, detail: "Ошибка при удалении файла");
            }

            try
            {
                var path = Path.Combine(webRootPath ?? string.Empty, documentPathModel.Path);

                if (documentPathModel.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase))
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    else
                    {
                        return NotFound(new { errorText = "Файл не найден" });
                    }
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        foreach (var filePath in Directory.GetFiles(path))
                        {
                            System.IO.File.Delete(filePath);
                        }

                        Directory.Delete(path);
                    }
                    else
                    {
                        return NotFound(new { errorText = "Папка не найдена" });
                    }
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
            var folderFullName = docsPath + JsonConvert.DeserializeObject<FolderNameModel>(await reader.ReadToEndAsync())?
                .FolderName?.Replace('-', ' ');

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
                    string filePath = Path.Combine(docsPath ?? throw new NullReferenceException("Не задан путь к фалу"), 
                        uploadFileModel.FolderName ?? string.Empty, file.FileName);
                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                }
            }
            catch
            {
                return Problem(statusCode: 500, detail: "Ошибка загрузки файла");
            }
            
            return Ok();
        }

        private static string? ByteLengthToSizeString(long? length)
        {
            return length switch
            {
                >= 1000 and < 999999 => $"{length / 1000} Кб",
                >= 1000000 and < 999999999 => $"{length / 1000000} Mб",
                _ => $"{length} Байт",
            };
        }
    }
}
