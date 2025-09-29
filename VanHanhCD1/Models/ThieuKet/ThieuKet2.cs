
using System.ComponentModel.DataAnnotations;

namespace VanHanhCD1.Models.ThieuKet
{
    public class ThieuKet2 : IThieuKet
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
