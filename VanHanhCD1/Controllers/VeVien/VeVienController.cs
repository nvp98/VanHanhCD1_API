using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanHanhCD1.Models;
using ClosedXML.Excel;

namespace VanHanhCD1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeVienController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VeVienController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursData()
        {
            // 1. Tìm thời điểm mới nhất trong bảng
            var latestTime = await _context.VH_LoVeVien
                .OrderByDescending(x => x.ThoiGian)
                .Select(x => x.ThoiGian)
                .FirstOrDefaultAsync();

            if (latestTime == default)
                return Ok(new List<Dictionary<string, object>>()); // Không có dữ liệu

            // 2. Tạo danh sách 24 mốc giờ từ latestTime đổ ngược
            var expectedTimes = Enumerable.Range(0, 24)
                .Select(i => latestTime.AddHours(-i).Date.AddHours(latestTime.AddHours(-i).Hour))
                .OrderBy(t => t)
                .ToList();

            // 3. Lấy toàn bộ bản ghi nằm trong các mốc giờ cần
            var allData = await _context.VH_LoVeVien
                .Where(x => expectedTimes.Any(t => x.ThoiGian.Date == t.Date && x.ThoiGian.Hour == t.Hour))
                .ToListAsync();

            // 4. Nhóm theo thời gian + tag, lấy bản ghi cuối cùng
            var rawData = allData
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            // 5. Gom về 1 dictionary theo từng mốc giờ
            var result = expectedTimes.Select(time =>
            {
                var group = rawData.Where(x => x.ThoiGian.Date == time.Date && x.ThoiGian.Hour == time.Hour);
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

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");

            var dataInRange = await _context.VH_LoVeVien
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();

            var rawData = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var uniqueTimes = rawData
                .Select(x => x.ThoiGian.Date.AddHours(x.ThoiGian.Hour))
                .Distinct()
                .OrderBy(t => t)
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

            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var dataInRange = await _context.VH_LoVeVien
                .Where(x => x.ThoiGian >= from && x.ThoiGian <= to)
                .ToListAsync();

            var grouped = dataInRange
                .GroupBy(x => new { x.ThoiGian.Date, x.ThoiGian.Hour, x.TagName })
                .Select(g => g.OrderByDescending(x => x.ThoiGian).First())
                .ToList();

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_VeVien.xlsx");
            if (!System.IO.File.Exists(templatePath))
                return NotFound("Không tìm thấy file mẫu BMVH_VeVien.xlsx");

            using var templateWorkbook = new XLWorkbook(templatePath);

            var sheet1 = templateWorkbook.Worksheet("Sheet1");
            var caNgay = templateWorkbook.Worksheet("Ca ngay");
            var caDem = templateWorkbook.Worksheet("Ca dem");

            // Map ký hiệu (col E) -> dòng từ hàng 6 trở đi
            var rowSymbolMap = new Dictionary<string, int>();
            for (int row = 6; row <= sheet1.LastRowUsed().RowNumber(); row++)
            {
                var symbol = sheet1.Cell(row, 5).GetValue<string>()?.Trim();
                if (!string.IsNullOrEmpty(symbol))
                {
                    rowSymbolMap[symbol] = row;
                }
            }

            var blockStart = new DateTime(from.Year, from.Month, from.Day, 8, 0, 0);
            if (from.Hour < 8) blockStart = blockStart.AddDays(-1);

            var blockEndLimit = new DateTime(to.Year, to.Month, to.Day, 7, 0, 0);
            if (to.Hour >= 8) blockEndLimit = blockEndLimit.AddDays(1);

            int blockIndex = 0;

            while (blockStart < blockEndLimit)
            {
                var blockEnd = blockStart.AddHours(23);
                var sheetName = $"Sheet{blockIndex + 1}";
                var sheet = blockIndex == 0 ? sheet1 : sheet1.CopyTo(sheetName);

                // Copy Ca ngay và Ca dem tương ứng
                string dayFrom = blockStart.Day.ToString("00");
                string dayTo = blockStart.AddDays(1).Day.ToString("00");
                string monthYear = blockStart.ToString("MM-yyyy");

                var caNgaySheet = caNgay.CopyTo($"Ca ngay {dayFrom} đến {dayTo}-{monthYear}");
                var caDemSheet = caDem.CopyTo($"Ca dem {dayFrom} đến {dayTo}-{monthYear}");
                // Chuỗi tiêu đề
                var fromStr = $"Từ {dayFrom} đến {dayTo} ngày {dayTo} tháng {blockStart:MM} năm {blockStart:yyyy}";
                var title = "NHẬT KÝ VẬN HÀNH LÒ VÊ VIÊN";
                var subtitle = $"Kíp…..…..{fromStr}";

                // Ca ngày
                var cellNgay = caNgaySheet.Cell("F2");
                cellNgay.Clear(XLClearOptions.Contents);
                var richNgay = cellNgay.GetRichText();
                richNgay.AddText(title).SetFontSize(18).SetBold().SetItalic(false);
                richNgay.AddText("\n" + subtitle).SetFontSize(16).SetItalic().SetBold(false);

                // Ca đêm
                var cellDem = caDemSheet.Cell("F2");
                cellDem.Clear(XLClearOptions.Contents);
                var richDem = cellDem.GetRichText();
                richDem.AddText(title).SetFontSize(18).SetBold().SetItalic(false);
                richDem.AddText("\n" + subtitle).SetFontSize(16).SetItalic().SetBold(false);



                // Ghi từng giờ trong block
                for (int i = 0; i < 24; i++)
                {
                    var currentHour = blockStart.AddHours(i);
                    int hour = currentHour.Hour;
                    int col = 7 + ((hour - 8 + 24) % 24);

                    // Ghi giờ hàng 5
                    sheet.Cell(5, col).Value = $"{hour}h";

                    foreach (var symbol in rowSymbolMap.Keys)
                    {
                        int row = rowSymbolMap[symbol];
                        var record = grouped.FirstOrDefault(x =>
                            x.ThoiGian.Date == currentHour.Date &&
                            x.ThoiGian.Hour == currentHour.Hour &&
                            x.TagName.Trim() == symbol);

                        sheet.Cell(row, col).Value = record != null ? record.GiaTri : "-";
                    }
                }

                // Cập nhật công thức trong các sheet Ca ngay / Ca dem
                void UpdateFormulaSheet(IXLWorksheet ws)
                {
                    foreach (var cell in ws.CellsUsed())
                    {
                        if (cell.HasFormula)
                        {
                            string formula = cell.FormulaA1;
                            if (formula.Contains("Sheet1!", StringComparison.OrdinalIgnoreCase))
                            {
                                cell.FormulaA1 = formula.Replace("Sheet1!", $"{sheetName}!");
                            }
                        }
                    }
                }

                UpdateFormulaSheet(caNgaySheet);
                UpdateFormulaSheet(caDemSheet);

                blockStart = blockStart.AddDays(1);
                blockIndex++;
            }
            // Ẩn sheet gốc
            foreach (var ws in templateWorkbook.Worksheets)
            {
                if (ws.Name.StartsWith("Sheet"))
                {
                    ws.Hide();
                }
            }
            sheet1.Hide();
            sheet1.Visibility = XLWorksheetVisibility.VeryHidden;
            caNgay.Visibility = XLWorksheetVisibility.VeryHidden;
            caDem.Visibility = XLWorksheetVisibility.VeryHidden;

            using var stream = new MemoryStream();
            templateWorkbook.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"LoVeVien_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }

}
