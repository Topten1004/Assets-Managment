using Backend.Business.Services;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
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
        public async Task<IResult> UpdateBook([FromRoute] string id, [FromBody] AssetEntity model)
        {
            var save = await _genericService.UpdateAssetDetail(model);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(save);
        }

        // Method to delete the Asset detail
        [HttpDelete(Name = "DeleteAsset")]
        public async Task<IResult> DeleteAsset(string Id)
        {
            await _genericService.DeleteAsset(Convert.ToInt32(Id));
            return Results.Ok();
        }
    }
}