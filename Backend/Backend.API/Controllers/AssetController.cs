using Backend.Business.Services;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AssetController : ControllerBase
    {
        private readonly ILogger<AssetController> _logger;
        private readonly IGenericService _genericService;
        
        public AssetController(ILogger<AssetController> logger, IGenericService genericService)
        {
            _genericService= genericService;
            _logger = logger;
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
            if (result == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(result);
        }

        // Method to Save the Asset detail
        [HttpPost(Name = "SaveAssetDetail")]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

        public async Task<IResult> SaveAssetDetail(AssetEntity model)
        {
            var save = await _genericService.SaveAssetDetail(model);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(save);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(AssetEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

        public async Task<IResult> UpdateBook([FromRoute] string id, [FromBody] AssetEntity model)
        {
            model.Id = Convert.ToInt32(id);
            var save = await _genericService.UpdateAssetDetail(model);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(save);
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
    }
}