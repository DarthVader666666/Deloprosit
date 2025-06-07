using Delopro.Bll;
using Delopro.Bll.Interfaces;
using Delopro.Server.Enums;
using Delopro.Server.Models;

using Google;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly string? docsPath;
        private readonly string? documentsDirectoryName;
        private readonly string? webRootPath;

        private readonly IDriveService _driveService;

        public DocumentsController(IDriveService driveService)
        {
            docsPath = ConfigurationHelper.DocsPath;
            webRootPath = ConfigurationHelper.WebRootPath;
            documentsDirectoryName = ConfigurationHelper.DocsFolderName;
            _driveService = driveService;
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

        void FillNodes(string? path, DocumentNode? parentNode = null)
        {
            parentNode ??= new DocumentNode();

            var rootDirectoryInfo = new DirectoryInfo(path);
            var directoryInfoArray = rootDirectoryInfo.GetDirectories();
            var shortPath = path.Replace(webRootPath + Path.DirectorySeparatorChar, "");

            parentNode.Key = $"{shortPath.Replace(Path.DirectorySeparatorChar, '-') + (shortPath == documentsDirectoryName ? "" : '-' + path)}";
            parentNode.Icon = shortPath == documentsDirectoryName ? "pi pi-ellipsis-h" : "pi pi-folder";
            parentNode.Data = new TreeNode
            {
                Name = shortPath == documentsDirectoryName ? "" : rootDirectoryInfo.Name,
                Path = shortPath,
                Type = shortPath == documentsDirectoryName ? nameof(DocumentType.Root).ToLower() : nameof(DocumentType.Folder).ToLower(),
            };

            foreach (var directoryInfo in directoryInfoArray ?? [])
            {
                var node = new DocumentNode();
                parentNode.Children?.Add(node);
                FillNodes(directoryInfo.FullName, node);
            }

            var files = rootDirectoryInfo.GetFiles();
            var fileNodes = files.Select(f => new DocumentNode
            {
                Key = $"{shortPath.Replace(Path.DirectorySeparatorChar, '-')}-{f.FullName}",
                Icon = "pi pi-file",
                Data = new TreeNode
                {
                    Name = f.Name,
                    Path = Path.Combine(shortPath ?? string.Empty, f.Name),
                    Type = nameof(DocumentType.File).ToLower(),
                    Size = ByteLengthToSizeString(f.Length),
                }
            });

            parentNode.Children?.AddRange(fileNodes);
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetNodes()
        {
            try
            {
                var node = new DocumentNode();
                FillNodes(docsPath, node);

                return Ok(new List<DocumentNode> { node });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = ex.Message });
            }
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

            var path = Path.Combine(webRootPath ?? string.Empty, documentPathModel.Path);

            try
            {
                if (documentPathModel.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase))
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                        Task.Run(() => _driveService.Delete(path));
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

                        Directory.Delete(path, recursive: true);
                        Task.Run(() => _driveService.Delete(path, isFolder: true));
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

            return Ok(new { okText = documentPathModel.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase) 
                ? $"Файл \"{Path.GetFileName(path)}\" успешно удален" 
                : $"Папка \"{path.Split(Path.DirectorySeparatorChar).Last()}\" успешно удалена" });
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> AddFolder()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var folderPath = JsonConvert.DeserializeObject<FolderPathModel>(await reader.ReadToEndAsync())?.FolderPath?.Replace('-', ' ');
            var path = Path.Combine(docsPath!, folderPath!);

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path ?? throw new NullReferenceException());
                    Task.Run(() => _driveService.CreateFolder(folderPath));
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

            return Ok(new { okText = $"Папка \"{folderPath}\" успешно создана" });
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public IActionResult Upload([FromForm] UploadFileModel? uploadFileModel)
        {
            if (uploadFileModel == null || uploadFileModel.Files == null || !uploadFileModel.Files.Any())
            {
                return BadRequest(new { errorText = "Нет выбранных файлов" });
            }

            var fileNames = new List<string>();
            var filePaths = new List<string>();

            try
            {
                foreach (IFormFile file in uploadFileModel.Files)
                {
                    fileNames.Add(file.FileName);

                    var filePath = Path.Combine(docsPath ?? throw new NullReferenceException("Не задан путь к файлу"),
                        uploadFileModel.FolderName ?? string.Empty, file.FileName);

                    if (!System.IO.File.Exists(filePath))
                    {
                        filePaths.Add(filePath);

                        using Stream fileStream = new FileStream(filePath, FileMode.Create);
                        file.CopyTo(fileStream);
                    }
                }

                Task.Run(() =>
                {
                    foreach (var filePath in filePaths)
                    {
                        if (System.IO.File.Exists(filePath))
                        {
                            _driveService.CreateFile(filePath);
                        }
                    }
                });                
            }
            catch (GoogleApiException ex)
            {
                return StatusCode(StatusCodes.Status304NotModified, new { warningText = $"Файл \"{fileNames}\" не был создан в облаке" });
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = $"Ошибка загрузки файла \"{fileNames}\"" });
            }

            return Ok(new
            {
                okText = fileNames?.Count() > 1
                    ? $"Файлы \"{string.Join(", ", fileNames)}\" успешно загружены"
                    : $"Файл \"{fileNames?.First()}\" успешно загружен"
            });
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
                var path = Path.Combine(webRootPath ?? string.Empty, Path.Combine(updateDocumentModel.Path.Split(Path.DirectorySeparatorChar)[..^1]));
                var sourcePath = Path.Combine(webRootPath ?? string.Empty, updateDocumentModel.Path);
                var destPath = Path.Combine(path, updateDocumentModel.NewName);

                if (updateDocumentModel.Type.Equals(nameof(DocumentType.Folder), StringComparison.OrdinalIgnoreCase))
                {
                    Directory.Move(sourcePath, destPath);
                    Task.Run(() => _driveService.Rename(sourcePath, updateDocumentModel.NewName, isFolder: true));
                }
                else if (updateDocumentModel.Type.Equals(nameof(DocumentType.File), StringComparison.OrdinalIgnoreCase))
                {
                    System.IO.File.Move(sourcePath, destPath);
                    Task.Run(() => _driveService.Rename(sourcePath, updateDocumentModel.NewName));
                }
                else
                {
                    return BadRequest(new { errorText = "Не указан тип документа" });
                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = $"Ошибка при переименовании" });
            }

            return Ok(new { okText = "Имя успешно обновлено" });
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Move()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var moveFileModel = JsonConvert.DeserializeObject<MoveFileModel>(await reader.ReadToEndAsync());

            if (moveFileModel == null || moveFileModel.OldPath == null || moveFileModel.NewPath == null)
            {
                return BadRequest(new { errorText = "Не указан путь для перемещения файла" });
            }

            var oldPath = Path.Combine(webRootPath!, Path.Combine(moveFileModel.OldPath.Split(Path.DirectorySeparatorChar)));
            var newPath = Path.Combine(webRootPath!, documentsDirectoryName!, Path.Combine(moveFileModel.NewPath.Split(Path.DirectorySeparatorChar)));

            if (oldPath == newPath)
            {
                return BadRequest(new { errorText = "Пути совпадают" });
            }

            if (System.IO.File.Exists(newPath))
            {
                return BadRequest(new { errorText = $"Файл с именем \"{newPath.Split(Path.DirectorySeparatorChar).Last()}\" уже существует" });
            }

            try
            {
                var overwrite = System.IO.File.Exists(newPath);
                Directory.Move(oldPath, newPath);

                Task.Run(() =>
                {
                    _driveService.Delete(oldPath);

                    if (overwrite)
                    {
                        _driveService.Delete(newPath);
                    }

                    _driveService.CreateFile(newPath);
                });                

                return Ok(new { okText = $"Файл \"{Path.GetFileName(oldPath)}\" успешно перемещен" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { errorText = $"Ошибка при перемещении файла: {ex.Message}" });
            }
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
