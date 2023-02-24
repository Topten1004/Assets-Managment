using Backend.API.ViewModel;
using Backend.Business.Services;
using Backend.Controllers;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IGenericService _genericService;

        public UserController(ILogger<UserController> logger, IGenericService genericService)
        {
            _genericService = genericService;
            _logger = logger;
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

            return Results.Ok(result);
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

            return Results.Ok(save);
        }
    }
}
