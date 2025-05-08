using AutoMapper;

using Delopro.Bll.Interfaces;
using Delopro.Data.Entities;
using Delopro.Server.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Delopro.Server.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("AllowClient")]
    [ApiController]

    [Authorize(Roles = "Owner, Admin")]
    public class AdministrationController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMapper _mapper;

        public AdministrationController(IRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUsers()
        { 
            var users = await _userRepository.GetListAsync();
            var userShortResponseModels = _mapper.Map<IEnumerable<UserShortResponseModel>>(users);

            return Ok(userShortResponseModels);
        }

        [HttpGet]
        [Route("[action]/{userId:int}")]
        public async Task<IActionResult> GetUser([FromRoute] int userId)
        {
            var user = await _userRepository.GetAsync(userId);
            var userLongResponseModel = _mapper.Map<UserLongResponseModel>(user);

            return Ok(userLongResponseModel);
        }
    }
}
