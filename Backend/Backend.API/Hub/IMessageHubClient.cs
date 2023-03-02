using Backend.API.ViewModel;

namespace Backend.Controllers
{
    public interface IMessageHubClient
    {
        Task SendCommands(List<SocketCommandVM> commands);

        Task SendTotalAsset(PostTotalAsset model);

        Task SendLogs(List<LogVM> logs);
    }
}
