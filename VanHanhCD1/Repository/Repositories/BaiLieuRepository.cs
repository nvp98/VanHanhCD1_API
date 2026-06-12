using Microsoft.EntityFrameworkCore;
using VanHanhCD1.ExportExcel;
using VanHanhCD1.Models;
using VanHanhCD1.Models.BaiLieu;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Repository.Repositories
{
    public class BaiLieuRepository : IBaiLieuRepository
    {
        private readonly AppDbContext _context;
        private readonly ExportBaiLieuLBMTC _exportBaiLieuLBMTC;
        public BaiLieuRepository(AppDbContext context, ExportBaiLieuLBMTC exportBaiLieuLBMTC)
        {
            _context = context;
            _exportBaiLieuLBMTC = exportBaiLieuLBMTC;

        }
        private async Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDataAsync<T>(
        DbSet<T> dbSet) where T : class, IBaiLieu
        {
            var lastestTime = await dbSet
                .OrderByDescending(item => item.ThoiGian)
                .Select(item => item.ThoiGian)
                .FirstOrDefaultAsync();
            if (lastestTime == default)
                return new List<Dictionary<string, object>>();

            var expectedTimes = Enumerable.Range(0, 24)
                .Select(index => lastestTime.AddHours(-index).Date.AddHours(lastestTime.AddHours(-index).Hour))
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
            DbSet<T> dbSet, DateTime from, DateTime to) where T : class, IBaiLieu
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
        //Loc BuiC1s
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC1s()
        {
            return GetLast24HoursDataAsync(_context.dongCoLocBuiC1s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC1s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.dongCoLocBuiC1s, from, to);
        }
        public async Task<byte[]> ExportDongCoLocBuiC1s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.dongCoLocBuiC1s
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportBaiLieuLBMTC.GenerateExcelFile(
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
        public IEnumerable<DongCoLocBuiC1> GetDongCoLocBuiC1MinValues()
        {
            return _context.dongCoLocBuiC1s
                .FromSqlRaw("GET3Month_MinValue @TableName='DCNL_LocBuiC1'");
        }
        //END Loc BuiC1s

        //Loc BuiC2s
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC2s()
        {
            return GetLast24HoursDataAsync(_context.dongCoLocBuiC2s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC2s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.dongCoLocBuiC2s, from, to);
        }
        public async Task<byte[]> ExportDongCoLocBuiC2s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.dongCoLocBuiC2s
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportBaiLieuLBMTC.GenerateExcelFile(
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
        public IEnumerable<DongCoLocBuiC2> GetDongCoLocBuiC2MinValues()
        {
            return _context.dongCoLocBuiC2s
                .FromSqlRaw("GET3Month_MinValue @TableName='DCNL_LocBuiC2'");
        }
        //End Loc BuiC2s

        //Loc BuiC3s
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC3s()
        {
            return GetLast24HoursDataAsync(_context.dongCoLocBuiC3s);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC3s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.dongCoLocBuiC3s, from, to);
        }
        public async Task<byte[]> ExportDongCoLocBuiC3s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.dongCoLocBuiC3s
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportBaiLieuLBMTC.GenerateExcelFile(
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
        public IEnumerable<DongCoLocBuiC3> GetDongCoLocBuiC3MinValues()
        {
            return _context.dongCoLocBuiC3s
                .FromSqlRaw("GET3Month_MinValue @TableName='DCNL_LocBuiC3'");
        }
        //End Loc BuiC3s

        //Loc BuiC4s
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC4s()
        {
            return GetLast24HoursDataAsync(_context.dongCoLocBuiC4s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC4s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.dongCoLocBuiC4s, from, to);
        }
        public async Task<byte[]> ExportDongCoLocBuiC4s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.dongCoLocBuiC4s
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportBaiLieuLBMTC.GenerateExcelFile(
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
        public IEnumerable<DongCoLocBuiC4> GetDongCoLocBuiC4MinValues()
        {
            return _context.dongCoLocBuiC4s
                .FromSqlRaw("GET3Month_MinValue @TableName='DCNL_LocBuiC4'");
        }
        //End Loc BuiC4s

        //Loc BuiC5s
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoLocBuiC5s()
        {
            return GetLast24HoursDataAsync(_context.dongCoLocBuiC5s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoLocBuiC5s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.dongCoLocBuiC5s, from, to);
        }
        public async Task<byte[]> ExportDongCoLocBuiC5s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.dongCoLocBuiC5s
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();
            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();
            var exvelBytes = _exportBaiLieuLBMTC.GenerateExcelFile(
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
        public IEnumerable<DongCoLocBuiC5> GetDongCoLocBuiC5MinValues()
        {
            return _context.dongCoLocBuiC5s
                .FromSqlRaw("GET3Month_MinValue @TableName='DCNL_LocBuiC5'");
        }
        //End Loc BuiC5s

        //Trung The
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoTrungThes(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.dongCoTrungThes, from, to);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoTrungThes()
        {
            return GetLast24HoursDataAsync(_context.dongCoTrungThes);
        }
        public Task<byte[]> ExportDongCoTrungThes(DateTime from, DateTime to, string path)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<DongCoTrungThe> GetDongCoTrungTheMinValues()
        {
            return _context.dongCoTrungThes
                .FromSqlRaw("GET3Month_MinValue @TableName='DCNL_TrungThe'");
        }

        //End Trung The

        //MBA Bai Lieu
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursMBABaiLieus()
        {
            return GetLast24HoursDataAsync(_context.mbaBaiLieus);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeMBABaiLieus(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.mbaBaiLieus, from, to);
        }

        public IEnumerable<MBABaiLieu> GetMBABaiLieuMinValues()
        {
            return _context.mbaBaiLieus
                .FromSqlRaw("GET3Month_MinValue @TableName='MBA_NMNL'");
        }
        //End Bai Lieu

        // Tu Dien Bai Lieu
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursTuDienBaiLieus()
        {
            return GetLast24HoursDataAsync(_context.tuDienBaiLieus);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeTuDienBaiLieus(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.tuDienBaiLieus, from, to);
        }

        public IEnumerable<TuDienBaiLieu> GetTuDienBaiLieusMinValues()
        {
            return _context.tuDienBaiLieus
                .FromSqlRaw("GET3Month_MinValue @TableName='TUDIEN_TT_NMNL'");
        }
        // Tu Dien Bai Lieu End

        //Che bien

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDongCoCheBiens()
        {
            return GetLast24HoursDataAsync(_context.dongCoCheBiens);
        }

        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeDongCoCheBiens(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.dongCoCheBiens, from, to);
        }

        public Task<byte[]> ExportDongCoCheBiens(DateTime from, DateTime to, string path)
        {
            throw new NotImplementedException();
        } 

        public IEnumerable<DongCoCheBien> GetDongCoCheBienMinValues()
        {
            return _context.dongCoCheBiens
                .FromSqlRaw("GET3Month_MinValue @TableName='VHCB_S95'");
        }

        



        //End Che bien










    }
}
