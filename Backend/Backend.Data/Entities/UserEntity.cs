using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data.Entities
{
    [Table("Users")]

    public class UserEntity
    {
        public int Id { get; set; }

        public string? UserEmail { get; set; }

        public Role Role { get; set; }
    }

    public enum Role
    {
        Manager,
        Owner
    }
}
