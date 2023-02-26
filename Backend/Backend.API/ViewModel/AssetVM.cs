using Backend.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Backend.API.ViewModel
{
    public class AssetVM
    {
        public string? TankName { get; set; }
        public float Amount { get; set; }

        public float Latitude { get; set; }

        public float Longitude { get; set; }

        public string? UserEmail { get; set; }
    }
}
