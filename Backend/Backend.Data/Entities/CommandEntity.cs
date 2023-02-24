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
        public int Id { get; set; }
        public Command Command { get; set; }

        [ForeignKey(nameof(Command))]
        public int OwnerId { get; set; }
    }

    public enum Command
    {
        None,
        Fill,
        Repair,
    }
}
