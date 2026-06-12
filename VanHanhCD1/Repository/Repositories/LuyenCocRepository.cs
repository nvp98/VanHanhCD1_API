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
        public IEnumerable<LuyenCocLocBuiMoiTruongMatDat1> GetLuyenCocLocBuiMoiTruongMatDat1MinValues()
        {
            return _context.luyenCocLocBuiMoiTruongMatDat1s
                .FromSqlRaw("GET3Month_MinValue @TableName='VHLC_LBMT1'");
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

        public IEnumerable<LuyenCocLocBuiMoiTruongMatDat2> GetLuyenCocLocBuiMoiTruongMatDat2MinValues()
        {
            return _context.luyenCocLocBuiMoiTruongMatDat2s
                 .FromSqlRaw("GET3Month_MinValue @TableName='VHLC_LBMT2'");
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
        public IEnumerable<LuyenCocLocBuiNhaSang2> GetLuyenCocLocBuiNhaSang2MinValues()
        {
            return _context.luyenCocLocBuiNhaSang2s
                 .FromSqlRaw("GET3Month_MinValue @TableName='VHLC_LBNS2'");
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

        public IEnumerable<LuyenCocMayNghien> GetLuyenCocLuyenCocMayNghienMinValues()
        {
            return _context.luyenCocMayNghiens
                    .FromSqlRaw("GET3Month_MinValue @TableName='VHLC_COAL'");
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
        public IEnumerable<LuyenCocQuatTuanHoan1> GetLuyenCocLuyenCocQuatTuanHoan1MinValues()
        {
            return _context.luyenCocQuatTuanHoan1s
                .FromSqlRaw("GET3Month_MinValue @TableName='VHLC_QGTH1'");
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
        public IEnumerable<LuyenCocQuatTuanHoan2> GetLuyenCocLuyenCocQuatTuanHoan2MinValues()
        {
            return _context.luyenCocQuatTuanHoan2s
                .FromSqlRaw("GET3Month_MinValue @TableName='VHLC_QGTH2'");
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
        public IEnumerable<LuyenCocQuatTuanHoan3> GetLuyenCocLuyenCocQuatTuanHoan3MinValues()
        {
            return _context.luyenCocQuatTuanHoan3s
                .FromSqlRaw("GET3Month_MinValue @TableName='VHLC_QGTH3'");
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
        //Cum 3 4
        static readonly HashSet<string> Lo5Tags = new()
        {
               "LO501","LO502","LO503","LO504","LO505","LO506","LO507","LO508","LO509", "LO510",
               "LO511","LO512","LO513","LO514","LO515","LO516","LO517","LO518","LO519", "LO520",
        };
        static readonly HashSet<string> Lo6Tags = new()
        {
               "LO601","LO602","LO603","LO604","LO605","LO606","LO607","LO608","LO609", "LO610",
               "LO611","LO612","LO613","LO614","LO615","LO616","LO617","LO618","LO619", "LO620",
        };
        static readonly HashSet<string> Lo7Tags = new()
        {
               "LO701","LO702","LO703","LO704","LO705","LO706","LO707","LO708","LO709", "LO710",
               "LO711","LO712","LO713","LO714","LO715","LO716","LO717","LO718","LO719", "LO720",
        };
        static readonly HashSet<string> Lo8Tags = new()
        {
               "LO801","LO802","LO803","LO804","LO805","LO806","LO807","LO808","LO809", "LO810",
               "LO811","LO812","LO813","LO814","LO815","LO816","LO817","LO818","LO819", "LO820",
        };
        //Cum 3 4
        //Cum 5 6
        static readonly HashSet<string> Lo9Tags = new()
        {
               "L0901","L0902","L0903","L0904","L0905","L0906","L0907","L0908","L0909", "L0910",
               "L0911","L0912","L0913","L0914","L0915","L0916","L0917","L0918","L0919", "L0920",
        };
        static readonly HashSet<string> Lo10Tags = new()
        {
               "L1001","L1002","L1003","L1004","L1005","L1006","L1007","L1008","L1009", "L1010",
               "L1011","L1012","L1013","L1014","L1015","L1016","L1017","L1018","L1019", "L1020",
        };
        static readonly HashSet<string> Lo11Tags = new()
        {
               "L1101","L1102","L1103","L1104","L1105","L1106","L1107","L1108","L1109", "L1110",
               "L1111","L1112","L1113","L1114","L1115","L1116","L1117","L1118","L1119", "L1120",
        };
        static readonly HashSet<string> Lo12Tags = new()
        {
               "L1201","L1202","L1203","L1204","L1205","L1206","L1207","L1208","L1209", "L1210",
               "L1211","L1212","L1213","L1214","L1215","L1216","L1217","L1218","L1219", "L1220",
        };
        //Cum 5 6

        //Cum 7 8
        static readonly HashSet<string> Lo13Tags = new()
        {
               "L1301","L1302","L1303","L1304","L1305","L1306","L1307","L1308","L1309", "L1310",
               "L1311","L1312","L1313","L1314","L1315","L1316","L1317","L1318","L1319", "L1320",
        };
        static readonly HashSet<string> Lo14Tags = new()
        {
               "L1401","L1402","L1403","L1404","L1405","L1406","L1407","L1408","L1409", "L1410",
               "L1411","L1412","L1413","L1414","L1415","L1416","L1417","L1418","L1419", "L1420",
        };
        static readonly HashSet<string> Lo15Tags = new()
        {
               "L1501","L1502","L1503","L1504","L1505","L1506","L1507","L1508","L1509", "L1510",
               "L1511","L1512","L1513","L1514","L1515","L1516","L1517","L1518","L1519", "L1520",
        };
        static readonly HashSet<string> Lo16Tags = new()
        {
               "L1601","L1602","L1603","L1604","L1605","L1606","L1607","L1608","L1609", "L1610",
               "L1611","L1612","L1613","L1614","L1615","L1616","L1617","L1618","L1619", "L1620",
        };
        //Cum7 8
        //Cum 9 10
        static readonly HashSet<string> Lo17Tags = new()
        {
               "17_01H_PV","17_01M_PV","17_02H_PV","17_02M_PV","17_03H_PV","17_03M_PV", "17_04H_PV","17_04M_PV","17_05H_PV","17_05M_PV",
               "17_06H_PV","17_06M_PV","17_07H_PV","17_07M_PV", "17_08H_PV","17_08M_PV", "17_09H_PV","17_09M_PV", "17_10H_PV","17_10M_PV",
               "17_11H_PV","17_11M_PV", "17_12H_PV","17_12M_PV", "17_13H_PV","17_13M_PV", "17_14H_PV","17_14M_PV", "17_15H_PV","17_15M_PV",
               "17_16H_PV","17_16M_PV", "17_17H_PV","17_17M_PV", "17_18H_PV","17_18M_PV", "17_19H_PV","17_19M_PV", "17_20H_PV","17_20M_PV",
        };
        static readonly HashSet<string> Lo18Tags = new()
        {
               "18_01H_PV","18_01M_PV","18_02H_PV","18_02M_PV","18_03H_PV","18_03M_PV", "18_04H_PV","18_04M_PV","18_05H_PV","18_05M_PV",
               "18_06H_PV","18_06M_PV","18_07H_PV","18_07M_PV", "18_08H_PV","18_08M_PV", "18_09H_PV","18_09M_PV", "18_10H_PV","18_10M_PV",
               "18_11H_PV","18_11M_PV", "18_12H_PV","18_12M_PV", "18_13H_PV","18_13M_PV", "18_14H_PV","18_14M_PV", "18_15H_PV","18_15M_PV",
               "18_16H_PV","18_16M_PV", "18_17H_PV","18_17M_PV", "18_18H_PV","18_18M_PV", "18_19H_PV","18_19M_PV", "18_20H_PV","18_20M_PV",
        };
        static readonly HashSet<string> Lo19Tags = new()
        {
               "19_01H_PV","19_01M_PV","19_02H_PV","19_02M_PV","19_03H_PV","19_03M_PV", "19_04H_PV","19_04M_PV","19_05H_PV","19_05M_PV",
               "19_06H_PV","19_06M_PV","19_07H_PV","19_07M_PV", "19_08H_PV","19_08M_PV", "19_09H_PV","19_09M_PV", "19_10H_PV","19_10M_PV",
               "19_11H_PV","19_11M_PV", "19_12H_PV","19_12M_PV", "19_13H_PV","19_13M_PV", "19_14H_PV","19_14M_PV", "19_15H_PV","19_15M_PV",
               "19_16H_PV","19_16M_PV", "19_17H_PV","19_17M_PV", "19_18H_PV","19_18M_PV", "19_19H_PV","19_19M_PV", "19_20H_PV","19_20M_PV",
        };
        static readonly HashSet<string> Lo20Tags = new()
        {
               "20_01H_PV","20_01M_PV","20_02H_PV","20_02M_PV","20_03H_PV","20_03M_PV", "20_04H_PV","20_04M_PV","20_05H_PV","20_05M_PV",
               "20_06H_PV","20_06M_PV","20_07H_PV","20_07M_PV", "20_08H_PV","20_08M_PV", "20_09H_PV","20_09M_PV", "20_10H_PV","20_10M_PV",
               "20_11H_PV","20_11M_PV", "20_12H_PV","20_12M_PV", "20_13H_PV","20_13M_PV", "20_14H_PV","20_14M_PV", "20_15H_PV","20_15M_PV",
               "20_16H_PV","20_16M_PV", "20_17H_PV","20_17M_PV", "20_18H_PV","20_18M_PV", "20_19H_PV","20_19M_PV", "20_20H_PV","20_20M_PV",
        };
        //Cum 9 10

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
        string GetCum34(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return "";
            var baseTag = tagName.Split('_')[0];
            if (Lo5Tags.Contains(baseTag)) return "Lo 5";
            if (Lo6Tags.Contains(baseTag)) return "Lo 6";
            if (Lo7Tags.Contains(baseTag)) return "Lo 7";
            if (Lo8Tags.Contains(baseTag)) return "Lo 8";

            return "";
        }
        string GetCum56(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return "";
            var baseTag = tagName.Split('_')[0];
            if (Lo9Tags.Contains(baseTag)) return "Lo 9";
            if (Lo10Tags.Contains(baseTag)) return "Lo 10";
            if (Lo11Tags.Contains(baseTag)) return "Lo 11";
            if (Lo12Tags.Contains(baseTag)) return "Lo 12";

            return "";
        }
        string GetCum78(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return "";
            var baseTag = tagName.Split('_')[0];
            if (Lo13Tags.Contains(baseTag)) return "Lo 13";
            if (Lo14Tags.Contains(baseTag)) return "Lo 14";
            if (Lo15Tags.Contains(baseTag)) return "Lo 15";
            if (Lo16Tags.Contains(baseTag)) return "Lo 16";

            return "";
        }
        string GetCum910(string tagName)
        {
            if (string.IsNullOrEmpty(tagName)) return "";
            if (Lo17Tags.Contains(tagName)) return "Lo 17";
            if (Lo18Tags.Contains(tagName)) return "Lo 18";
            if (Lo19Tags.Contains(tagName)) return "Lo 19";
            if (Lo20Tags.Contains(tagName)) return "Lo 20";
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

        public IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum34(DateTime? times)
        {
            if (times == null) throw new AppException(ErrorCode.VALIDATE_TIME_EXCEPTION);
            var fromTime = new DateTime(
                times.Value.Year,
                times.Value.Month,
                times.Value.Day,
                times.Value.Hour,
                times.Value.Minute,
                0
                );
            var toTime = fromTime.AddMinutes(1);

            var dataInRange = _context.luyenCocCum34s
                .Where(x => x.ThoiGian >= fromTime && x.ThoiGian < toTime)
                .ToList();
            var rawData = dataInRange
                .GroupBy(item => new { item.ThoiGian.Hour, item.ThoiGian.Minute, item.TagName})
                .Select(g => g.OrderByDescending(item => item.ThoiGian).First())
                .ToList();
            var uniqueTimes = rawData
                .Select(item => item.ThoiGian.Date.AddHours(item.ThoiGian.Hour).AddMinutes(item.ThoiGian.Minute))
                .Distinct()
                .OrderBy(times => times)
                .ToList();
            var result = uniqueTimes.Select(time =>
            {
                var group = rawData.Where(x => x.ThoiGian.Date == time.Date && x.ThoiGian.Hour == time.Hour);
                var row = new Dictionary<string, object>
                {
                    ["ThoiGian"] = time,
                    ["Lo 5"] = new Dictionary<string, object>(),
                    ["Lo 6"] = new Dictionary<string, object>(),
                    ["Lo 7"] = new Dictionary<string, object>(),
                    ["Lo 8"] = new Dictionary<string, object>()
                };

                var lo5 = (Dictionary<string, object>)row["Lo 5"];
                var lo6 = (Dictionary<string, object>)row["Lo 6"];
                var lo7 = (Dictionary<string, object>)row["Lo 7"];
                var lo8 = (Dictionary<string, object>)row["Lo 8"];
                foreach (var item in group)
                {
                    if (string.IsNullOrEmpty(item.TagName)) continue;

                    var cum = GetCum34(item.TagName);

                    switch (cum)
                    {
                        case "Lo 5":
                            lo5[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 6":
                            lo6[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 7":
                            lo7[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 8":
                            lo8[item.TagName] = item.GiaTri;
                            break;
                    }
                }
                return row;
            }).ToList();
            return result;
        }

        public IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum56(DateTime? times)
        {
            if (times == null) throw new AppException(ErrorCode.VALIDATE_TIME_EXCEPTION);
            var fromTime = new DateTime(
                times.Value.Year,
                times.Value.Month,
                times.Value.Day,
                times.Value.Hour,
                times.Value.Minute,
                0
                );
            var toTime = fromTime.AddMinutes(1);

            var dataInRange = _context.luyenCocCum56s
                .Where(x => x.ThoiGian >= fromTime && x.ThoiGian < toTime)
                .ToList();
            var rawData = dataInRange
                .GroupBy(item => new { item.ThoiGian.Hour, item.ThoiGian.Minute, item.TagName })
                .Select(g => g.OrderByDescending(item => item.ThoiGian).First())
                .ToList();
            var uniqueTimes = rawData
                .Select(item => item.ThoiGian.Date.AddHours(item.ThoiGian.Hour).AddMinutes(item.ThoiGian.Minute))
                .Distinct()
                .OrderBy(times => times)
                .ToList();
            var result = uniqueTimes.Select(time =>
            {
                var group = rawData.Where(x => x.ThoiGian.Date == time.Date && x.ThoiGian.Hour == time.Hour);
                var row = new Dictionary<string, object>
                {
                    ["ThoiGian"] = time,
                    ["Lo 9"] = new Dictionary<string, object>(),
                    ["Lo 10"] = new Dictionary<string, object>(),
                    ["Lo 11"] = new Dictionary<string, object>(),
                    ["Lo 12"] = new Dictionary<string, object>()
                };

                var lo9 = (Dictionary<string, object>)row["Lo 9"];
                var lo10 = (Dictionary<string, object>)row["Lo 10"];
                var lo11 = (Dictionary<string, object>)row["Lo 11"];
                var lo12 = (Dictionary<string, object>)row["Lo 12"];
                foreach (var item in group)
                {
                    if (string.IsNullOrEmpty(item.TagName)) continue;

                    var cum = GetCum56(item.TagName);

                    switch (cum)
                    {
                        case "Lo 9":
                            lo9[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 10":
                            lo10[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 11":
                            lo11[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 12":
                            lo12[item.TagName] = item.GiaTri;
                            break;
                    }
                }
                return row;
            }).ToList();
            return result;
        }

        public IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum78(DateTime? times)
        {
            if (times == null) throw new AppException(ErrorCode.VALIDATE_TIME_EXCEPTION);

            var fromTime = new DateTime(
                times.Value.Year,
                times.Value.Month,
                times.Value.Day,
                times.Value.Hour,
                times.Value.Minute,
                0
                );
            var toTime = fromTime.AddMinutes(1);

            var dataInRange = _context.luyenCocCum78s
                .Where(x => x.ThoiGian >= fromTime && x.ThoiGian < toTime)
                .ToList();
            var rawData = dataInRange
                .GroupBy(item => new { item.ThoiGian.Hour, item.ThoiGian.Minute, item.TagName })
                .Select(g => g.OrderByDescending(item => item.ThoiGian).First())
                .ToList();
            var uniqueTimes = rawData
                .Select(item => item.ThoiGian.Date.AddHours(item.ThoiGian.Hour).AddMinutes(item.ThoiGian.Minute))
                .Distinct()
                .OrderBy(times => times)
                .ToList();
            var result = uniqueTimes.Select(time =>
            {
                var group = rawData.Where(x => x.ThoiGian.Date == time.Date && x.ThoiGian.Hour == time.Hour);
                var row = new Dictionary<string, object>
                {
                    ["ThoiGian"] = time,
                    ["Lo 13"] = new Dictionary<string, object>(),
                    ["Lo 14"] = new Dictionary<string, object>(),
                    ["Lo 15"] = new Dictionary<string, object>(),
                    ["Lo 16"] = new Dictionary<string, object>()
                };

                var lo13 = (Dictionary<string, object>)row["Lo 13"];
                var lo14 = (Dictionary<string, object>)row["Lo 14"];
                var lo15 = (Dictionary<string, object>)row["Lo 15"];
                var lo16 = (Dictionary<string, object>)row["Lo 16"];
                foreach (var item in group)
                {
                    if (string.IsNullOrEmpty(item.TagName)) continue;

                    var cum = GetCum78(item.TagName);

                    switch (cum)
                    {
                        case "Lo 13":
                            lo13[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 14":
                            lo14[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 15":
                            lo15[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 16":
                            lo16[item.TagName] = item.GiaTri;
                            break;
                    }
                }
                return row;
            }).ToList();
            return result;
        }

        public IEnumerable<Dictionary<string, object>> GetSearchTimeLuyenCocCum910(DateTime? times)
        {
            if (times == null) throw new AppException(ErrorCode.VALIDATE_TIME_EXCEPTION);

            var fromTime = new DateTime(
                times.Value.Year,
                times.Value.Month,
                times.Value.Day,
                times.Value.Hour,
                times.Value.Minute,
                0
                );
            var toTime = fromTime.AddMinutes(1);

            var dataInRange = _context.luyenCocCum910s
                .Where(x => x.ThoiGian >= fromTime && x.ThoiGian < toTime)
                .ToList();
            var rawData = dataInRange
                .GroupBy(item => new { item.ThoiGian.Hour, item.ThoiGian.Minute, item.TagName })
                .Select(g => g.OrderByDescending(item => item.ThoiGian).First())
                .ToList();
            var uniqueTimes = rawData
                .Select(item => item.ThoiGian.Date.AddHours(item.ThoiGian.Hour).AddMinutes(item.ThoiGian.Minute))
                .Distinct()
                .OrderBy(times => times)
                .ToList();
            var result = uniqueTimes.Select(time =>
            {
                var group = rawData.Where(x => x.ThoiGian.Date == time.Date && x.ThoiGian.Hour == time.Hour);
                var row = new Dictionary<string, object>
                {
                    ["ThoiGian"] = time,
                    ["Lo 17"] = new Dictionary<string, object>(),
                    ["Lo 18"] = new Dictionary<string, object>(),
                    ["Lo 19"] = new Dictionary<string, object>(),
                    ["Lo 20"] = new Dictionary<string, object>()
                };

                var lo17 = (Dictionary<string, object>)row["Lo 17"];
                var lo18 = (Dictionary<string, object>)row["Lo 18"];
                var lo19 = (Dictionary<string, object>)row["Lo 19"];
                var lo20 = (Dictionary<string, object>)row["Lo 20"];
                foreach (var item in group)
                {
                    if (string.IsNullOrEmpty(item.TagName)) continue;

                    var cum = GetCum910(item.TagName);

                    switch (cum)
                    {
                        case "Lo 17":
                            lo17[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 18":
                            lo18[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 19":
                            lo19[item.TagName] = item.GiaTri;
                            break;
                        case "Lo 20":
                            lo20[item.TagName] = item.GiaTri;
                            break;
                    }
                }
                return row;
            }).ToList();
            return result;
        }
    }
}
