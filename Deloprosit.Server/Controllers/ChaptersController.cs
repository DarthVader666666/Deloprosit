using Deloprosit.Bll.Interfaces;
using Deloprosit.Bll.Services;
using Deloprosit.Data.Entities;
using Deloprosit.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly UserManager _userManager;

        public ChaptersController(UserManager userManager, IRepository<Chapter> chapterRepository)
        {
            _userManager = userManager;
            _chapterRepository = chapterRepository;
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
                return Problem(statusCode: 500, detail: "Ошибка сервера");
            }

            var chapter = new Chapter
            {
                ChapterTitle = chapterCreateModel.ChapterTitle,
                DateCreated = chapterCreateModel.DateCreated ?? DateTime.Now,
                UserId = user.UserId
            };

            var createdChapter = await _chapterRepository.CreateAsync(chapter);

            if (createdChapter == null)
            {
                return Problem(statusCode: 500, detail: "Ошибка сервера");
            }

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll() 
        {
            var chapters = await _chapterRepository.GetListAsync();

            if (chapters == null)
            {
                return Problem(statusCode: 500, detail: "Ошибка сервера");
            }

            return Ok(chapters);
        }

        [HttpGet]
        [Route("[action]/{chapterId:int}")]
        public async Task<IActionResult> Get(int chapterId)
        {
            var chapter = await _chapterRepository.GetAsync(chapterId);

            if (chapter == null)
            {
                return Problem(statusCode: 500, detail: "Ошибка сервера");
            }

            return Ok(chapter);
        }

    }
}
