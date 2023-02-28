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

        public async Task SendCommands(List<SocketCommandVM> commands)
        {
            await Clients.All.SendCommands(commands);
        }

        public async Task SendTotalAsset(PostTotalAsset model)
        {
            await Clients.All.SendTotalAsset(model);
        }

        // Client SignalR is connect
        public override async Task<Task> OnConnectedAsync()
        {
            var result = await _genericService.GetCommandsList();
            var assets = await _genericService.GetAssetsList();

            IEnumerable<SocketCommandVM> models = _mapper.Map<IEnumerable<SocketCommandVM>>(result);

            foreach(var item in models)
            {
                item.UserEmail = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().UserEmail;
                item.MinAmount = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().MinAmount;
                item.MaxAmount = assets.Where(x => x.TankName == item.TankName).FirstOrDefault().MaxAmount;

            }

            await Clients.All.SendCommands(models.ToList());

            return base.OnConnectedAsync(); 
        }
    }
}
