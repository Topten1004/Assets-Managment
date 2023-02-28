using Backend.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.API.ViewModel
{
    public class GetAssetVM
    {
        [Required]
        [JsonPropertyName("tank_name")]
        public string TankName { get; set; }

        [Required]
        [JsonPropertyName("amount")]
        public float Amount { get; set; }

        [Required]
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }

        [Required]
        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }

        [Required]
        [JsonPropertyName("longitude")]

        public string? UserEmail { get; set; }

        [Required]
        [JsonPropertyName("min_amount")]

        public float? MinAmount { get; set; }

        [Required]
        [JsonPropertyName("max_amount")]

        public float? MaxAmount { get; set; }

        [Required]
        [JsonPropertyName("period")]

        public int Period { get; set; }

        public UserVM? Owner { get; set; }
    }

    public class PostTotalAsset
    {
        public int Count { get; set; }

        public float TotalAmount { get; set; }

        public float? Longitude { get; set; }

        public float? Latitude { get; set; }
    }

    public class BuyAsset
    {
        [Required]
        [JsonPropertyName("user_email")]
        public string UserEmail { get; set; }

        [Required]
        [JsonPropertyName("amount")]

        public float Amount { get; set; }
    }

    public class SellAsset
    {
        [Required]
        [JsonPropertyName("user_email")]

        public string? UserEmail { get; set; }

        [Required]
        [JsonPropertyName("amount")]

        public float Amount { get; set; }
    }
    public class PostAssetVM
    {
        [Required]
        public string? TankName { get; set; }
        [Required]

        public float Latitude { get; set; }
        [Required]

        public float Longitude { get; set; }
        [Required]

        public string? UserEmail { get; set; }
        [Required]

        public float? Limit { get; set; }

        [NotMapped]
        public string? Password { get; set; }
    }

    public partial class UserVM
    {
        [Required]

        public string? UserEmail { get; set; }
        [Required]

        public Role role { get; set; }
    }

    public partial class TotalAssetVM
    {
        public float? TotalAsset { get; set; }

        public int Count { get; set; }
    }
}

