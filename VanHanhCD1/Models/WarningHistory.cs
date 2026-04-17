using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VanHanhCD1.Models
{
    [Table("WarningHistory")]
    public class WarningHistory
    {
        [Key]
        public int Id { get; set; }
        public DateTime ThoiGian { get; set; }
        public string TagName { get; set; }
        public string KhuVuc { get; set; }
        public string TenThongSo { get; set; }
        public double GiaTri { get; set; }
        public int TrangThai {  get; set; }
        public string DonVi {  get; set; }
        
    }
}
