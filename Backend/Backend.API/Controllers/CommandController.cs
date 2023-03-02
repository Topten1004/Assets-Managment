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

        #region GetCommandsList
        // Method to get the list of the Commands
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetCommandsList()
        {
            try
            {
                var result = await _genericService.GetCommandsList();
                IEnumerable<CommandVM> models = _mapper.Map<IEnumerable<CommandVM>>(result);

                return Ok(models);

            }
            catch (Exception ex)
            {
                var msg = $"Method: GetCommandsList, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/CommandController/GetCommandsList", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region SaveCommandDetail
        // Method to Save the Command detail
        [HttpPost(Name = "SaveCommandDetail")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveCommandDetail(CommandVM model)
        {
            try
            {
                var assets = await _genericService.GetAssetsList();
                var commands = await _genericService.GetCommandsList();

                if (assets.Where(x => x.TankName == model.TankName).Count() == 0)
                {
                    return BadRequest("Can't find tank.");
                }

                CommandEntity command = _mapper.Map<CommandEntity>(model);
                command.Command = (CommandTypes)Enum.Parse(typeof(CommandTypes), model.Command);

                command.OwnerId = assets.Where(x => x.TankName == model.TankName).FirstOrDefault().OwnerId;
                command.Owner = await _genericService.GetUserDetailById(command.OwnerId);
                command.Flag = false;

                await _genericService.SaveCommandDetail(command);

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

                return Ok(models);

            } catch(Exception ex)
            {
                var msg = $"Method: SaveCommandDetail, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/CommandController/SaveCommandDetail", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }            
        }
        #endregion


        #region UpdateCommand
        // Update Command Method
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(CommandEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCommand([FromRoute] string id)
        {
            try
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

                return Ok(update);

            }
            catch (Exception ex)
            {
                var msg = $"Method: UpdateCommand, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/CommandController/UpdateCommand", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region DeleteCommand
        // Method to delete the Command detail
        [HttpDelete(Name = "DeleteCommand")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCommand(string Id)
        {
            try
            {
                await _genericService.DeleteCommand(Convert.ToInt32(Id));
                return Ok();
            }
            catch (Exception ex)
            {
                var msg = $"Method: DeleteCommand, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/CommandController/UpdateCommand", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion
    }
}
