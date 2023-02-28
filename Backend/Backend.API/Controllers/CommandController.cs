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

        public async Task<IActionResult> GetCommandsList()
        {
            var result = await _genericService.GetCommandsList();
            IEnumerable<CommandVM> models = _mapper.Map<IEnumerable<CommandVM>>(result);

            return Ok(models);
        }

        // Method to Save the Command detail
        [HttpPost(Name = "SaveCommandDetail")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveCommandDetail(CommandVM model)
        {

            var assets = await _genericService.GetAssetsList();
            var commands = await _genericService.GetCommandsList();

            if (assets.Where(x => x.TankName == model.TankName).Count() == 0) {
                return BadRequest("Can't find tank.");
            }
            //if (commands.Where(x => (x.Flag == false && x.TankName == model.TankName && x.Command == (CommandTypes)Enum.Parse(typeof(CommandTypes), model.Command))).ToList().Count() > 0){
            //    return BadRequest("Command is already exists!");                    
            //}

            CommandEntity command = _mapper.Map<CommandEntity>(model);
            command.Command = (CommandTypes)Enum.Parse(typeof(CommandTypes), model.Command);

            command.OwnerId = assets.Where(x => x.TankName == model.TankName).FirstOrDefault().OwnerId;
            command.Owner = await _genericService.GetUserDetailById(command.OwnerId);
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
  
            foreach(var item in models)
            {
                item.UserEmail = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().UserEmail;
                item.MinAmount = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().MinAmount;
                item.MaxAmount = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().MaxAmount;
            }

            await _messageHub.Clients.All.SendCommands(models.ToList());
            #endregion
               
            return Ok(models);
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCommand([FromRoute] string id)
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
                item.MinAmount = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().MinAmount;
                item.MaxAmount = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().MaxAmount;

            }

            await _messageHub.Clients.All.SendCommands(models.ToList());
            #endregion

            var update = await _genericService.UpdateCommandDetail(command);
            if (update == null)
            {
                return NotFound();
            }

            return Ok();
        }

        // Method to delete the Asset detail
        [HttpDelete(Name = "DeleteCommand")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCommand(string Id)
        {
            await _genericService.DeleteCommand(Convert.ToInt32(Id));
            return Ok();
        }
    }
}
