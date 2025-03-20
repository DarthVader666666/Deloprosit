using AutoMapper;
using Deloprosit.Bll.Interfaces;
using Deloprosit.Bll.Services;
using Deloprosit.Data.Entities;
using Deloprosit.Server.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Deloprosit.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly EmailSender _emailSender;
        private readonly IRepository<Message> _messageRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public FeedbackController(EmailSender emailSender, IRepository<Message> messageRepository, IMapper mapper, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _messageRepository = messageRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> Send([FromForm] MessageForm? messageForm)
        {
            if (messageForm == null)
            {
                return BadRequest(new { errorText = "Не пришла форма сообщения" });
            }

            var email = _configuration["OwnerEmail"];

            if (email == null)
            {
                return StatusCode(500, new { errorText = "Не найден email для отсылки сообщения" });
            }

            var result = Task.FromResult(false);

            if (!messageForm.Email.IsNullOrEmpty())
            {
                result = _emailSender.SendEmailAsync(email, $"{messageForm.Name} прислал сообщение в Deloprosit",
                    $"<div>{messageForm.Text}</div><div>Email: {messageForm.Email}</div><div>Телефон: {messageForm.Phone}</div>");
            }

            try
            {
                var message = _mapper.Map<Message>(messageForm);
                await _messageRepository.CreateAsync(message);
            }
            catch
            {
                return StatusCode(500, new { errorText = "Ошибка сервера" });
            }

            if (await result)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, new { errorText = "Ошибка отправки письма на email" });
            }
        }
    }
}
