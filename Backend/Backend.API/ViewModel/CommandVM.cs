﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Backend.API.ViewModel
{
    public class CommandVM
    {
        public string? TankName { get; set; }

        public string? Command { get; set; }
    }

    public class SocketCommandVM
    {
        public string? TankName { get; set; }
        public CommandType Command { get; set; }
    }
}
