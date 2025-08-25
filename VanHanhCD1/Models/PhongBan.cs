using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VanHanhCD1.Models
{
    [Table("PhongBan")]
    public class PhongBan
    {
        [Key]
        public int IDPhongBan { get; set; }
        public string TenPhongBan { get; set; }
    }
}
