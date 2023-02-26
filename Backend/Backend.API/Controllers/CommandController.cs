using AutoMapper;
using Backend.API.ViewModel;
using Backend.Business.Services;
using Backend.Controllers;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.API.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CommandController : ControllerBase
    {
        private readonly ILogger<CommandController> _logger;
        private readonly IGenericService _genericService;
        private readonly IMapper _mapper;

        public CommandController(ILogger<CommandController> logger, IGenericService genericService, IMapper mapper)
        {
            _genericService = genericService;
            _logger = logger;
            _mapper = mapper;
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
            IEnumerable<CommandVM> models = _mapper.Map<IEnumerable<CommandVM>>(result);

            if (result == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(models);
        }

        // Method to Save the Command detail
        [HttpPost(Name = "SaveCommandDetail")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IResult> SaveCommandDetail(CommandVM model)
        {
            UserEntity currentUser = GetCurrentUser();
            CommandEntity command = _mapper.Map<CommandEntity>(model);

            var users = await _genericService.GetUsersList();
            command.OwnerId = users.Where(x => x.UserEmail == currentUser.UserEmail).FirstOrDefault().Id;
            command.Owner = await _genericService.GetUserDetailById(command.OwnerId);

            var save = await _genericService.SaveCommandDetail(command);
            if (save == null)
            {
                return Results.NotFound();
            }
            return Results.Ok();
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IResult> UpdateCommand([FromRoute] string id, [FromBody] CommandVM model)
        {
            UserEntity currentUser = GetCurrentUser();
            CommandEntity command = _mapper.Map<CommandEntity>(model);

            var users = await _genericService.GetUsersList();
            command.OwnerId = users.Where(x => x.UserEmail == currentUser.UserEmail).FirstOrDefault().Id;
            command.Owner = await _genericService.GetUserDetailById(command.OwnerId);

            var update = await _genericService.UpdateCommandDetail(command);
            if (update == null)
            {
                return Results.NotFound();
            }

            return Results.Ok();
        }

        // Method to delete the Asset detail
        [HttpDelete(Name = "DeleteCommand")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IResult> DeleteCommand(string Id)
        {
            await _genericService.DeleteCommand(Convert.ToInt32(Id));
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
