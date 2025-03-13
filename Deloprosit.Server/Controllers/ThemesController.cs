﻿using AutoMapper;
using Deloprosit.Bll.Interfaces;
using Deloprosit.Bll.Services;
using Deloprosit.Data.Entities;
using Deloprosit.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("api/[controller]")]
    [ApiController]
    public class ThemesController : ControllerBase
    {
        private readonly IRepository<Theme> _themeRepository;
        private readonly UserManager _userManager;
        private readonly IMapper _mapper;

        public ThemesController(IRepository<Theme> themesRepository, UserManager userManager, IMapper mapper)
        {
            _themeRepository = themesRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("[action]/{themeId:int?}")]
        public async Task<IActionResult> Get(int? themeId)
        {
            var theme = await _themeRepository.GetAsync(themeId);

            return Ok(theme);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetList([FromQuery] int? chapterId = null)
        {
            var themes = await _themeRepository.GetListAsync(chapterId);

            return Ok(themes);
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> Create(ThemeCreateModel themeCreateModel)
        {
            try
            {
                var theme = _mapper.Map<Theme>(themeCreateModel);
                var userId = (await _userManager.GetCurrentUserAsync(HttpContext))?.UserId;
                theme.UserId = userId;

                await _themeRepository.CreateAsync(theme);
            }
            catch (SqlException)
            {
                return Problem(statusCode: 500);
            }

            return Ok();
        }

        [HttpDelete]
        [Route("[action]/{themeId:int}")]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> Delete([FromRoute] int? themeId) 
        {
            if (themeId == null)
            {
                return BadRequest();
            }

            try
            {
                await _themeRepository.DeleteAsync(themeId);
            }
            catch (SqlException)
            {
                return Problem(statusCode: 500);
            }

            return Ok();
        }
    }
}
