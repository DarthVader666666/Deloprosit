using AutoMapper;

using Deloprosit.Bll.Interfaces;
using Deloprosit.Bll.Services;
using Deloprosit.Data.Entities;
using Deloprosit.Server.Models;

using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IMapper _mapper;

        public ChaptersController(UserManager userManager, IRepository<Chapter> chapterRepository, IMapper mapper)
        {
            _userManager = userManager;
            _chapterRepository = chapterRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Owner, Admin")]
        [Route("[action]")]
        public async Task<IActionResult> Create([FromForm] ChapterCreateModel chapterCreateModel)
        {
            if (chapterCreateModel == null || chapterCreateModel.ChapterTitle.IsNullOrEmpty() || chapterCreateModel.DateCreated == null)
            {
                return BadRequest(new { errorText = "Неверные данные для создания раздела" } );
            }

            var user = await _userManager.GetCurrentUserAsync(HttpContext);

            if (user == null)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            var chapter = new Chapter
            {
                ChapterTitle = chapterCreateModel.ChapterTitle,
                ImagePath = chapterCreateModel.ImagePath,
                DateCreated = chapterCreateModel.DateCreated ?? DateTime.Now,
                UserId = user.UserId
            };

            var createdChapter = await _chapterRepository.CreateAsync(chapter);

            if (createdChapter == null)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            return Ok(createdChapter);
        }


        [HttpDelete]
        [Route("[action]/{chapterId:int}")]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> Delete(int chapterId)
        {
            try
            {
                await _chapterRepository.DeleteAsync(chapterId);
            }
            catch (SqlException)
            {
                return StatusCode(500, new { errorText = "Ошибка базы данных" });
            }

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetList() 
        {
            IEnumerable<Chapter?> chapters;

            try
            {
                chapters = await _chapterRepository.GetListAsync();

                if (chapters == null)
                {
                    return StatusCode(500, new { errorText = "Ошибка сервера" });
                }
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { errorText = ex.Message });
            }

            var response = _mapper.Map<IEnumerable<ChapterResponseModel>>(chapters);

            return Ok(response);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetNodes()
        {
            var chapters = await _chapterRepository.GetListAsync();

            if (chapters == null)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            var response = _mapper.Map<IEnumerable<ChapterNode>>(chapters);

            return Ok(response);
        }

        [HttpGet]
        [Route("[action]/{chapterId:int}")]
        public async Task<IActionResult> Get(int? chapterId)
        {
            if (chapterId == null)
            {
                return BadRequest();
            }

            var chapter = await _chapterRepository.GetAsync(chapterId);

            if (chapter == null)
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            var chapterResponseModel = _mapper.Map<ChapterResponseModel>(chapter);

            return Ok(chapterResponseModel);
        }

        [HttpPut]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Update([FromBody] ChapterUpdateModel chapterUpdateModel)
        {   
            var chapter = _mapper.Map<Chapter>(chapterUpdateModel);

            try
            {
                await _chapterRepository.UpdateAsync(chapter);
            }
            catch (SqlException)
            {
                return StatusCode(500, new { errorText = "Ошибка базы данных" });
            }

            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Search()
        {
            string? searchLine = null;

            try
            {
                var reader = new StreamReader(HttpContext.Request.Body);
                searchLine = JsonConvert.DeserializeObject<SearchLineModel>(await reader.ReadToEndAsync())?.SearchLine;

                if (searchLine == null || searchLine.Length < 3)
                {
                    return Ok(Enumerable.Empty<ChapterSearchResultModel>());
                }
            }
            catch
            {
                return StatusCode(500, new { errorText = "Не удалось прочесть запрос" });
            }

            if (searchLine == null)
            {
                return BadRequest(new { errorText = "Не задана строка поиска" });
            }

            var chapterSearchResultModels = (await _chapterRepository.GetListAsync()).SelectMany(chapter => chapter?.Themes ?? [])
                .Where(theme => theme.Content != null && theme.Content.Contains(searchLine, StringComparison.OrdinalIgnoreCase))
                .SelectMany(theme => theme == null || theme.Content.IsNullOrEmpty() ? [] : GetChapterSearchResultModels(theme, searchLine));

            return Ok(chapterSearchResultModels);
        }

        private static IEnumerable<ChapterSearchResultModel> GetChapterSearchResultModels(Theme theme, string searchLine)
        {
            const int offset = 100;

            var htmlPage = new HtmlDocument();
            htmlPage.LoadHtml(theme!.Content!);
            var rootNode = htmlPage.DocumentNode;
            var nodes = rootNode.ChildNodes.Where(x => x.InnerText.Contains(searchLine, StringComparison.OrdinalIgnoreCase));

            foreach (var node in nodes)
            {
                var childNode = node.ChildNodes.FirstOrDefault(x => x.Name != "#text" && x.InnerText.Contains(searchLine, StringComparison.OrdinalIgnoreCase)) ?? node;
                var content = childNode.InnerText;

                var lastIndex = content.Length;
                var startIndex = 0;

                while (startIndex < lastIndex)
                {
                    var index = content.IndexOf(searchLine, startIndex, StringComparison.OrdinalIgnoreCase);

                    if (index < 0)
                    {
                        break;
                    }

                    var leftOffset = offset;
                    var leftIndex = index - offset;

                    if (leftIndex < 0)
                    {
                        leftOffset = leftIndex + offset;
                        leftIndex = index - leftOffset;
                    }

                    var rightIndex = index + searchLine.Length + offset;
                    var rigthOffset = offset;

                    if (rightIndex > lastIndex)
                    {
                        rigthOffset = lastIndex - (index + searchLine.Length);
                    }

                    var searchFragmentText = content.Substring(leftIndex, leftOffset + searchLine.Length + rigthOffset);
                    var searchLineContent = content.Substring(index, searchLine.Length);
                    searchFragmentText = searchFragmentText.Replace(searchLineContent, $"<span style=\"background:yellow\">{searchLineContent.TrimStart('/')}</span>");
                    var searchFragment = content.Replace(childNode.InnerText, searchFragmentText);

                    var chapterSearchResultModel = new ChapterSearchResultModel
                    {
                        ChapterId = theme.ChapterId,
                        ThemeId = theme.ThemeId,
                        ThemeTitle = theme.ThemeTitle,
                        DateCreated = theme.DateCreated,
                        SearchFragment = 
                            $"<{childNode.Name} style=\"{string.Join(';', childNode.Attributes.Select(attribute => 
                            $"{attribute.Name}:{attribute.DeEntitizeValue}"))}\">{searchFragment}</{childNode.Name}>"
                    };

                    startIndex = index + searchLine.Length;

                    yield return chapterSearchResultModel;
                }
            }
        }
    }
}
