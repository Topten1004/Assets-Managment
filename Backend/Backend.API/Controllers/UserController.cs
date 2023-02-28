using Backend.API.ViewModel;
using Backend.Business.Services;
using Backend.Controllers;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.InteropServices;

namespace Backend.API.Controllers
{
    public class AccessToken
    {
        public string token { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IGenericService _genericService;
        private readonly IConfiguration _config;

        public UserController(ILogger<UserController> logger, IGenericService genericService, IConfiguration config)
        {
            _genericService = genericService;
            _logger = logger;
            _config = config;
        }

        // Method to Login
        [HttpPost]
        [Route("/Login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginIn(LoginVM model)
        {
            try
            {
                var results = await _genericService.GetUsersList();

                var salt = System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
                var password = System.Text.Encoding.UTF8.GetBytes(model.Password);

                var hmacSHA1 = new HMACSHA1(salt);
                var saltedHash = hmacSHA1.ComputeHash(password);

                var comparePassword = Convert.ToBase64String(saltedHash);

                var result = results.Where(x => (x.UserEmail == model.UserEmail && x.Password == comparePassword));

                if (result.Count() == 0)
                {
                    return NotFound();
                }

                string token = GenerateToken(result.First());

                return Ok(token);
            }
            catch(Exception ex)
            {
                var msg = $"Method: LoginIn, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/UserController/LoginIn", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // Method to Register Manager
        [HttpPost]
        [Route("/Register")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterManager(SignUpVM model)
        {
            try
            {
                var users = await _genericService.GetUsersList();
                users = users.Where(x => x.UserEmail == model.UserEmail);

                if (users.Count() > 0)
                {
                    return BadRequest("UserEmail exists!");
                }

                UserEntity newUser = new UserEntity();
                newUser.UserEmail = model.UserEmail;

                var salt = System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
                var password = System.Text.Encoding.UTF8.GetBytes(model.Password);

                var hmacSHA1 = new HMACSHA1(salt);
                var saltedHash = hmacSHA1.ComputeHash(password);

                newUser.UserEmail = model.UserEmail;
                newUser.Password = Convert.ToBase64String(saltedHash);
                newUser.Role = 0;

                var save = await _genericService.SaveUserDetail(newUser);
                if (save == null)
                {
                    return NotFound();
                }

                string token = GenerateToken(save);

                return Ok(token);
            } catch(Exception ex)
            {
                var msg = $"Method: RegisterManager, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/UserController/RegisterManager", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        // To generate token
        private string GenerateToken(UserEntity user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(10005),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
