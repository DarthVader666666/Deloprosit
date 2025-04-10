using AutoMapper;
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
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly EmailSender _emailSender;
        private readonly UserManager _userManager;
        private readonly CryptoService _cryptoService;
        private readonly IRepository<Message> _messageRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public FeedbackController(EmailSender emailSender, UserManager userManager, CryptoService cryptoService,
            IRepository<Message> messageRepository, IMapper mapper, IConfiguration configuration)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _cryptoService = cryptoService;
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

            await _emailSender.SendEmailAsync(email, $"{messageForm.Name} прислал сообщение в Deloprosit",
                $"<div>{messageForm.Text}</div><div>Email: {messageForm.Email}</div><div>Телефон: {messageForm.Phone}</div>");

            Message? createdMessage = null;

            try
            {
                var message = _mapper.Map<Message>(messageForm);

                var user = await _userManager.GetUserByAsync(nickname: _configuration["OwnerNickname"]) ?? throw new NullReferenceException();
                message.UserId = user.UserId;

                message.Name = _cryptoService.Encrypt(message.Name);
                message.Email = _cryptoService.Encrypt(message.Email);
                message.Phone = _cryptoService.Encrypt(message.Phone);
                message.Text = _cryptoService.Encrypt(message.Text);

                createdMessage = await _messageRepository.CreateAsync(message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorText = ex.Message });
            }

            if (createdMessage != null)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, new { errorText = "Ошибка отправки сообщения" });
            }
        }

        [HttpGet]
        [Route("[action]/{isRead:bool}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetList([FromRoute] bool isRead)
        {
            var user = await _userManager.GetCurrentUserAsync(HttpContext);
            var messages = (await _messageRepository.GetListAsync(user?.UserId)).Where(message => message?.IsRead == isRead)
                .Select(message =>
                {
                    if (message == null)
                    {
                        return null;
                    }

                    message.Name = _cryptoService.Decrypt(message.Name);
                    message.Email = _cryptoService.Decrypt(message.Email);
                    message.Phone = _cryptoService.Decrypt(message.Phone);
                    message.Text = _cryptoService.Decrypt(message.Text);

                    return message;
                });

            try
            {
                var messageResponseModels = _mapper.Map<IEnumerable<MessageResponseModel>>(messages);
                return Ok(messageResponseModels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorText = ex.Message });
            }
        }

        [HttpGet]
        [Route("[action]/{messageId:int}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Get([FromRoute] int messageId)
        {
            var message = await _messageRepository.GetAsync(messageId);

            if (message == null)
            {
                return NotFound(new { errorText = "Сообщение не найдено" });
            }

            message.Name = _cryptoService.Decrypt(message.Name);
            message.Email = _cryptoService.Decrypt(message.Email);
            message.Phone = _cryptoService.Decrypt(message.Phone);
            message.Text = _cryptoService.Decrypt(message.Text);

            try
            {
                var messageResponseModel = _mapper.Map<MessageResponseModel>(message);
                return Ok(messageResponseModel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorText = ex.Message });
            }
        }

        [HttpPut]
        [Route("[action]/{messageId:int}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Update([FromRoute] int messageId)
        {
            var message = await _messageRepository.GetAsync(messageId);

            if (message == null)
            {
                return NotFound(new { errorText = "Сообщение не найдено" });
            }

            try
            {
                message.IsRead = true;
                var messageResult = await _messageRepository.UpdateAsync(message);

                if (messageResult != null)
                {
                    return Ok();
                }
                else 
                {
                    return StatusCode(500, new { errorText = "Ошибка сервера" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorText = ex.Message });
            }
        }

        [HttpGet]
        [Route("[action]")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetUnreadMessagesCount()
        {
            try
            {
                var user = await _userManager.GetCurrentUserAsync(HttpContext);
                var count = (await _messageRepository.GetListAsync(user?.UserId)).Count(message => !(message?.IsRead ?? true));

                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errorText = ex.Message });
            }
        }
    }
}
