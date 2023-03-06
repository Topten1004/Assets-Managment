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


        #region GetAssetsList
        // Method to get the list of the Assets
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<AssetEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAssetsList()
        {
            try
            {
                var result = await _genericService.GetAssetsList();
                IEnumerable<GetAssetVM> models = _mapper.Map<IEnumerable<GetAssetVM>>(result);

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(models);
            }
            catch(Exception ex)
            {
                var msg = $"Method: GetAssetsList, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/Asset/GetAssetsList", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region IsRepair
        [HttpPost]
        [Route("/IsRepair")]
        [ProducesResponseType(typeof(Boolean), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> IsRepair(RepairAssetVM model)
        {
            try
            {
                var assets = await _genericService.GetAssetsList();
                if (assets.Where(x => x.UserEmail == model.UserEmail).Count() == 0)
                    return BadRequest("Can't find user email");

                var asset = assets.Where(x => x.UserEmail == model.UserEmail).First();

                var checkDate = asset.UpdatedDate;
                if (DateTime.UtcNow > checkDate.AddDays(model.Period))
                    return Ok(true);

                else
                    return Ok(false);

            }
            catch (Exception ex)
            {
                var msg = $"Method: IsRepair, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/AssetController/IsRepair", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region TotalAsset
        [HttpGet]
        [Route("/TotalAsset/{UserEmail}")]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TotalAsset(string UserEmail)
        {
            try
            {
                float totalAmounts = 0;
                int count = 0;

                var assets = await _genericService.GetAssetsList();
                var users = await _genericService.GetUsersList();

                var user = users.Where(x => x.UserEmail == UserEmail).FirstOrDefault();

                List<GetAssetVM> totalAssets = _mapper.Map<List<GetAssetVM>>(assets);

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

                            var socketData = new PostTotalAsset { Count = count, TotalAmount = totalAmounts, Assets = totalAssets };
                            await _messageHub.Clients.All.SendTotalAsset(socketData);
                        }
                    }
                }

                PostTotalAsset data = new PostTotalAsset { Count = count, TotalAmount = totalAmounts, Assets = totalAssets };
                return Ok(data);
            }
            catch(Exception ex)
            {
                var msg = $"Method: TotalAsset, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/AssetController/TotalAsset", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
#endregion


        #region SetLimitAmount
        [HttpPost]
        [Route("/SetLimitAmount")]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetAlertLimitAmount(AlertVM model)
        {
            try
            {
                var assets = await _genericService.GetAssetsList();
                foreach (var item in assets)
                {
                    item.Period = model.Period;
                    item.MinAmount = model.MinAmount;

                    await _genericService.UpdateAssetDetail(item);
                }

                return Ok();
            }
            catch(Exception ex)
            {
                var msg = $"Method: SetAlertLimitAmount, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/AssetController/SetAlertLimitAmount", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
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
            try
            {
                IEnumerable<AssetEntity> assets = await _genericService.GetAssetsList();

                if (assets.Where(x => x.UserEmail == model.UserEmail).Count() == 0)
                    return BadRequest("Not find manager");

                var asset = assets.Where(x => x.UserEmail == model.UserEmail).First();

                if ((asset.Amount + model.Amount) > asset.MaxAmount)
                    return BadRequest("Overflow Tank");

                else
                {
                    asset.Amount += model.Amount;
                    LogEntity log = new LogEntity
                    {
                        From = model.From,
                        Amount = model.Amount,
                        TankName = asset.TankName,
                        Type = "Buy",
                        CreatedDate = DateTime.UtcNow,
                        UserEmail = model.UserEmail,
                    };

                    await _genericService.SaveLogDetail(log);
                }

                var save = _genericService.UpdateAssetDetail(asset);
                return Ok(save);
            } catch(Exception ex)
            {
                var msg = $"Method: BuyAsset, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/AssetController/BuyAsset", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region RepairAsset
        [HttpGet]
        [Route("/RepairAsset/{UserEmail}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]

        public async Task<IActionResult> RepairAsset(string UserEmail)
        {
            try
            {
                IEnumerable<AssetEntity> assets = await _genericService.GetAssetsList();

                if (assets.Where(x => x.UserEmail == UserEmail).Count() == 0)
                    return BadRequest("Not find manager");

                var asset = assets.Where(x => x.UserEmail == UserEmail).First();

                LogEntity log = new LogEntity
                {
                    From = string.Empty,
                    Amount = 0,
                    TankName = asset.TankName,
                    Type = "Repair",
                    CreatedDate = DateTime.UtcNow,
                    UserEmail = asset.UserEmail,
                };

                await _genericService.SaveLogDetail(log);

                asset.UpdatedDate = DateTime.UtcNow;

                var save = _genericService.UpdateAssetDetail(asset);

                return Ok(save);
            }
            catch (Exception ex)
            {
                var msg = $"Method: RepairAsset, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/AssetController/RepairAsset", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
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
            try
            {
                IEnumerable<AssetEntity> assets = await _genericService.GetAssetsList();
                if (assets.Where(x => x.UserEmail == model.UserEmail).Count() == 0)
                    return BadRequest("Not find Tank");

                AssetEntity asset = assets.Where(x => x.UserEmail == model.UserEmail).FirstOrDefault();

                if (asset.Amount < model.Amount)
                    return BadRequest("Not enuogh amount to sell");
                else
                {
                    asset.Amount -= model.Amount;

                    LogEntity log = new LogEntity
                    {
                        From = model.From,
                        Amount = model.Amount,
                        TankName = asset.TankName,
                        Type = "Sell",
                        CreatedDate = DateTime.UtcNow,
                        UserEmail = model.UserEmail
                    };

                    await _genericService.SaveLogDetail(log);
                }

                var save = await _genericService.UpdateAssetDetail(asset);
                return Ok(save);
            } catch(Exception ex)
            {
                var msg = $"Method: SellAsset, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/AssetController/SellAsset", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }

        }
        #endregion


        #region SaveAssetDetail
        // Method to Save the Asset detail
        [HttpPost(Name = "SaveAssetDetail")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]

        public async Task<IActionResult> SaveAssetDetail(PostAssetVM model)
        {
            try
            {
                var assets = await _genericService.GetAssetsList();
                var users = await _genericService.GetUsersList();

                if (assets.Where(x => x.UserEmail == model.UserEmail).Count() > 0)
                    return BadRequest("Manager already have a tank!");

                AssetEntity asset = _mapper.Map<AssetEntity>(model);

                var checkUser = users.Where(x => x.UserEmail == model.UserEmail);
                asset.Owner = new UserEntity();
                asset.Amount = 0;

                if(assets.Count() == 0)
                {
                    asset.MinAmount = 0;
                    asset.Period = 0;
                }
                else
                {
                    asset.MinAmount = assets.First().MinAmount;
                    asset.Period = assets.First().Period;
                }

                asset.UpdatedDate = DateTime.UtcNow;

                if (checkUser.Count() == 0)
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
            catch (Exception ex)
            {
                var msg = $"Method: SaveAssetDetail, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/AssetController/SaveAssetDetail", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region UpdateAsset
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]

        public async Task<IActionResult> UpdateAsset([FromRoute] string id, [FromBody] PostAssetVM model)
        {
            try
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

                return Ok(save);
            }
            catch(Exception ex)
            {
                var msg = $"Method: UpdateAsset, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/AssetController/UpdateAsset", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region DeleteAsset
        // Method to delete the Asset detail
        [HttpDelete(Name = "DeleteAsset")]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]

        public async Task<IActionResult> DeleteAsset(string Id)
        {
            try
            {
                await _genericService.DeleteAsset(Convert.ToInt32(Id));
                return Ok();
            }
            catch (Exception ex)
            {
                var msg = $"Method: DeleteAsset, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/AssetController/DeleteAsset", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region GetCurrentUser
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
        #endregion
    }
}