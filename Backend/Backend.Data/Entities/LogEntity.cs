using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Data.Entities
{
    [Table("Logs")]
    public class LogEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Type { get; set; }

        public string TankName { get; set; }

        public float Amount { get; set; }

        public string UserEmail { get; set; }

        public string From { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
