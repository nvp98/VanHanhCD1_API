using Microsoft.EntityFrameworkCore;
using VanHanhCD1.ExportExcel;
using VanHanhCD1.Models;
using VanHanhCD1.Models.PhuTro;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Repository.Repositories
{
    public class PhuTroRepository : IPhuTroRepository
    {
        private readonly AppDbContext _context;
        private readonly ExportQuatGio _exportQuatGio;
        private readonly ExportTurbine _exportTurbine;
        private readonly ExportKhuKhiKhoi _exportKhuKhiKhoi;
        private readonly ExportNoiHoiMatVongMot _exportNoiHoiMatVongMot;
        private readonly ExportTramNuocTuanHoan _exportTramNuocTuanHoan;
        public PhuTroRepository(AppDbContext context, ExportQuatGio exportQuatGio,
            ExportTurbine exportTurbine, ExportNoiHoiMatVongMot exportNoiHoiMatVongMot,
            ExportKhuKhiKhoi exportKhuKhiKhoi, ExportTramNuocTuanHoan exportTramNuocTuanHoan)
        {
            _context = context;
            _exportQuatGio = exportQuatGio;
            _exportTurbine = exportTurbine;
            _exportNoiHoiMatVongMot = exportNoiHoiMatVongMot;
            _exportKhuKhiKhoi = exportKhuKhiKhoi;
            _exportTramNuocTuanHoan = exportTramNuocTuanHoan;
        }

        private async Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDataAsync<T>(
        DbSet<T> dbSet) where T : class, IPhuTro
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
           DbSet<T> dbSet, DateTime from, DateTime to) where T : class, IPhuTro
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatGioMotThieuKetMots()
        {
            return GetLast24HoursDataAsync(_context.quatGioMotThieuKetMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatGioMotThieuKetMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.quatGioMotThieuKetMots, from, to);
        }
        public async Task<byte[]> ExportQuatGioMotThieuKetMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.quatGioMotThieuKetMots
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportQuatGio.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatGioHaiThieuKetMots()
        {
            return GetLast24HoursDataAsync(_context.quatGioHaiThieuKetMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatGioHaiThieuKetMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.quatGioHaiThieuKetMots, from, to);
        }

        public async Task<byte[]> ExportQuatGioHaiThieuKetMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.quatGioHaiThieuKetMots
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportQuatGio.GenerateExcelFile(
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
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTurbineMotThieuKetMots()
        {
            return GetLast24HoursDataAsync(_context.turbineMotThieuKetMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTurbineMotThieuKetMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.turbineMotThieuKetMots, from, to);
        }
        public async Task<byte[]> ExportTurbineMotThieuKetMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.turbineMotThieuKetMots
             .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
             .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var excelBytes = _exportTurbine.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTurbineHaiThieuKetMots()
        {
            return GetLast24HoursDataAsync(_context.turbineHaiThieuKetMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTurbineHaiThieuKetMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.turbineHaiThieuKetMots, from, to);
        }
        public async Task<byte[]> ExportTurbineHaiThieuKetMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.turbineHaiThieuKetMots
             .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
             .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var excelBytes = _exportTurbine.GenerateExcelFile(
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
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursNoiHoiMatVongMots()
        {
            return GetLast24HoursDataAsync(_context.noiHoiMatVongMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeNoiHoiMatVongMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.noiHoiMatVongMots, from, to);
        }
        public async Task<byte[]> ExportNoiHoiMatVongMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.noiHoiMatVongMots
             .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
             .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var excelBytes = _exportNoiHoiMatVongMot.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursNoiHoiOngKhoiQuatGioLamMatVongMots()
        {
            return GetLast24HoursDataAsync(_context.noiHoiOngKhoiQuatGioLamMatVongMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeNoiHoiOngKhoiQuatGioLamMatVongMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.noiHoiOngKhoiQuatGioLamMatVongMots, from, to);
        }

        public async Task<byte[]> ExportNoiHoiOngKhoiQuatGioLamMatVongMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.noiHoiOngKhoiQuatGioLamMatVongMots
             .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
             .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var excelBytes = _exportNoiHoiMatVongMot.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatGioBaThieuKetHais()
        {
            return GetLast24HoursDataAsync(_context.quatGioBaThieuKetHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatGioBaThieuKetHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.quatGioBaThieuKetHais, from, to);
        }

        public async Task<byte[]> ExportQuatGioBaThieuKetHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.quatGioBaThieuKetHais
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportQuatGio.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursQuatGioBonThieuKetHais()
        {
            return GetLast24HoursDataAsync(_context.quatGioBonThieuKetHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeQuatGioBonThieuKetHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.quatGioBonThieuKetHais, from, to);
        }

        public async Task<byte[]> ExportQuatGioBonThieuKetHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.quatGioBonThieuKetHais
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportQuatGio.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTurbineBaThieuKetHais()
        {
            return GetLast24HoursDataAsync(_context.turbineBaThieuKetHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTurbineBaThieuKetHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.turbineBaThieuKetHais, from, to);
        }

        public async Task<byte[]> ExportTurbineBaThieuKetHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.turbineBaThieuKetHais
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportTurbine.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTurbineBonThieuKetHais()
        {
            return GetLast24HoursDataAsync(_context.turbineBonThieuKetHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTurbineBonThieuKetHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.turbineBonThieuKetHais, from, to);
        }

        public async Task<byte[]> ExportTurbineBonThieuKetHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.turbineBonThieuKetHais
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportTurbine.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursNoiHoiMatVongHais()
        {
            return GetLast24HoursDataAsync(_context.noiHoiMatVongHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeNoiHoiMatVongHais(DateTime from, DateTime to)
        {
             return SearchByTimeRange(_context.noiHoiMatVongHais, from, to);
        }

        public async Task<byte[]> ExportNoiHoiMatVongHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.noiHoiMatVongHais
            .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
            .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var excelBytes = _exportNoiHoiMatVongMot.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursNoiHoiOngKhoiQuatGioLamMatVongHais()
        {
            return GetLast24HoursDataAsync(_context.noiHoiOngKhoiQuatGioLamMatVongHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeNoiHoiOngKhoiQuatGioLamMatVongHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.noiHoiOngKhoiQuatGioLamMatVongHais, from, to);
        }

        public async Task<byte[]> ExportNoiHoiOngKhoiQuatGioLamMatVongHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.noiHoiOngKhoiQuatGioLamMatVongHais
              .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
              .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var excelBytes = _exportNoiHoiMatVongMot.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursKhuKhiKhoiThieuKetMots()
        {
            return GetLast24HoursDataAsync(_context.khuKhiKhoiThieuKetMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeKhuKhiKhoiThieuKetMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.khuKhiKhoiThieuKetMots, from, to);
        }

        public async Task<byte[]> ExportKhuKhiKhoiThieuKetMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.khuKhiKhoiThieuKetMots
           .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
           .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var excelBytes = _exportKhuKhiKhoi.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursKhuKhiKhoiThieuKetHais()
        {
            return GetLast24HoursDataAsync(_context.khuKhiKhoiThieuKetHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeKhuKhiKhoiThieuKetHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.khuKhiKhoiThieuKetHais, from, to);
        }

        public async Task<byte[]> ExportKhuKhiKhoiThieuKetHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.khuKhiKhoiThieuKetHais
           .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
           .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var excelBytes = _exportKhuKhiKhoi.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTramNuocTuanHoans()
        {
            return GetLast24HoursDataAsync(_context.tramNuocTuanHoans);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTramNuocTuanHoans(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.tramNuocTuanHoans, from, to);
        }

        public async Task<byte[]> ExportTramNuocTuanHoans(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.tramNuocTuanHoans
           .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
           .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var excelBytes = _exportTramNuocTuanHoan.GenerateExcelFile(
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
    }
}
