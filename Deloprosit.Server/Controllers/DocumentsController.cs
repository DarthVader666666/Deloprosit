using Deloprosit.Bll;
using Deloprosit.Bll.Services;
using Deloprosit.Server.Enums;
using Deloprosit.Server.Models;

using Google;

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

        private readonly GoogleDriveService _googleDriveService;

        public DocumentsController(GoogleDriveService googleDriveService)
        {
            docsPath = ConfigurationHelper.DocsPath;
            webRootPath = ConfigurationHelper.WebRootPath;
            documentsDirectoryName = ConfigurationHelper.DocsFolderName;
            _googleDriveService = googleDriveService;   
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
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
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
                            Path = Path.Combine(documentsDirectoryName ?? string.Empty, f.Name),
                            Type = nameof(DocumentType.File).ToLower(),
                            Size = ByteLengthToSizeString(f.Length), 
                        }
                    })
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
                                Path = Path.Combine(documentsDirectoryName ?? string.Empty, d.Name),
                                Type = nameof(DocumentType.Folder).ToLower(),
                            },
                            Children = d.GetFiles().Select(f => new DocumentNode
                            {
                                Key = $"{d.Name}-{f.FullName}",
                                Icon = "pi pi-file",
                                Data = new TreeNode
                                {
                                    Name = f.Name,
                                    Path = Path.Combine(documentsDirectoryName ?? string.Empty, d.Name, f.Name),
                                    Type = nameof(DocumentType.File).ToLower(),
                                    Size = ByteLengthToSizeString(f.Length), 
                                }
                            })
                        }));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
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
                return StatusCode( 500, new { errorText = "Ошибка при удалении файла" });
            }

            try
            {
                var path = Path.Combine(webRootPath ?? string.Empty, documentPathModel.Path);

                if (documentPathModel.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase))
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        Task.Run(() => _googleDriveService.Delete(path));
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
                        Task.Run(() => _googleDriveService.Delete(path, isFolder: true));
                    }
                    else
                    {
                        return NotFound(new { errorText = "Папка не найдена" });
                    }
                }
            }
            catch(Exception ex)
            {
                return StatusCode( StatusCodes.Status500InternalServerError, new { errorText = $"Ошибка при удалении\n\r{ex.Message}" });
            }

            return Ok(new { okText = documentPathModel.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase) ? "Файл успешно удален" : "Папка успешно удалена" });
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> AddFolder()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var path = Path.Combine(docsPath ?? "", JsonConvert.DeserializeObject<FolderNameModel>(await reader.ReadToEndAsync())?.FolderName?.Replace('-', ' ') ?? "");

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path ?? throw new NullReferenceException());
                    Task.Run(() => _googleDriveService.CreateFolder(path.Split('\\').Last()));
                }
                else
                {
                    return BadRequest(new { errorText = "Папка уже существует" });
                }
            }
            catch (GoogleApiException ex)
            {
                return StatusCode(StatusCodes.Status304NotModified, new { warningText = "Папка не была создана в облаке" });
            }
            catch
            {
                return StatusCode( StatusCodes.Status500InternalServerError, new { errorText = "Ошибка при создании папки" });
            }

            return Ok(new { okText = "Папка успешно создана" });
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
                var filePath = "";

                foreach (IFormFile file in uploadFileModel.Files)
                {
                    filePath = Path.Combine(docsPath ?? throw new NullReferenceException("Не задан путь к файлу"),
                        uploadFileModel.FolderName ?? string.Empty, file.FileName);

                    var overwrite = System.IO.File.Exists(filePath);

                    using Stream fileStream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(fileStream);
                    fileStream.Close();

                    if (System.IO.File.Exists(filePath))
                    {
                        Task.Run(() => _googleDriveService.CreateFile(filePath, overwrite));
                    }
                }
            }
            catch (GoogleApiException ex)
            {
                return StatusCode(StatusCodes.Status304NotModified, new { warningText = "Файл не был создан в облаке" });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Ошибка загрузки файла" });
            }

            return Ok(new { okText = "Файл успешно загружен" });
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public IActionResult Update([FromBody] UpdateDocumentModel? updateDocumentModel)
        {
            if (updateDocumentModel == null || updateDocumentModel.NewName == null || updateDocumentModel.Path == null || updateDocumentModel.Type == null)
            {
                return BadRequest(new { errorText = "Запрос не полный" });
            }

            try
            {
                var path = Path.Combine(webRootPath ?? string.Empty, Path.Combine(updateDocumentModel.Path.Split('\\')[..^1]));
                var sourcePath = Path.Combine(webRootPath ?? string.Empty, updateDocumentModel.Path);
                var destPath = Path.Combine(path, updateDocumentModel.NewName);

                if (updateDocumentModel.Type.Equals(nameof(DocumentType.Folder), StringComparison.OrdinalIgnoreCase))
                {
                    Directory.Move(sourcePath, destPath);
                }
                else if (updateDocumentModel.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase))
                {
                    System.IO.File.Move(sourcePath, destPath);
                }
                else
                {
                    return BadRequest(new { errorText = "Не указан тип документа" });
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = "Папка уже существует" });
            }

            return Ok(new { okText = "Имя успешно обновлено" });
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
