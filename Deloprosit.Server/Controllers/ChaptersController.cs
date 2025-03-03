﻿using AutoMapper;

using Deloprosit.Bll.Interfaces;
using Deloprosit.Bll.Services;
using Deloprosit.Data.Entities;
using Deloprosit.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("[controller]")]
    [ApiController]
    public class ChaptersController : ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IRepository<Chapter> _chapterRepository;
        private readonly IRepository<Theme> _themeRepository;
        private readonly IMapper _mapper;

        public ChaptersController(UserManager userManager, IRepository<Chapter> chapterRepository, 
            IRepository<Theme> themeRepository, IMapper mapper)
        {
            _userManager = userManager;
            _chapterRepository = chapterRepository;
            _themeRepository = themeRepository;
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
                return Problem(statusCode: 500, detail: "Ошибка сервера");
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
                return Problem(statusCode: 500, detail: "Ошибка сервера");
            }

            return Ok();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetList() 
        {
            var chapters = await _chapterRepository.GetListAsync();

            if (chapters == null)
            {
                return Problem(statusCode: 500, detail: "Ошибка сервера");
            }

            var response = _mapper.Map<IEnumerable<ChapterResponseModel>>(chapters);

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
                return Problem(statusCode: 500, detail: "Ошибка сервера");
            }

            var chapterResponseModel = _mapper.Map<ChapterResponseModel>(chapter);

            return Ok(chapterResponseModel);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Owner, Admin")]
        public async Task<IActionResult> Update([FromBody] ChapterUpdateModel chapterUpdateModel)
        {   
            var userId = (await _userManager.GetCurrentUserAsync(HttpContext))?.UserId;

            if (userId == null) {
                return Problem(statusCode: 500, detail: "Пользователь не найден");
            }

            var chapter = _mapper.Map<Chapter>(chapterUpdateModel);

            try
            {
                await _chapterRepository.UpdateAsync(chapter);

                foreach (var updatedTheme in chapterUpdateModel.Themes ?? [])
                {
                    var theme = _mapper.Map<Theme>(updatedTheme);
                    theme.UserId = (int)userId;
                    await _themeRepository.CreateAsync(theme);
                }
            }
            catch (SqlException)
            { 
                return Problem(statusCode: 500, detail: "Ошибка базы данных");
            }

            return Ok();
        }
    }
}
