using AutoMapper;

using Delopro.Bll.Interfaces;
using Delopro.Data.Entities;
using Delopro.Data.Enums;
using Delopro.Server.Enums;
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
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IMapper _mapper;

        public AdministrationController(IRepository<User> userRepository, IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
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

            if (user == null)
            {
                return StatusCode(500, new { errorText = "Ошибка базы данных" });
            }

            var userLongResponseModel = _mapper.Map<UserLongResponseModel>(user);
            var roleTypes = (await _roleRepository.GetListAsync(user.UserId))
                .Select(x => Enum.TryParse(typeof(UserRoleType), x?.RoleName, out object? role)
                ? (int)role - 1 : -1).ToArray();

            userLongResponseModel.Roles = roleTypes;

            return Ok(userLongResponseModel);
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateModel userUpdateModel)
        {
            var userRolesToDelete = await _userRoleRepository.GetListAsync(userUpdateModel?.UserId);
            var userRolesToCreate = userUpdateModel?.Roles?.Select(x => (UserRole?)new UserRole { UserId = userUpdateModel.UserId, RoleId = x + 1 }) ?? [];

            await _userRoleRepository.DeleteRangeAsync(userRolesToDelete);

            foreach (var userRole in userRolesToCreate ?? [])
            {
                await _userRoleRepository.CreateAsync(userRole);
            }

            var user = await _userRepository.GetAsync(userUpdateModel?.UserId);

            if (user == null)
            {
                return StatusCode(500, new { errorText = "Ошибка базы данных" });
            }

            user.DeletionDate = userUpdateModel?.DeletionDate;
            user.IsConfirmed = userUpdateModel?.Status == (int)UserStatus.Confirmed;
            user.IsDeleted = userUpdateModel?.Status == (int)UserStatus.Deleted;

            await _userRepository.UpdateAsync(user);

            return Ok(new { okText = "Пользователь успешно обновлен" });
        }
    }
}
