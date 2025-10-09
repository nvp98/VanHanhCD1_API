using Microsoft.EntityFrameworkCore;
using VanHanhCD1.ExportExcel;
using VanHanhCD1.Models;
using VanHanhCD1.Models.VoiXiMang;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Repository.Repositories
{
    public class VoiXiMangRepository: IVoiXiMangRepository
    {
        private readonly AppDbContext _context;
        private readonly ExportVoiXiMang _exportVoiXiMang;
        private readonly ExportLoVoiQuay _exportLoVoiQuay;

        public VoiXiMangRepository(AppDbContext context, ExportVoiXiMang exportVoiXiMang, ExportLoVoiQuay exportLoVoiQuay)
        {
            _context = context;
            _exportVoiXiMang = exportVoiXiMang;
            _exportLoVoiQuay = exportLoVoiQuay;
        }

        private async Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDataAsync<T>(
        DbSet<T> dbSet) where T : class, IVoiXiMangEntity
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
           DbSet<T> dbSet, DateTime from, DateTime to) where T : class, IVoiXiMangEntity
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
        //Mot
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoVoiDungMots()
        {
            return GetLast24HoursDataAsync(_context.loVoiDungMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoVoiDungMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.loVoiDungMots, from, to);
        }

        public async Task<byte[]> ExportLoVoiDungMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.loVoiDungMots
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportVoiXiMang.GenerateExcelFile(
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
        //Mot
        
        //Hai
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoVoiDungHais()
        {
            return GetLast24HoursDataAsync(_context.loVoiDungHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoVoiDungHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.loVoiDungHais, from, to);
        }

        public async Task<byte[]> ExportLoVoiDungHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.loVoiDungHais
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportVoiXiMang.GenerateExcelFile(
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
        //Hai

        //Ba
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoVoiDungBas()
        {
            return GetLast24HoursDataAsync(_context.loVoiDungBas);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoVoiDungBas(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.loVoiDungBas, from, to);
        }

        public async Task<byte[]> ExportLoVoiDungBas(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.loVoiDungBas
            .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
            .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportVoiXiMang.GenerateExcelFile(
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
        //Ba

        //Mot
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoDoMitMots()
        {
            return GetLast24HoursDataAsync(_context.loDoMitMots);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoDoMitMots(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.loDoMitMots, from, to);
        }

        public async Task<byte[]> ExportLoDoMitMots(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.loDoMitMots
            .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
            .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportVoiXiMang.GenerateExcelFile(
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
        //Mot

        //Hai
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoDoMitHais()
        {
            return GetLast24HoursDataAsync(_context.loDoMitHais);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoDoMitHais(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.loDoMitHais, from, to);
        }

        public async Task<byte[]> ExportLoDoMitHais(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.loDoMitHais
            .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
            .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportVoiXiMang.GenerateExcelFile(
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

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLoVoiQuay()
        {
            return GetLast24HoursDataAsync(_context.loVoiQuays);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLoVoiQuay(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.loVoiQuays, from, to);
        }

        public async Task<byte[]> ExportLoVoiQuay(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.loVoiQuays
            .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
            .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportLoVoiQuay.GenerateExcelFile(
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
        //Hai

    }
}
