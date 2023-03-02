using AutoMapper;
using Backend.API.ViewModel;
using Backend.Business.Services;
using Backend.Controllers;
using Backend.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Backend.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        private readonly ILogger<AssetController> _logger;
        private readonly IGenericService _genericService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private IHubContext<MessageHub, IMessageHubClient> _messageHub;

        public LogController(ILogger<AssetController> logger, IGenericService genericService, IMapper mapper, IConfiguration config, IHubContext<MessageHub, IMessageHubClient> messageHub)
        {
            _genericService = genericService;
            _logger = logger;
            _mapper = mapper;
            _config = config;
            _messageHub = messageHub;
        }


        #region GetLogsList
        // Method to get the list of the Logs
        [HttpGet]
        [Route("")]
        [ProducesResponseType(typeof(List<LogEntity>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLogsList()
        {
            try
            {
                var result = await _genericService.GetLogsList();
                List<LogVM> models = _mapper.Map<IEnumerable<LogVM>>(result).ToList();

                await _messageHub.Clients.All.SendLogs(models);

                if (models == null)
                {
                    return NotFound();
                }

                return Ok(models);
            }
            catch(Exception ex) {
                var msg = $"Method: GetLogsList, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/LogController/GetLogsList", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion


        #region SaveLogDetails
        [HttpPost]
        [Route("")]
        [ProducesResponseType(typeof(LogEntity), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SaveLogDetail(LogVM model)
        {
            try
            {
                var assets = await _genericService.GetAssetsList();
                if (assets.Where(x => x.TankName == model.TankName).Count() == 0)
                    return BadRequest("Can't find Tank");

                LogEntity log = _mapper.Map<LogEntity>(model);
                log.CreatedDate = DateTime.UtcNow;
                log.UserEmail = assets.Where(x => x.TankName == model.TankName).FirstOrDefault().UserEmail;

                var save = await _genericService.SaveLogDetail(log);

                if (save == null)
                {
                    return NotFound();
                }

                return Ok(save);
            } catch (Exception ex)
            {
                var msg = $"Method: SaveLogDetail, Exception: {ex.Message}";

                _logger.LogError(msg);

                return Problem(title: "/LogController/SaveLogDetail", detail: ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
        #endregion
    }
}
