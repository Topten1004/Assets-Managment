using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Backend.API.ViewModel
{
    public class CommandVM
    {
        [Required]

        public string? TankName { get; set; }

        [Required]

        public string? Command { get; set; }

    }

    public class SocketCommandVM
    {
        public int Id { get; set; }
        public string? TankName { get; set; }
        public string? Command { get; set; }

        public bool? Flag { get; set; }

        [NotMapped]
        public float MinAmount { get; set; }

        [NotMapped]

        public float MaxAmount { get; set; }
        [NotMapped]
        public string? UserEmail { get; set; }
    }
}
