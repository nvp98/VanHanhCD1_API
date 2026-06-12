namespace VanHanhCD1.Models.BaiLieu
{
    public interface IBaiLieu
    {
        public string TagName { get; set; }

        public DateTime ThoiGian { get; set; }

        public double GiaTri { get; set; }
    }
}
