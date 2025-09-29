using System.ComponentModel.DataAnnotations;

namespace VanHanhCD1.Models.VeVien
{
    public interface IVeVienEntity
    {
        public string TagName { get; set; }

        public DateTime ThoiGian { get; set; }

        public double GiaTri { get; set; }
    }
}
