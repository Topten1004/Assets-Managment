using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data.Entities
{
    [Table("Commands")]

    public class CommandEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public CommandTypes Command { get; set; }

        public int  OwnerId { get; set; }

        public string TankName { get; set; }

        public bool? Flag { get; set; }
        public virtual UserEntity? Owner { get; set; }
    }

    public enum CommandTypes
    {
        Fill,
        Repair,
        Both
    }
}