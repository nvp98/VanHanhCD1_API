namespace VanHanhCD1.Models.VoiXiMang
{
    public interface IVoiXiMangEntity
    {
        public string TagName { get; set; }

        public DateTime ThoiGian { get; set; }

        public double GiaTri { get; set; }
    }
}
