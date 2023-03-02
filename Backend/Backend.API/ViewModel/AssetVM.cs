using Backend.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.API.ViewModel
{
    public partial class GetAssetVM
    {
        [Required]
        [JsonPropertyName("tankName")]
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
        [JsonPropertyName("userEmail")]

        public string? UserEmail { get; set; }

        [Required]
        [JsonPropertyName("minAmount")]

        public float? MinAmount { get; set; }

        [Required]
        [JsonPropertyName("maxAmount")]

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

        public List<GetAssetVM> Assets { get; set; }
    }

    public partial class RepairAssetVM
    {
        [Required]
        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; }

        [Required]
        [JsonPropertyName("period")]
        public int Period { get; set; }
    }

    public partial class BuyAsset
    {
        [Required]
        [JsonPropertyName("userEmail")]
        public string UserEmail { get; set; }

        [Required]
        [JsonPropertyName("amount")]

        public float Amount { get; set; }

        [Required]
        [JsonPropertyName("from")]
        public string From { get; set; }
    }

    public partial class SellAsset
    {
        [Required]
        [JsonPropertyName("userEmail")]

        public string? UserEmail { get; set; }

        [Required]
        [JsonPropertyName("amount")]

        public float Amount { get; set; }

        [Required]
        [JsonPropertyName("from")]

        public string From { get; set; }
    }
    public partial class PostAssetVM
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

        public float? MaxAmount { get; set; }

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
}

