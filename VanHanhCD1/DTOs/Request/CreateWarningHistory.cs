namespace VanHanhCD1.DTOs.Request
{
    public class CreateWarningHistory
    {
        public DateTime ThoiGian { get; set; }
        public string TagName {  get; set; }
        public string KhuVuc {  get; set; }
        public string TenThongSo {  get; set; }
        public double GiaTri {  get; set; }
        public int TrangThai { get; set; }
        public string DonVi {  get; set; }
    }
}
