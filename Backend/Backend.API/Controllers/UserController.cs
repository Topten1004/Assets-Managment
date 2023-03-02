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
using System.Linq;

namespace Backend.API.Controllers
{
    public partial class AccessToken
    {
        public string? token { get; set; }
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

        #region LoginIn
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
                var users = await _genericService.GetUsersList();

                users = users.Where(x => x.UserEmail == model.UserEmail);

                if (users.Count() == 0)
                {
                    return BadRequest("UserEmail not exists!");
                }

                var salt = System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
                var password = System.Text.Encoding.UTF8.GetBytes(model.Password);

                var hmacSHA1 = new HMACSHA1(salt);
                var saltedHash = hmacSHA1.ComputeHash(password);

                var comparePassword = Convert.ToBase64String(saltedHash);

                var result = users.Where(x => (x.UserEmail == model.UserEmail && x.Password == comparePassword));

                if (result.Count() == 0)
                {
                    return BadRequest("Password doesn't match!");
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
        #endregion


        #region RegisterManager
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
        #endregion


        #region Change Password
        // Method to Change Password
        [HttpPost]
        [Route("/ChangePassword")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            try
            {
                var users = await _genericService.GetUsersList();

                if (users.Where(x => x.UserEmail == model.UserEmail).Count() == 0)
                {
                    return BadRequest("UserEmail not exists!");
                }

                var user = users.Where(x => x.UserEmail == model.UserEmail).FirstOrDefault();

                var salt = System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
                var password = System.Text.Encoding.UTF8.GetBytes(model.OldPassword);

                var hmacSHA1 = new HMACSHA1(salt);
                var saltedHash = hmacSHA1.ComputeHash(password);

                var comparePassword = Convert.ToBase64String(saltedHash);

                if (users.Where(x => x.Password == comparePassword).Count() == 0)
                {
                    return BadRequest("Old Password doesn't match!");
                }

                #region setNewPassowrd
                salt = System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
                password = System.Text.Encoding.UTF8.GetBytes(model.NewPassword);

                hmacSHA1 = new HMACSHA1(salt);
                saltedHash = hmacSHA1.ComputeHash(password);

                comparePassword = Convert.ToBase64String(saltedHash);

                user.Password = comparePassword;

                #endregion
                var save = await _genericService.UpdateUserDetail(user);

                if (save == null)
                {
                    return NotFound();
                }

                string token = GenerateToken(save);

                return Ok(token);
            }
            catch (Exception ex)
            {
                var msg = $"Method: ChangePassword, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/UserController/ChangePassword", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region GenerateToken
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
        #endregion
    }
}
