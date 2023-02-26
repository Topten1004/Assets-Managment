using AutoMapper;
using Backend.API.ViewModel;
using Backend.Business.Services;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        public AssetController(ILogger<AssetController> logger, IGenericService genericService, IMapper mapper)
        {
            _genericService= genericService;
            _logger = logger;
            _mapper = mapper;
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

        // Method to Save the Asset detail
        [HttpPost(Name = "SaveAssetDetail")]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

        public async Task<IResult> SaveAssetDetail(PostAssetVM model)
        {
            UserEntity currentUser = GetCurrentUser();
            AssetEntity asset = _mapper.Map<AssetEntity>(model);

            var users = await _genericService.GetUsersList();
            asset.OwnerId = users.Where(x => x.UserEmail == currentUser.UserEmail).FirstOrDefault().Id;
            asset.Owner = await _genericService.GetUserDetailById(asset.OwnerId);

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