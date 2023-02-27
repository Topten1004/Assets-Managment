using AutoMapper;
using Backend.API.ViewModel;
using Backend.Business.Services;
using Backend.Controllers;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.Json;
using Websocket.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

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
        private IHubContext<MessageHub, IMessageHubClient> _messageHub;

        public CommandController(ILogger<CommandController> logger, IGenericService genericService, IMapper mapper, IHubContext<MessageHub, IMessageHubClient> messageHub)
        {
            _genericService = genericService;
            _logger = logger;
            _mapper = mapper;
            _messageHub = messageHub;
        }

        // Method to get the list of the Commands
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

        public async Task<IResult> GetCommandsList(CommandTypes commandTypes)
        {
            var result = await _genericService.GetCommandsList();
            IEnumerable<CommandVM> models = _mapper.Map<IEnumerable<CommandVM>>(result);

            return Results.Ok(models);
        }

        // Method to Save the Command detail
        [HttpPost(Name = "SaveCommandDetail")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IResult> SaveCommandDetail(CommandVM model)
        {
            CommandEntity command = _mapper.Map<CommandEntity>(model);
            command.Command = (CommandTypes)Enum.Parse(typeof(CommandTypes), model.Command);

            var assets = await _genericService.GetAssetsList();
            command.OwnerId = assets.Where(x => x.TankName == model.TankName).FirstOrDefault().OwnerId;
            command.Owner = await _genericService.GetUserDetailById(command.OwnerId);
            command.Flag = false;

            var save = await _genericService.SaveCommandDetail(command);

            #region Socket
            var result = await _genericService.GetCommandsList();
            IEnumerable<SocketCommandVM> models = _mapper.Map<IEnumerable<SocketCommandVM>>(result);
  
            foreach(var item in models)
            {
                item.UserEmail = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().UserEmail;
            }

            _messageHub.Clients.All.SendCommands(models.ToList());
            #endregion

            if (save == null)
            {
                return Results.NotFound();
            }
                
            return Results.Ok(models);
        }



        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IResult> UpdateCommand([FromRoute] string id)
        {
            var assets = await _genericService.GetAssetsList();

            CommandEntity command = await _genericService.GetCommandDetailById(Convert.ToInt32(id));
            command.Flag = true;

            #region Socket
            var result = await _genericService.GetCommandsList();
            IEnumerable<SocketCommandVM> models = _mapper.Map<IEnumerable<SocketCommandVM>>(result);

            foreach (var item in models)
            {
                item.UserEmail = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().UserEmail;
            }

            _messageHub.Clients.All.SendCommands(models.ToList());
            #endregion

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
    }
}
