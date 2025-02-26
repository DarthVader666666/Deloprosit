using Deloprosit.Bll.Interfaces;
using Deloprosit.Data.Entities;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

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
    }
}
