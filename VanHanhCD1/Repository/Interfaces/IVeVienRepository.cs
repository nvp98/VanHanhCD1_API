using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Models.VeVien;

namespace VanHanhCD1.Repository.Interfaces
{
    public interface IVeVienRepository
    {
        public IEnumerable<LBDO1VeVien> GetLocBuiDaOngMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiDaOngMots();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiDaOngMots(DateTime from, DateTime to);
        public Task<byte[]> ExportLocBuiDaOngMots(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiDaOngHais();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiDaOngHais(DateTime from, DateTime to);
        public Task<byte[]> ExportLocBuiDaOngHais(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatHutChinhs();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatHutChinhs(DateTime from, DateTime to);
        public Task<byte[]> ExportQuatHutChinhs(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiTinhDien();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiTinhDien(DateTime from, DateTime to);
        public Task<byte[]> ExportLocBuiTinhDien(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongPhoiLieuBanLuoc();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruongPhoiLieuBanLuoc(DateTime from, DateTime to);
        public Task<byte[]> ExportLocBuiMoiTruongPhoiLieuBanLuoc(DateTime from, DateTime to, string path);

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongMangThanhPhamTrungChuyen23();
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruongMangThanhPhamTrungChuyen23(DateTime from, DateTime to);
        public Task<byte[]> ExportLocBuiMoiTruongMangThanhPhamTrungChuyen23(DateTime from, DateTime to, string path);

    }
}
