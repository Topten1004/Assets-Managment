using System.Data;

namespace Backend.API.ViewModel
{
    public class CommandVM
    {
        public CommandType command { get; set; }

        public int ownerId { get; set; }
    }
}
