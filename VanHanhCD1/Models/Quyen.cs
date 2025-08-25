using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VanHanhCD1.Models
{
    [Table("Quyen")]
    public class Quyen
    {
        [Key]
        public int IDQuyen { get; set; }

        public string TenQuyen { get; set; }
    }
}
