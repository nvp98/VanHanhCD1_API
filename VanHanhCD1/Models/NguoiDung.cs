using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VanHanhCD1.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        public int IDNguoiDung { get; set; }

        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public int? NhanVienID { get; set; }

        [ForeignKey("Quyen")] // ⚠️ NÓI RÕ đây là khóa ngoại trỏ đến navigation Quyen
        public int? IDQuyen { get; set; }

        public int IsLock { get; set; }

        [ForeignKey("NhanVienID")]
        public virtual NhanVien? NhanVien { get; set; }

        public virtual Quyen? Quyen { get; set; }
    }
}
