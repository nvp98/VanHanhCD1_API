using System.ComponentModel.DataAnnotations.Schema;

namespace VanHanhCD1.Models.LuyenCoc
{
    public interface ILuyenCoc
    {
        public string TagName { get; set; }

        public DateTime ThoiGian { get; set; }

        public double GiaTri { get; set; }
    }
}
