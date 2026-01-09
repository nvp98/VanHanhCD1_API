
using Microsoft.EntityFrameworkCore;
using VanHanhCD1.ExportExcel;
using VanHanhCD1.Models;
using VanHanhCD1.Models.ThieuKet;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Repository.Repositories
{
    public class ThieuKetRepository : IThieuKetRepository
    {
        private readonly AppDbContext _context;
        private readonly ExportThieuKet _exportThieuKet;
        private readonly ExportLBMT _exportLBMT;

        public ThieuKetRepository(AppDbContext context, ExportThieuKet exportThieuKet, ExportLBMT exportLBMT)
        {
            _context = context;
            _exportThieuKet = exportThieuKet;
            _exportLBMT = exportLBMT;
        }

        private async Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDataAsync<T>(
        DbSet<T> dbSet) where T : class, IThieuKet
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
           DbSet<T> dbSet, DateTime from, DateTime to) where T : class, IThieuKet
        {
            if (from >= to) return new List<Dictionary<string, object>>();
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

        public  Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursThieuKetMots()
        {
            return GetLast24HoursDataAsync(_context.thieuKetMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeThieuKetMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.thieuKetMots, from, to);
        }

        public async Task<byte[]> ExportThieuKetMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.thieuKetMots
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportThieuKet.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursThieuKetHais()
        {
           return GetLast24HoursDataAsync(_context.thieuKetHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeThieuKetHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.thieuKetHais, from, to);
        }

        public async Task<byte[]> ExportThieuKetHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.thieuKetHais
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportThieuKet.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongDuoiMayMots()
        {
            return GetLast24HoursDataAsync(_context.locBuiMoiTruongDuoiMayMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruongDuoiMayMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.locBuiMoiTruongDuoiMayMots, from, to);
        }

        public async Task<byte[]> ExportLocBuiMoiTruongDuoiMayMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.locBuiMoiTruongDuoiMayMots
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLBMT.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongDuoiMayHais()
        {
            return GetLast24HoursDataAsync(_context.locBuiMoiTruongDuoiMayHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruongDuoiMayHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.locBuiMoiTruongDuoiMayHais, from, to);
        }

        public async Task<byte[]> ExportLocBuiMoiTruongDuoiMayHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.locBuiMoiTruongDuoiMayHais
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLBMT.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLocBuiMoiTruongMangQuangs()
        {
            return GetLast24HoursDataAsync(_context.locBuiMoiTruongMangQuangs);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLocBuiMoiTruogMangQuangs(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.locBuiMoiTruongMangQuangs, from, to);
        }

        public async Task<byte[]> ExportLocBuiMoiTruongMangQuangs(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.locBuiMoiTruongMangQuangs
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLBMT.GenerateExcelFile(
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

        
        public IEnumerable<LocBuiMoiTruongDuoiMay1> GetLocBuiMoiTruongDuoiMayMotMinValues()
        {
            return _context.locBuiMoiTruongDuoiMayMots
                .FromSqlRaw("GET3Month_MinValue @TableName='VH_LBMT_DuoiMay1'");
        }

        public IEnumerable<LocBuiMoiTruongDuoiMay2> GetLocBuiMoiTruongDuoiMayHaiMinValues()
        {
            return _context.locBuiMoiTruongDuoiMayHais
                .FromSqlRaw("GET3Month_MinValue @TableName='VH_LBMT_DuoiMay2'");
        }

        public IEnumerable<LocBuiMoiTruongMangQuang> GetLocBuiMoiTruongMangQuangMinValues()
        {
            return _context.locBuiMoiTruongMangQuangs
            .FromSqlRaw("GET3Month_MinValue @TableName='VH_LBMT_MangQuang'");
        }

        //Dong Co TK1
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoThieuKet1s()
        {
            return GetLast24HoursDataAsync(_context.dongCoThieuKet1s);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoThieuKet1s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.dongCoThieuKet1s, from, to);
        }

        public Task<byte[]> ExportDongCoThieuKet1s(DateTime from, DateTime to, string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DongCoThieuKet1> GetDongCoThieuKet1MinValues()
        {
            return _context.dongCoThieuKet1s
                 .FromSqlRaw("GET3Month_MinValue @TableName='DongCo_ThieuKet1'");
        }

        //Dong Co TK1

        //Dong Co TK2
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoThieuKet2s()
        {
            return GetLast24HoursDataAsync(_context.dongCoThieuKet2s);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoThieuKet2s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.dongCoThieuKet2s, from, to);
        }

        public Task<byte[]> ExportDongCoThieuKet2s(DateTime from, DateTime to, string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DongCoThieuKet2> GetDongCoThieuKet2MinValues()
        {
            return _context.dongCoThieuKet2s
                .FromSqlRaw("GET3Month_MinValue @TableName='DongCo_ThieuKet2'");
        }
        //Dong Co TK2
    }
}
