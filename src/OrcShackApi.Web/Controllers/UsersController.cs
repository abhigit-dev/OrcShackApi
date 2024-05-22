using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrcShackApi.Core.Models;
using OrcShackApi.Web.Jwt;
using OrcShackApi.Web.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace OrcShackApi.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IValidator<UserDto> _userDtoValidator;
        private readonly IValidator<PasswordUpdateDto> _passwordUpdateDtoValidator;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ITokenService tokenService, IMapper mapper, IValidator<UserDto> userDtoValidator, IValidator<PasswordUpdateDto> passwordUpdateDtoValidator, ILogger<UsersController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userDtoValidator = userDtoValidator ?? throw new ArgumentNullException(nameof(userDtoValidator));
            _passwordUpdateDtoValidator = passwordUpdateDtoValidator ?? throw new ArgumentNullException(nameof(passwordUpdateDtoValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [AllowAnonymous]
        [HttpPost ("Register", Order = 1)]
        [SwaggerOperation("Register User", "1")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            _logger.LogInformation("Registering a new user");

            var validationResult = await _userDtoValidator.ValidateAsync(userDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed while registering a new user");
                return BadRequest(validationResult.Errors);
            }

            var user = _mapper.Map<User>(userDto);
            await _userService.Create(user, userDto.Password);

            _logger.LogInformation("New user registered successfully");
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("authenticate" , Order = 2)]
        [SwaggerOperation("Authenticate User and Get Token", "2")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateDto authenticateDto)
        {
            _logger.LogInformation("Authenticating user");

            var user = await _userService.Authenticate(authenticateDto.Email, authenticateDto.Password);
            if (user == null)
            {
                _logger.LogWarning("Failed to authenticate user");
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            var token = _tokenService.GenerateToken(user);
            _logger.LogInformation("User authenticated successfully");

            return Ok(new { Token = token });
        }

        [AllowAnonymous]
        [HttpPut("ResetPassword", Order = 3)]
        [SwaggerOperation("Reset Password", "3")]
        public async Task<IActionResult> UpdatePassword(string email, [FromBody] PasswordUpdateDto passwordUpdateDto)
        {
            _logger.LogInformation($"Resetting password for user with email {email}");

            var validationResult = await _passwordUpdateDtoValidator.ValidateAsync(passwordUpdateDto);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed while resetting password");
                return BadRequest(validationResult.Errors);
            }

            var result = await _userService.UpdatePassword(email, passwordUpdateDto.OldPassword, passwordUpdateDto.NewPassword);
            if (!result)
            {
                _logger.LogWarning("Failed to reset password");
                return BadRequest(new { message = "Old password is incorrect" });
            }

            _logger.LogInformation("Password reset successfully");
            return NoContent();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Getting all users");

            var users = await _userService.GetAll();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation($"Getting user with ID {id}");

            var user = await _userService.GetById(id);
            if (user == null)
            {
                _logger.LogWarning($"User with ID {id} not found");
                return NotFound();
            }

            return Ok(user);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User userParam)
        {
            _logger.LogInformation($"Updating user with ID {id}");

            if (id != userParam.Id)
                return BadRequest();

            try
            {
                await _userService.Update(userParam);
                _logger.LogInformation($"User with ID {id} updated successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating user with ID {id}: {ex.Message}");
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Deleting user with ID {id}");

            try
            {
                await _userService.Delete(id);
                _logger.LogInformation($"User with ID {id} deleted successfully");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while deleting user with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
