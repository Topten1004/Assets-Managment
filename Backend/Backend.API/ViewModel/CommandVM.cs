using System.Data;

namespace Backend.API.ViewModel
{
    public class CommandVM
    {
        public CommandType Command { get; set; }
        public UserVM? Owner { get; set; }
    }
}
