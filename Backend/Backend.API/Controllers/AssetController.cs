using AutoMapper;
using Backend.API;
using Backend.API.Controllers;
using Backend.API.ViewModel;
using Backend.Business.Services;
using Backend.Data;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly ApplicationDbContext _db;
        private IHubContext<MessageHub, IMessageHubClient> _messageHub;

        public AssetController(ILogger<AssetController> logger, IGenericService genericService, IMapper mapper, IConfiguration config, IHubContext<MessageHub, IMessageHubClient> messageHub, ApplicationDbContext db)
        {
            _genericService= genericService;
            _logger = logger;
            _mapper = mapper;
            _config= config;
            _messageHub = messageHub;
            _db = db;
        }

        // Method to get the list of the Assets
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<AssetEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAssetsList()
        {
            var result = await _genericService.GetAssetsList();
            IEnumerable<GetAssetVM> models = _mapper.Map<IEnumerable<GetAssetVM>>(result);

            if (result == null)
            {
                return NotFound();
            }
            return Ok(models);
        }

        [HttpPost]
        [Route("/TotalAsset/{UserEmail}")]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TotalAsset(string UserEmail)
        {
            float totalAmounts = 0;
            int count = 0;

            var assets = await _genericService.GetAssetsList();
            var users = await _genericService.GetUsersList();

            var user = users.Where(x => x.UserEmail == UserEmail).FirstOrDefault();

            float longitude = 0;
            float latitude = 0;
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
                        longitude = asset.Longitude;
                        latitude = asset.Latitude;
                        totalAmounts += asset.Amount;
                        count++;

                        var socketData = new PostTotalAsset { Count = count, TotalAmount = totalAmounts, Longitude = longitude, Latitude = latitude };
                        await _messageHub.Clients.All.SendTotalAsset(socketData);
                    }
                }
            }

            PostTotalAsset data = new PostTotalAsset { Count = count, TotalAmount = totalAmounts, Longitude = longitude, Latitude = latitude };
            return Ok(data);
        }

        #region SetLimitAmount
        [HttpPost]
        [Route("/SetLimitAmount")]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetAlertLimitAmount(AlertVM model)
        {
            var assets = await _genericService.GetAssetsList();
            foreach( var item in assets)
            {
                item.Period = model.Period;
                item.MinAmount = model.MinAmount;
                item.UpdatedDate = DateTime.UtcNow;

                await _genericService.UpdateAssetDetail(item);
            }

            await CheckAlert();
            return Ok();
        }
        #endregion
        #region BuyAsset
        [HttpPost]
        [Route("/BuyAsset")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]

        public async Task<IActionResult> BuyAsset(BuyAsset model)
        {
            IEnumerable<AssetEntity> assets = await _genericService.GetAssetsList();

            if (assets.Where(x => x.TankName == model.TankName).Count() == 0)
                return BadRequest("Not find Tank");

            var asset = assets.Where(x => x.TankName == model.TankName).First();

            if ((asset.Amount + model.Amount) > asset.MaxAmount)
                return BadRequest("Overflow Tank");

            else
            {
                asset.Amount += model.Amount;
                LogEntity log = new LogEntity
                {
                    Amount = model.Amount,
                    TankName = model.TankName,
                    Type = "Buy",
                    CreatedDate = DateTime.UtcNow,
                    UserEmail = asset.UserEmail,
                };

                await _genericService.SaveLogDetail(log);
            }

            await CheckAlert();

            var save = _genericService.UpdateAssetDetail(asset);
            return Ok(save);
        }
        #endregion

        #region SellAsset
        [HttpPost]
        [Route("/SellAsset")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        public async Task<IActionResult> SellAsset(SellAsset model)
        {
            IEnumerable<AssetEntity> assets = await _genericService.GetAssetsList();
            if (assets.Where(x => x.TankName == model.TankName).Count() == 0)
                return BadRequest("Not find Tank");

            AssetEntity asset = assets.Where(x => x.TankName == model.TankName).FirstOrDefault();

            if (asset.Amount < model.Amount)
                return BadRequest("Not enuogh amount to sell");


            if ((asset.Amount - model.Amount) < asset.MinAmount)
                return BadRequest("Rest amount is below the min amount");
            else
            {
                asset.Amount -= model.Amount;

                LogEntity log = new LogEntity
                {
                    Amount = model.Amount,
                    TankName = model.TankName,
                    Type = "Sell",
                    CreatedDate = DateTime.UtcNow,
                    UserEmail = asset.UserEmail
                };

                await _genericService.SaveLogDetail(log);
            }
            await CheckAlert();

            var save = await _genericService.UpdateAssetDetail(asset);
            return Ok(save);
        }
        #endregion
        // Method to Save the Asset detail
        [HttpPost(Name = "SaveAssetDetail")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]

        public async Task<IActionResult> SaveAssetDetail(PostAssetVM model)
        {
            var assets = await _genericService.GetAssetsList();
            var users = await _genericService.GetUsersList();

            if (assets.Where( x => x.TankName == model.TankName).Count() > 0)
                return BadRequest("TankName exists!");

            if (assets.Where(x => x.UserEmail == model.UserEmail).Count() > 0)
                return BadRequest("Manager already have a tank!");

            AssetEntity asset = _mapper.Map<AssetEntity>(model);

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
                asset.Owner = checkUser.First();
            }

            var save = await _genericService.SaveAssetDetail(asset);

            // Save command to the database
            if (asset.Amount == 0)
            {

                CommandEntity command = new CommandEntity();
                command.Command = CommandTypes.Fill;
                command.OwnerId = asset.Owner.Id;
                command.TankName = model.TankName;
                command.Flag = false;

                //var commands = await _genericService.GetCommandsList();
                //int count = commands.Where(x => ((x.Command == command.Command) && (x.OwnerId == command.OwnerId) && (x.TankName == command.TankName) && (x.Flag == false))).Count();
                //if (count == 0)
                //{
                //    await _genericService.SaveCommandDetail(command);
                //}

                await _genericService.SaveCommandDetail(command);

                #region Socket
                var result = await _genericService.GetCommandsList();
                IEnumerable<SocketCommandVM> models = _mapper.Map<IEnumerable<SocketCommandVM>>(result);

                assets = await _genericService.GetAssetsList();

                if (models.Count() > 0)
                {
                    foreach (var item in models)
                    {
                        var tAsset = assets.Where(x => x.TankName == item.TankName).ToList();

                        item.MinAmount = tAsset[0].MinAmount;
                        item.MaxAmount = tAsset[0].MaxAmount;
                        item.UserEmail = tAsset[0].UserEmail;
                    }

                    await _messageHub.Clients.All.SendCommands(models.ToList());
                }
                #endregion
            }

            if (save == null)
            {
                return NotFound();
            }
            return Ok(save);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]

        public async Task<IActionResult> UpdateAsset([FromRoute] string id, [FromBody] PostAssetVM model)
        {
            UserEntity currentUser = GetCurrentUser();
            AssetEntity asset = _mapper.Map<AssetEntity>(model);

            var users = await _genericService.GetUsersList();
            asset.OwnerId = users.Where(x => x.UserEmail == currentUser.UserEmail).FirstOrDefault().Id;
            asset.Owner = currentUser;

            var save = await _genericService.UpdateAssetDetail(asset);
            if (save == null)
            {
                return NotFound();
            }
            await CheckAlert();

            return Ok(save);
        }

        // Method to delete the Asset detail
        [HttpDelete(Name = "DeleteAsset")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]

        public async Task<IActionResult> DeleteAsset(string Id)
        {
            await _genericService.DeleteAsset(Convert.ToInt32(Id));
            return Ok();
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

        private async Task CheckAlert()
        {
            var commands = await _genericService.GetCommandsList();
            var assets = await _genericService.GetAssetsList();
            float _alertLimit = assets.First().MinAmount;

            foreach (var item in assets)
            {
                if (item.Amount < _alertLimit)
                {
                    CommandEntity command = new CommandEntity();
                    command.Command = CommandTypes.Fill;
                    command.OwnerId = item.Owner.Id;
                    command.TankName = item.TankName;
                    command.Flag = false;

                    //int count = commands.Where(x => ((x.Command == command.Command) && (x.OwnerId == command.OwnerId) && (x.TankName == command.TankName) && (x.Flag == false))).Count();
                    //if (count == 0)
                    //{
                    //    await _genericService.SaveCommandDetail(command);
                    //}

                    await _genericService.SaveCommandDetail(command);

                    #region Socket
                    var result = await _genericService.GetCommandsList();
                    IEnumerable<SocketCommandVM> models = _mapper.Map<IEnumerable<SocketCommandVM>>(result);

                    foreach (var modelItem in models)
                    {
                        modelItem.UserEmail = assets.Where(x => x.TankName == modelItem.TankName).FirstOrDefault().UserEmail;
                        modelItem.MinAmount = assets.Where(x => x.TankName == modelItem.TankName).FirstOrDefault().MinAmount;
                        modelItem.MaxAmount = assets.Where(x => x.TankName == modelItem.TankName).FirstOrDefault().MaxAmount;
                    }
                    await _messageHub.Clients.All.SendCommands(models.ToList());
                    #endregion
                }
            }
        }
    }
}