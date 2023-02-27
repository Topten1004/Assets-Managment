using AutoMapper;
using Backend.API.ViewModel;
using Backend.Business.Services;
using Backend.Data;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Backend.Controllers
{
    public class MessageHub : Hub<IMessageHubClient>
    {
        private readonly IGenericService _genericService;

        private readonly IMapper _mapper;

        public MessageHub(IGenericService genericService, IMapper mapper)
        {
            _genericService = genericService;
            _mapper = mapper;
        }

        public async Task SendCommands(List<string> message)
        {

            await Clients.All.SendCommands(message);
        }

        // Client SignalR is connect
        public override async Task<Task> OnConnectedAsync()
        {
            var result = await _genericService.GetCommandsList();
            IEnumerable<CommandVM> models = _mapper.Map<IEnumerable<CommandVM>>(result);
            List<string> commands = new List<string>();

            foreach (var tempModel in models)
            {
                string temp = JsonSerializer.Serialize(tempModel);
                commands.Add(temp);
            }

            await Clients.All.SendCommands(commands);

            return base.OnConnectedAsync(); 
        }
    }
}
