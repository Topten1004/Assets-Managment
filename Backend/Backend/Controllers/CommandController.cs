using Backend.Business.Services;
using Backend.Controllers;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CommandController : ControllerBase
    {
        private readonly ILogger<CommandController> _logger;
        private readonly IGenericService _genericService;

        public CommandController(ILogger<CommandController> logger, IGenericService genericService)
        {
            _genericService = genericService;
            _logger = logger;
        }

        // Method to get the list of the Commands
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

        public async Task<IResult> GetCommandsList()
        {
            var result = await _genericService.GetCommandsList();
            if (result == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(result);
        }

        // Method to Save the Command detail
        [HttpPost(Name = "SaveCommandDetail")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IResult> SaveAssetDetail(CommandEntity model)
        {
            var save = await _genericService.SaveCommandDetail(model);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(save);
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IResult> UpdateBook([FromRoute] string id, [FromBody] CommandEntity model)
        {
            model.Id = Convert.ToInt32(id);
            var save = await _genericService.UpdateCommandDetail(model);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(save);
        }

        // Method to delete the Asset detail
        [HttpDelete(Name = "DeleteCommand")]
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
