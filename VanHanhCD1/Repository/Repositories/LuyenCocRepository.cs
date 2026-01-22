using DocumentFormat.OpenXml.Office.CustomUI;
using Microsoft.EntityFrameworkCore;
using VanHanhCD1.Exceptions;
using VanHanhCD1.ExportExcel;
using VanHanhCD1.Models;
using VanHanhCD1.Models.LuyenCoc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Repository.Repositories
{
    public class LuyenCocRepository : ILuyenCocRepository
    {
        private readonly AppDbContext _context;
        private readonly ExportLuyenCocCDQ _exportLuyenCocCDQ;
        private readonly ExportLuyenCocMayNghien _exportLuyenCocMayNghien;
        private readonly ExportLuyenCocQGTH _exportLuyenCocQGTH;
        private readonly ExportLuyenCocLBMTMD _exportLuyenCocLBMTMD;
        private readonly ExportLuyenCocLocBuiNhaSang _exportLuyenCocLocBuiNhaSang;
        public LuyenCocRepository(AppDbContext context, ExportLuyenCocCDQ exportLuyenCocCDQ,
            ExportLuyenCocMayNghien exportLuyenCocMayNghien, ExportLuyenCocQGTH exportLuyenCocQGTH,
            ExportLuyenCocLBMTMD exportLuyenCocLBMTMD, ExportLuyenCocLocBuiNhaSang exportLuyenCocLocBuiNhaSang
            )
        {
            _context = context;
            _exportLuyenCocCDQ = exportLuyenCocCDQ;
            _exportLuyenCocMayNghien = exportLuyenCocMayNghien; 
            _exportLuyenCocQGTH = exportLuyenCocQGTH;
            _exportLuyenCocLBMTMD = exportLuyenCocLBMTMD;
            _exportLuyenCocLocBuiNhaSang = exportLuyenCocLocBuiNhaSang;
        }

        private async Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursDataAsync<T>(
        DbSet<T> dbSet) where T : class, ILuyenCoc
        {
            var lastestTime = await dbSet
                .OrderByDescending(item => item.ThoiGian)
                .Select(item => item.ThoiGian)
                .FirstOrDefaultAsync();
            if(lastestTime == default)
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
                .Select(g =>g.OrderByDescending(item => item.ThoiGian).First())
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
            DbSet<T> dbSet, DateTime from, DateTime to) where T : class, ILuyenCoc
        {
            if (from >= to) return new List<Dictionary<string, object>>();
            var dataInRange = await dbSet
                .Where(item => item.ThoiGian >= from && item.ThoiGian <= to)
                .ToListAsync();
            
            var rawData = dataInRange
                .GroupBy(item => new {item.ThoiGian.Date, item.ThoiGian.Hour, item.TagName})
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

        //CDQ1
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocCDQ1s()
        {
            return GetLast24HoursDataAsync(_context.luyenCocCDQ1s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocCDQ1s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocCDQ1s, from, to);
        }
        public async Task<byte[]> ExportLuyenCocCDQ1s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.luyenCocCDQ1s
             .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
             .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocCDQ.GenerateExcelFile(
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

        //EndCDQ1

        //CDQ2
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocCDQ2s()
        {
            return GetLast24HoursDataAsync(_context.luyenCocCDQ2s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocCDQ2s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocCDQ2s, from, to);
        }
        public async Task<byte[]> ExportLuyenCocCDQ2s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.luyenCocCDQ2s
            .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
            .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocCDQ.GenerateExcelFile(
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
        //EndCDQ2

        //CDQ3
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocCDQ3s()
        {
            return GetLast24HoursDataAsync(_context.luyenCocCDQ3s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocCDQ3s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocCDQ3s, from, to);
        }
        public async Task<byte[]> ExportLuyenCocCDQ3s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.luyenCocCDQ3s
            .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
            .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocCDQ.GenerateExcelFile(
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
        //EndCDQ3

        //MatDat1

        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocLocBuiMoiTruongMatDat1s()
        {
            return GetLast24HoursDataAsync(_context.luyenCocLocBuiMoiTruongMatDat1s);

        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocLocBuiMoiTruongMatDat1s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocLocBuiMoiTruongMatDat1s, from, to);
        }
        public async Task<byte[]> ExportLuyenCocLocBuiMoiTruongMatDat1s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.luyenCocLocBuiMoiTruongMatDat1s
            .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
            .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocLBMTMD.GenerateExcelFile(
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
        //MatDat1

        //MatDat2
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocLocBuiMoiTruongMatDat2s()
        {
            return GetLast24HoursDataAsync(_context.luyenCocLocBuiMoiTruongMatDat2s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocLocBuiMoiTruongMatDat2s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocLocBuiMoiTruongMatDat2s, from, to);
        }
        public async Task<byte[]> ExportLuyenCocLocBuiMoiTruongMatDat2s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.luyenCocLocBuiMoiTruongMatDat2s
             .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
             .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocLBMTMD.GenerateExcelFile(
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
        //MatDat2

        //NhaSang
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocLocBuiNhaSang2s()
        {
            return GetLast24HoursDataAsync(_context.luyenCocLocBuiNhaSang2s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocLocBuiNhaSang2s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocLocBuiNhaSang2s, from, to);
        }
        public async Task<byte[]> ExportLuyenCocLocBuiNhaSang2s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.luyenCocLocBuiNhaSang2s
             .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
             .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocLocBuiNhaSang.GenerateExcelFile(
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
        //EndNhaSang

        //MayNghien
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocMayNghiens()
        {
            return GetLast24HoursDataAsync(_context.luyenCocMayNghiens);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocMayNghiens(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocMayNghiens, from, to);
        }
        public async Task<byte[]> ExportLuyenCocMayNghiens(DateTime from, DateTime to, string path) 
        {
            var dataInRange = await _context.luyenCocMayNghiens
            .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
            .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocMayNghien.GenerateExcelFile(
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

        //EndMayNghien

        //QuatTuanHoan1
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocQuatTuanHoan1s()
        {
            return GetLast24HoursDataAsync(_context.luyenCocQuatTuanHoan1s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocQuatTuanHoan1s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocQuatTuanHoan1s, from, to);
        }
        public async Task<byte[]> ExportLuyenCocQuatTuanHoan1s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.luyenCocQuatTuanHoan1s
           .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
           .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocQGTH.GenerateExcelFile(
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
        //EndQuatTuanHoan1

        //QuatTuanHoan2
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocQuatTuanHoan2s()
        {
            return GetLast24HoursDataAsync(_context.luyenCocQuatTuanHoan2s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocQuatTuanHoan2s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocQuatTuanHoan2s, from, to);
        }
        public async Task<byte[]> ExportLuyenCocQuatTuanHoan2s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.luyenCocQuatTuanHoan2s
          .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
          .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocQGTH.GenerateExcelFile(
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
        //EndQuatTuanHoan2

        //QuatTuanHoan3
        public Task<IEnumerable<Dictionary<string, object>>> GetLast24HoursLuyenCocQuatTuanHoan3s()
        {
            return GetLast24HoursDataAsync(_context.luyenCocQuatTuanHoan3s);
        }
        public Task<IEnumerable<Dictionary<string, object>>> GetSearchTimeLuyenCocQuatTuanHoan3s(DateTime from, DateTime to)
        {
            return SearchByTimeRange(_context.luyenCocQuatTuanHoan3s, from, to);
        }
        public async Task<byte[]> ExportLuyenCocQuatTuanHoan3s(DateTime from, DateTime to, string path)
        {
            var dataInRange = await _context.luyenCocQuatTuanHoan3s
          .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
          .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();


            var excelBytes = _exportLuyenCocQGTH.GenerateExcelFile(
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
        //QuatTuanHoan3
        static readonly HashSet<string> Lo1Tags = new()
        {
                "LO101","LO102","LO103","LO104","LO105","LO106","LO107","LO108","LO109", "LO110",
                "LO111","LO112","LO113","LO114","LO115","LO116","LO117","LO118","LO119", "LO120",
        };
        static readonly HashSet<string> Lo2Tags = new()
        {
                "LO201","LO202","LO203","LO204","LO205","LO206","LO207","LO208","LO209", "LO210",
                "LO211","LO212","LO213","LO214","LO215","LO216","LO217","LO218","LO219", "LO220",
        };
        static readonly HashSet<string> Lo3Tags = new()
        {
                "LO301","LO302","LO303","LO304","LO305","LO306","LO307","LO308","LO309", "LO310",
                "LO311","LO312","LO313","LO314","LO315","LO316","LO317","LO318","LO319", "LO320",
        };
        static readonly HashSet<string> Lo4Tags = new()
        {
                "LO401","LO402","LO403","LO404","LO405","LO406","LO407","LO408","LO409", "LO410",
                "LO411","LO412","LO413","LO414","LO415","LO416","LO417","LO418","LO419", "LO420",
        };
        string GetCum(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return "";

            var baseTag = tagName.Split('_')[0];   // LO101_H → LO101

            if (Lo1Tags.Contains(baseTag)) return "Lo 1";
            if (Lo2Tags.Contains(baseTag)) return "Lo 2";
            if (Lo3Tags.Contains(baseTag)) return "Lo 3";
            if (Lo4Tags.Contains(baseTag)) return "Lo 4";

            return "";
        }


        public  IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum12(DateTime? times)
        {
            if (times==null) throw new AppException(ErrorCode.VALIDATE_TIME_EXCEPTION);

            var dataInRange = _context.luyenCocCum12s
                .Where(x => x.ThoiGian == times)
                .ToList();
            var rawData = dataInRange
                .GroupBy(item => new { item.ThoiGian.Hour, item.ThoiGian.Minute, item.TagName })
                .Select(g => g.OrderByDescending(item => item.ThoiGian).First())
                .ToList();

            var uniqueTimes = rawData
                .Select(item => item.ThoiGian.Date.AddHours(item.ThoiGian.Hour).AddMinutes(item.ThoiGian.Minute))
                .Distinct()
                .OrderBy(time => time)
                .ToList();
            var result = uniqueTimes.Select(time =>
            {
                var group = rawData.Where(x => x.ThoiGian.Date == time.Date && x.ThoiGian.Hour == time.Hour);

                var row = new Dictionary<string, object>
                {
                    ["ThoiGian"] = time,
                    ["Lo 1"] = new Dictionary<string, object>(),
                    ["Lo 2"] = new Dictionary<string, object>(),
                    ["Lo 3"] = new Dictionary<string, object>(),
                    ["Lo 4"] = new Dictionary<string, object>()
                };

                var lo1 = (Dictionary<string, object>)row["Lo 1"];
                var lo2 = (Dictionary<string, object>)row["Lo 2"];
                var lo3 = (Dictionary<string, object>)row["Lo 3"];
                var lo4 = (Dictionary<string, object>)row["Lo 4"];

                foreach (var item in group)
                {
                    if (string.IsNullOrEmpty(item.TagName)) continue;

                    var cum = GetCum(item.TagName);

                    switch (cum)
                    {
                        case "Lo 1":
                            lo1[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 2":
                            lo2[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 3":
                            lo3[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 4":
                            lo4[item.TagName] = item.GiaTri;
                            break;
                    }
                }
                return row;
            }).ToList();

            return result;

        }

    }
}
