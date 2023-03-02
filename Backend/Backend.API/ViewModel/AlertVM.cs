using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.API.ViewModel
{
    public class AlertVM
    {
        [Required]
        [JsonPropertyName("minAmount")]
        public float MinAmount { get; set; }

        [Required]
        [JsonPropertyName("period")]
        public int Period { get; set; }
    }
}
