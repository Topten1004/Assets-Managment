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

namespace Backend.API.Controllers
{
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
        public async Task<IResult> LoginIn(LoginVM model)
        {
            var results = await _genericService.GetUsersList();
            
            var salt = System.Text.Encoding.UTF8.GetBytes("passwodencryption");
            var password = System.Text.Encoding.UTF8.GetBytes(model.Password);

            var result = results.Where(x => (x.UserEmail == model.UserEmail && x.Password == Convert.ToBase64String(password)));

            if (result == null)
            {
                return Results.NotFound();
            }

            string token = GenerateToken(result.FirstOrDefault());

            return Results.Ok(token);
        }

        // Method to Sign Up
        [HttpPost]
        [Route("/SignUp")]
        public async Task<IResult> SignUpUser(SignUpVM model)
        {
            UserEntity newUser = new UserEntity();
            newUser.UserEmail = model.UserEmail;

            var salt = System.Text.Encoding.UTF8.GetBytes("passwodencryption");
            var password = System.Text.Encoding.UTF8.GetBytes(model.Password);

            var hmacSHA1 = new HMACSHA1(salt);
            var saltedHash = hmacSHA1.ComputeHash(password);

            newUser.UserEmail = model.UserEmail;
            newUser.Password = Convert.ToBase64String(saltedHash);
            newUser.Role = model.Role;

            var save = await _genericService.SaveUserDetail(newUser);
            if (save == null)
            {
                return Results.NotFound();
            }

            string token = GenerateToken(save);

            return Results.Ok(token);
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
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
