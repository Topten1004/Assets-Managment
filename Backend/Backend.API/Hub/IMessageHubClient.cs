namespace Backend.Controllers
{
    public interface IMessageHubClient
    {
        Task SendCommands(List<string> message);
    }
}
