
using System.ComponentModel.DataAnnotations.Schema;
namespace VanHanhCD1.Models
{
    public class NhanVien
    {
        public int ID { get; set; }
        public string MaNV { get; set; }
        public string HoTen { get; set; }
        [ForeignKey("IDPhongBan")]
        public virtual PhongBan? PhongBan { get; set; }
        public int? IDPhongBan { get; set; }

    }
}
