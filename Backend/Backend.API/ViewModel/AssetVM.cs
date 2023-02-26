using Backend.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backend.API.ViewModel
{
    public class GetAssetVM
    {
        public string? TankName { get; set; }
        public float Amount { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string? UserEmail { get; set; }

        public UserVM? Owner { get; set; }
    }

    public class PostAssetVM
    {
        public string? TankName { get; set; }
        public float Amount { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string? UserEmail { get; set; }
    }

    public partial class UserVM
    {
        public string? UserEmail { get; set; }

        public Role role { get; set; }
    }
}
