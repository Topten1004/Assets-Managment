using AutoMapper;
using Backend.API;
using Backend.API.Controllers;
using Backend.API.ViewModel;
using Backend.Business.Services;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly ILogger<AssetController> _logger;
        private readonly IGenericService _genericService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AssetController(ILogger<AssetController> logger, IGenericService genericService, IMapper mapper, IConfiguration config)
        {
            _genericService= genericService;
            _logger = logger;
            _mapper = mapper;
            _config= config;
        }

        // Method to get the list of the Assets
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<AssetEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IResult> GetAssetsList()
        {
            var result = await _genericService.GetAssetsList();
            IEnumerable<GetAssetVM> models = _mapper.Map<IEnumerable<GetAssetVM>>(result);

            if (result == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(models);
        }

        [HttpPost]
        [Route("/TotalAsset/{UserEmail}")]
        public async Task<IResult> TotalAsset(string UserEmail)
        {
            float totalAmounts = 0;
            int count = 0;

            var assets = await _genericService.GetAssetsList();
            var users = await _genericService.GetUsersList();

            var user = users.Where(x => x.UserEmail == UserEmail).FirstOrDefault();

            foreach (var asset in assets)
            {
                if (user.Role == Role.Admin)
                {
                    totalAmounts += asset.Amount;
                    count = assets.Count();
                }
                else
                {
                    if (asset.UserEmail == user.UserEmail)
                    {
                        totalAmounts += asset.Amount;
                        count++;
                    }
                }
            }

            PostTotalAsset data = new PostTotalAsset { Count = count, TotalAmount = totalAmounts };
            return Results.Ok(data);
        }

        // Method to Save the Asset detail
        [HttpPost(Name = "SaveAssetDetail")]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

        public async Task<IResult> SaveAssetDetail(PostAssetVM model)
        {
            AssetEntity asset = _mapper.Map<AssetEntity>(model);

            var users = await _genericService.GetUsersList();
            var checkUser = users.Where( x =>  x.UserEmail == model.UserEmail );
            asset.Owner = new UserEntity();
            asset.Amount = 0;

            if(checkUser.Count() == 0)
            {
                asset.Owner.UserEmail = model.UserEmail;

                var salt = System.Text.Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
                var password = System.Text.Encoding.UTF8.GetBytes(model.Password);

                var hmacSHA1 = new HMACSHA1(salt);
                var saltedHash = hmacSHA1.ComputeHash(password);

                asset.Owner.UserEmail = model.UserEmail;
                asset.Owner.Password = Convert.ToBase64String(saltedHash);
                asset.Owner.Role = Role.Manager;

                var result = await _genericService.SaveUserDetail(asset.Owner);
                asset.OwnerId = result.Id;
            }
            else
            {
                asset.Owner = checkUser.FirstOrDefault();
            }
            // Save command to the database
            if (asset.Amount == 0)
            {
                CommandEntity command = new CommandEntity();
                command.Command = CommandTypes.Fill;
                command.OwnerId = asset.Owner.Id;
                command.TankName = model.TankName;

                await _genericService.SaveCommandDetail(command);
            }

            var save = await _genericService.SaveAssetDetail(asset);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

        public async Task<IResult> UpdateAsset([FromRoute] string id, [FromBody] PostAssetVM model)
        {
            UserEntity currentUser = GetCurrentUser();
            AssetEntity asset = _mapper.Map<AssetEntity>(model);

            var users = await _genericService.GetUsersList();
            asset.OwnerId = users.Where(x => x.UserEmail == currentUser.UserEmail).FirstOrDefault().Id;
            asset.Owner = currentUser;

            var save = await _genericService.UpdateAssetDetail(asset);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        }

        // Method to delete the Asset detail
        [HttpDelete(Name = "DeleteAsset")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IResult> DeleteAsset(string Id)
        {
            await _genericService.DeleteAsset(Convert.ToInt32(Id));
            return Results.Ok();
        }

        private UserEntity GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                Role role;
                Enum.TryParse<Role>(userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value, out role);

                return new UserEntity
                {
                    UserEmail = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value,
                    Role = role
                };
            }

            return null;
        }
    }
}