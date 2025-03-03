using Deloprosit.Bll.Interfaces;
using Deloprosit.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Deloprosit.Server.Controllers
{
    [EnableCors("AllowClient")]
    [Route("[controller]")]
    [ApiController]
    public class ThemesController : ControllerBase
    {
        private readonly IRepository<Theme> _themeRepository;

        public ThemesController(IRepository<Theme> themesRepository)
        {
            _themeRepository = themesRepository;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetList([FromQuery] int? chapterId = null)
        {
            var themes = await _themeRepository.GetListAsync(chapterId);

            return Ok(themes);
        }

        [HttpDelete]
        [Route("[action]")]
        [Authorize(Roles = "Admin, Owner")]
        public async Task<IActionResult> DeleteList([FromQuery] int[] themeIds) 
        {
            if (themeIds == null || !themeIds.Any())
            {
                return BadRequest(new { errorText = "Нет выбранных тем" });
            }

            try
            {
                foreach (var themeId in themeIds ?? [])
                {
                    await _themeRepository.DeleteAsync(themeId);
                }
            }
            catch (SqlException)
            {
                return Problem(statusCode: 500);
            }            

            return Ok();
        }
    }
}
