using AutoMapper;

using Delopro.Bll.Interfaces;
using Delopro.Data.Entities;

using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        private readonly IRepository<Captcha> _captchaRepository;
        private readonly IMapper _mapper;

        public CaptchaController(IRepository<Captcha> captchaRepository, IMapper mapper)
        {
            _captchaRepository = captchaRepository;
            _mapper = mapper;
        }

        [Route("[action]")]
        public async Task<IActionResult> Get()
        {
            var captcha = await _captchaRepository.GetAsync(null);

            if (captcha != null)
            {
                return Ok(captcha);
            }
            else
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }
        }
    }
}
