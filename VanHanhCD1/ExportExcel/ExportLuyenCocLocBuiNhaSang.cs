using ClosedXML.Excel;

namespace VanHanhCD1.ExportExcel
{
    public class ExportLuyenCocLocBuiNhaSang
    {
        private const string TemplateSheetNameOne = "Sheet1";
        private const string TemplateSheetNameTwo = "Sheet2";
        private const int DefaultSymbolColumn = 3; //column D
        private const int DefaultSymbolRow = 6;
        private const int HoursInBlock = 24;
        private const int DataStartColumn = 4;
        private readonly ILogger<ExportLuyenCocLocBuiNhaSang> _logger;

        public ExportLuyenCocLocBuiNhaSang(ILogger<ExportLuyenCocLocBuiNhaSang> logger)
        {
            _logger = logger;
        }

        public byte[] GenerateExcelFile<T>(
           List<T> data,
           string templatePath,
           DateTime from,
           DateTime to,
           Func<T, DateTime> dateSelector,
           Func<T, string> symbolSelector,
           Func<T, double?> valueSelector,
           Func<T, string> tagNameSelector,
           int symbolColumn = DefaultSymbolColumn,
           int symbolRow = DefaultSymbolRow)
        {


            if (!File.Exists(templatePath))
            {
                _logger.LogError("Template file not found at {TemplatePath}", templatePath);
                throw new FileNotFoundException("File not found", templatePath);
            }

            using var templateWorkbook = new XLWorkbook(templatePath);
            var sheet1 = templateWorkbook.Worksheet(TemplateSheetNameOne);
            var sheet2 = templateWorkbook.Worksheet(TemplateSheetNameTwo);


            var rowSymbolMap = CreateRowSymbolMap(sheet2, symbolColumn, symbolRow);
            if (rowSymbolMap.Count == 0)
            {
                _logger.LogWarning("No symbols found in the template sheet at column {SymbolColumn}, row {SymbolRow}", symbolColumn, symbolRow);
                throw new InvalidOperationException("No symbols found in the template sheet.");
            }

            var dataIndex = BuildDataIndex(data, dateSelector, symbolSelector, tagNameSelector);

            // Xác định các khối thời gian
            var blocks = GetDateBlocks(from, to);
            if (blocks.Count == 0)
            {
                _logger.LogWarning("Invalid date range: from date is after to date.");
            }

            int blockIndex = 0;
            foreach (var block in blocks)
            {
                var blockEnd = block.AddHours(23);
                var sheetName = $"Sheet{blockIndex + 2}";
                var sheet = blockIndex == 0 ? sheet2 : sheet2.CopyTo(sheetName);

                string dayFrom = block.Day.ToString("00");
                string dayTo = block.AddDays(1).Day.ToString("00");
                string monthYear = block.ToString("MM-yyyy");

                var sheetOne = sheet1.CopyTo($"Ngày {dayFrom} đến {dayTo}-{monthYear}");

                UpdateTitles(sheetOne, block, dayFrom, dayTo);

                WriteHourlyData(sheet, block, rowSymbolMap, dataIndex, valueSelector, DataStartColumn);

                UpdateFormulaSheet(sheetOne, TemplateSheetNameTwo, sheetName);

                blockIndex++;
            }

            //var json = System.Text.Json.JsonSerializer.Serialize(rowSymbolMap);
            //_logger.LogInformation("RowSymbolMap: {Map}", json);
            HideOriginalSheets(templateWorkbook);

            using var stream = new MemoryStream();
            templateWorkbook.SaveAs(stream);
            stream.Position = 0;
            return stream.ToArray();

        }

        // Tạo ánh xạ symbol
        private static Dictionary<string, int> CreateRowSymbolMap(IXLWorksheet sheet,
            int symbolColumn, int symbolRow)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var lastRowUsed = sheet.LastRowUsed();
            if (lastRowUsed == null) return map;
            var lastRow = lastRowUsed.RowNumber();
            for (int row = symbolRow; row <= lastRow; row++)
            {
                var symbol = sheet.Cell(row, symbolColumn).GetValue<string>()?.Trim();
                if (!string.IsNullOrEmpty(symbol))
                {
                    map[symbol] = row;
                }
            }
            return map;
        }

        //Chỉ mục dữ liệu
        private static Dictionary<(DateTime Date, int Hour, string Symbol), T> BuildDataIndex<T>(
        List<T> data,
        Func<T, DateTime> dateSelector,
        Func<T, string> symbolSelector,
        Func<T, string> tagNameSelector)
        {
            return data.ToDictionary(x => (dateSelector(x).Date, dateSelector(x).Hour, tagNameSelector(x).Trim()), x => x);
        }

        // Lấy danh sách khối thời gian
        private static List<DateTime> GetDateBlocks(DateTime from, DateTime to)
        {
            var blocks = new List<DateTime>();
            var blockStart = new DateTime(from.Year, from.Month, from.Day, 8, 0, 0);
            if (from.Hour <= 8) blockStart = blockStart.AddDays(-1);
            var blockEnd = new DateTime(to.Year, to.Month, to.Day, 8, 0, 0);
            if (to.Hour >= 8) blockEnd = blockEnd.AddDays(1);

            while (blockStart <= to && blockStart < blockEnd)
            {
                var blockFinish = blockStart.AddHours(23);

                // Chỉ thêm block nếu có bất kỳ giờ nào giao với [from, to]
                if (!(blockFinish < from || blockStart > to))
                {
                    blocks.Add(blockStart);
                }

                blockStart = blockStart.AddDays(1);
            }
            return blocks;
        }

        private static void WriteHourlyData<T>(
            IXLWorksheet sheet, DateTime blockStart, Dictionary<string, int> rowSymbolMap,
            Dictionary<(DateTime dateTime, int hour, string symbol), T> dataIndex,
            Func<T, double?> valueSelector, int dateStartColumn
            )
        {

            for (int i = 0; i < HoursInBlock; i++)
            {
                var currentHour = blockStart.AddHours(i);
                int hour = currentHour.Hour;
                int col = dateStartColumn + ((hour - 8 + 24) % 24);
                sheet.Cell(5, col).Value = hour;

                foreach (var index in rowSymbolMap.Keys)
                {
                    int row = rowSymbolMap[index];
                    var key = (currentHour.Date, currentHour.Hour, index);
                    if (dataIndex.TryGetValue(key, out var record))
                    {
                        sheet.Cell(row, col).Value = valueSelector(record)?.ToString() ?? "-";
                    }
                    else
                    {
                        sheet.Cell(row, col).Value = "-";
                    }
                }
            }
        }

        //Cập nhật lại tiêu đề
        private static void UpdateTitles(
            IXLWorksheet sheet, DateTime blockStart,
            string dayFrom, string dayTo)
        {
            //var dateTimeTitle = $"Từ {dayFrom} đến {dayTo} tháng {blockStart:MM} năm {blockStart:yyyy}";
            var dayFromTimeTitle = $"8h đến 19h ngày {dayFrom} tháng {blockStart:MM} năm {blockStart:yyyy}";
            var dayToTimeTitle = $"20h ngày {dayFrom} đến 7h ngày {dayTo} tháng {blockStart:MM} năm {blockStart:yyyy}";
            //// var title = "NHẬT KÝ VẬN HÀNH VÊ VIÊN";
            //var subtitle = $"{dateTimeTitle}";
            var subtitleFrom = $"{dayFromTimeTitle}";
            var subtitleTo = $"{dayToTimeTitle}";

            //var cellSheet = sheet.Cell("S4");
            var cellSheetFrom = sheet.Cell("B7");
            var cellSheetTo = sheet.Cell("N7");

            //cellSheet.Clear(XLClearOptions.Contents);
            cellSheetFrom.Clear(XLClearOptions.Contents);
            cellSheetTo.Clear(XLClearOptions.Contents);
            //var richTextNgay = cellSheet.GetRichText();
            var richTextFrom = cellSheetFrom.GetRichText();
            var richTextTo = cellSheetTo.GetRichText();
            ////richTextNgay.AddText(title).SetFontSize(18).SetBold().SetItalic(false);
            //richTextNgay.AddText(subtitle).SetFontSize(10).SetItalic().SetBold(false);
            richTextFrom.AddText(subtitleFrom).SetFontSize(10).SetBold().SetItalic(false);
            richTextTo.AddText(subtitleTo).SetFontSize(10).SetBold().SetItalic(false);

        }

        // Cập nhật  công thức
        private static void UpdateFormulaSheet(IXLWorksheet sheet, string oldSheetName, string newSheetName)
        {
            var cellsWithFormula = sheet.CellsUsed(c => c.HasFormula);
            foreach (var cell in cellsWithFormula)
            {
                string formula = cell.FormulaA1;
                if (formula.Contains(oldSheetName + "!", StringComparison.OrdinalIgnoreCase))
                {
                    cell.FormulaA1 = formula.Replace(oldSheetName + "!", newSheetName + "!", StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        // Ẩn sheet gõc
        private static void HideOriginalSheets(XLWorkbook workbook)
        {
            foreach (var ws in workbook.Worksheets)
            {
                if (ws.Name.StartsWith("Sheet", StringComparison.OrdinalIgnoreCase) ||
                   ws.Name.Equals(TemplateSheetNameOne, StringComparison.OrdinalIgnoreCase))
                {
                    ws.Visibility = XLWorksheetVisibility.VeryHidden;

                }
            }
        }
    }
}
