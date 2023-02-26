using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        public UserEntity()
        {
            this.Assets = new HashSet<AssetEntity>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserEmail { get; set; }

        public string? Password { get; set; }

        public Role Role { get; set; }
        
        public virtual ICollection<AssetEntity> Assets { get; set; }

    }

    public enum Role
    {
        Manager,
        Admin
    }
}
