

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VanHanhCD1.Models.BaiLieu
{
    [Table("DCNL_LocBuiC1")]
    public class DongCoLocBuiC1 : IBaiLieu
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string TagName { get; set; } = string.Empty;

        public DateTime ThoiGian { get; set; }

        public double GiaTri { get; set; }
    }
}
