using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Backend.API.ViewModel
{
    public class LogVM
    {
        [Required]
        public string Type { get; set; }

        [Required]

        public string TankName { get; set; }

        [Required]

        public float Amount { get; set; }

        public string? UserEmail { get; set; }

        public string From { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
