using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VanHanhCD1.Models.PhuTro
{
    [Table("VH_NoiHoiOngKhoiQuatGioLamMatVong2")]
    public class NoiHoiOngKhoiQuatGioLamMatVong2:IPhuTro
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
