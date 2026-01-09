

using VanHanhCD1.Models.ThieuKet;

namespace VanHanhCD1.Repository.Interfaces
{
    public interface IThieuKetRepository
    {
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursThieuKetMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeThieuKetMots(DateTime from, DateTime to);
        public Task<byte[]> ExportThieuKetMots(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursThieuKetHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeThieuKetHais(DateTime from, DateTime to);
        public Task<byte[]> ExportThieuKetHais(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongDuoiMayMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruongDuoiMayMots(DateTime from, DateTime to);
        public Task<byte[]> ExportLocBuiMoiTruongDuoiMayMots(DateTime from, DateTime to, string path);
        public IEnumerable<LocBuiMoiTruongDuoiMay1> GetLocBuiMoiTruongDuoiMayMotMinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongDuoiMayHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruongDuoiMayHais(DateTime from, DateTime to);
        public Task<byte[]> ExportLocBuiMoiTruongDuoiMayHais(DateTime from, DateTime to, string path);
        public IEnumerable<LocBuiMoiTruongDuoiMay2> GetLocBuiMoiTruongDuoiMayHaiMinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongMangQuangs();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruogMangQuangs(DateTime from, DateTime to);
        public Task<byte[]> ExportLocBuiMoiTruongMangQuangs(DateTime from, DateTime to, string path);
        public IEnumerable<LocBuiMoiTruongMangQuang> GetLocBuiMoiTruongMangQuangMinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoThieuKet1s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoThieuKet1s(DateTime from, DateTime to);
        public Task<byte[]> ExportDongCoThieuKet1s(DateTime from, DateTime to, string path);
        public IEnumerable<DongCoThieuKet1> GetDongCoThieuKet1MinValues();

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoThieuKet2s();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoThieuKet2s(DateTime from, DateTime to);
        public Task<byte[]> ExportDongCoThieuKet2s(DateTime from, DateTime to, string path);
        public IEnumerable<DongCoThieuKet2> GetDongCoThieuKet2MinValues();
    }
}
