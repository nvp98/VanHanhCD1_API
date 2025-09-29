using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using VanHanhCD1.ExportExcel;
using VanHanhCD1.Models;
using VanHanhCD1.Models.VeVien;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Repository.Repositories
{
    public class VeVienRepository : IVeVienRepository
    {
        private readonly AppDbContext _context;
        private readonly ExportLBDO1 _exportLBDO1;
        private readonly ExportQHCVeVien _exportQHCVeVien;
        private readonly ExportLBTDVeVien _exportLBTDVeVien;
        private readonly ExportLBMTPLieuBLuocVeVien _exportLBMTPLieuBLuocVeVien;
        public VeVienRepository(AppDbContext context, ExportLBDO1 exportLBDO1, 
            ExportQHCVeVien exportQHCVeVien, ExportLBTDVeVien exportLBTDVeVien,
            ExportLBMTPLieuBLuocVeVien exportLBMTPLieuBLuocVeVien
            )
        {
            _context = context;
            _exportLBDO1 = exportLBDO1;
            _exportQHCVeVien = exportQHCVeVien;
            _exportLBTDVeVien = exportLBTDVeVien;
            _exportLBMTPLieuBLuocVeVien = exportLBMTPLieuBLuocVeVien;
        }

        private async Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDataAsync<T>(
        DbSet<T> dbSet) where T : class, IVeVienEntity
        {
            var latestTime = await dbSet
                .OrderByDescending(item => item.ThoiGian)
                .Select(item => item.ThoiGian)
                .FirstOrDefaultAsync();

            if (latestTime == default)
                return new List<Dictionary<string, object>>();

            var expectedTimes = Enumerable.Range(0, 24)
                .Select(index => latestTime.AddHours(-index).Date.AddHours(latestTime.AddHours(-index).Hour))
                .OrderBy(time => time)
                .ToList();

            var allData = await dbSet
                .Where(item => expectedTimes.Any(time =>
                    item.ThoiGian.Date == time.Date && item.ThoiGian.Hour == time.Hour))
                .ToListAsync();

            var rawData = allData
                .GroupBy(item => new { item.ThoiGian.Date, item.ThoiGian.Hour, item.TagName })
                .Select(g => g.OrderByDescending(item => item.ThoiGian).First())
                .ToList();

            var result = expectedTimes.Select(time =>
            {
                var group = rawData
                    .Where(item => item.ThoiGian.Date == time.Date && item.ThoiGian.Hour == time.Hour);
                var row = new Dictionary<string, object> { ["ThoiGian"] = time };

                foreach (var item in group)
                {
                    if (!string.IsNullOrEmpty(item.TagName))
                    {
                        row[item.TagName] = item.GiaTri;
                    }
                }
                return row;
            }).ToList();

            return result;
        }

        private async Task<IEnumerable<Dictionary<string, object>>> SearchByTimeRange<T>(
            DbSet<T> dbSet, DateTime from, DateTime to) where T : class, IVeVienEntity
        {
            if(from >= to) return new List<Dictionary<string, object>>();
            var dataInRange = await dbSet
                .Where(item => item.ThoiGian >= from && item.ThoiGian <= to)
                .ToListAsync();
            var rawData = dataInRange
               .GroupBy(item => new { item.ThoiGian.Date, item.ThoiGian.Hour, item.TagName })
               .Select(g => g.OrderByDescending(item => item.ThoiGian).First())
               .ToList();

            var uniqueTimes = rawData
                .Select(item => item.ThoiGian.Date.AddHours(item.ThoiGian.Hour))
                .Distinct()
                .OrderBy(time => time)
                .ToList();

            var result = uniqueTimes.Select(time =>
            {
                var group = rawData.Where(x => x.ThoiGian.Date == time.Date && x.ThoiGian.Hour == time.Hour);
                var row = new Dictionary<string, object> { ["ThoiGian"] = time };

                foreach (var item in group)
                {
                    if (!string.IsNullOrEmpty(item.TagName))
                        row[item.TagName] = item.GiaTri;
                }

                return row;
            }).ToList();
            return result;

        }

        public  Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiDaOngMots()
        {
            return  GetLast24HoursDataAsync(_context.LBDO1VeViens);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiDaOngMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.LBDO1VeViens, from, to);
        }


        public  Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiDaOngHais()
        {
            return  GetLast24HoursDataAsync(_context.LBDO2VeViens);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiDaOngHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.LBDO2VeViens, from, to);
        }


        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatHutChinhs()
        {
            return GetLast24HoursDataAsync(_context.quatHutChinhs);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatHutChinhs(DateTime from, DateTime to)
        {
           return SearchByTimeRange(_context.quatHutChinhs, from, to);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiTinhDien()
        {
            return GetLast24HoursDataAsync(_context.locBuiTinhDiens);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiTinhDien(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.locBuiTinhDiens, from, to);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongPhoiLieuBanLuoc()
        {
            return GetLast24HoursDataAsync(_context.locBuiMoiTruongPLieuBLuocs);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruongPhoiLieuBanLuoc(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.locBuiMoiTruongPLieuBLuocs, from, to);
        }


        public IEnumerable<LBDO1VeVien> GetLocBuiDaOngMots()
        {
            return _context.LBDO1VeViens
                .OrderByDescending(x  => x.ID)
                .Take(10)
                .ToList();
        }

        public async Task<byte[]> ExportLocBuiDaOngMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.LBDO1VeViens
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

        
            var excelBytes = _exportLBDO1.GenerateExcelFile(
                grouped,
                path,
                from,
                to,
                x => x.ThoiGian,
                x => x.TagName,
                x => x.GiaTri,
                x => x.TagName
                );

            return excelBytes;


        }

        public async Task<byte[]> ExportLocBuiDaOngHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.LBDO2VeViens
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLBDO1.GenerateExcelFile(
                grouped,
                path,
                from,
                to,
                x => x.ThoiGian,
                x => x.TagName,
                x => x.GiaTri,
                x => x.TagName
                );

            return excelBytes;
        }

        public async Task<byte[]> ExportQuatHutChinhs(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.quatHutChinhs
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportQHCVeVien.GenerateExcelFile(
                grouped,
                path,
                from,
                to,
                x => x.ThoiGian,
                x => x.TagName,
                x => x.GiaTri,
                x => x.TagName
                );
            return exvelBytes;
        }

        public async Task<byte[]> ExportLocBuiTinhDien(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.locBuiTinhDiens
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportLBTDVeVien.GenerateExcelFile(
                grouped,
                path,
                from,
                to,
                x => x.ThoiGian,
                x => x.TagName,
                x => x.GiaTri,
                x => x.TagName
                );
            return exvelBytes;
        }

        public async Task<byte[]> ExportLocBuiMoiTruongPhoiLieuBanLuoc(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.locBuiMoiTruongPLieuBLuocs
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportLBMTPLieuBLuocVeVien.GenerateExcelFile(
                grouped,
                path,
                from,
                to,
                x => x.ThoiGian,
                x => x.TagName,
                x => x.GiaTri,
                x => x.TagName
                );
            return exvelBytes;
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongMangThanhPhamTrungChuyen23()
        {
            return GetLast24HoursDataAsync(_context.locBuiMoiTruongMangThanhPhamTrungChuyen23s);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruongMangThanhPhamTrungChuyen23(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.locBuiMoiTruongMangThanhPhamTrungChuyen23s, from, to);
        }

        public async Task<byte[]> ExportLocBuiMoiTruongMangThanhPhamTrungChuyen23(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.locBuiMoiTruongMangThanhPhamTrungChuyen23s
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportLBMTPLieuBLuocVeVien.GenerateExcelFile(
                grouped,
                path,
                from,
                to,
                x => x.ThoiGian,
                x => x.TagName,
                x => x.GiaTri,
                x => x.TagName
                );
            return exvelBytes;
        }
    }
}
