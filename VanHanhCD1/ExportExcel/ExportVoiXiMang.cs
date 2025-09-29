using ClosedXML.Excel;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Finance.Implementations;

namespace VanHanhCD1.ExportExcel
{
    public class ExportVoiXiMang
    {
        private const string TemplateSheetNameOne = "Sheet1";
        private const string TemplateSheetNameTwo = "Sheet2";
        private const string TemplateSheetNameCaNgay = "Ca ngay";
        private const string TemplateSheetNameCaDem = "Ca Dem";
        private const int DefaultSymbolColumn = 4;
        private const int DefaultSymbolRow = 6;
        private const int HoursInBlock = 24;
        private const int DataStartColumn = 5;
        private readonly ILogger<ExportVoiXiMang> _logger;
        
        public ExportVoiXiMang(ILogger<ExportVoiXiMang> logger)
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
            var sheetCaNgay = templateWorkbook.Worksheet(TemplateSheetNameCaNgay);
            var sheetCaDem = templateWorkbook.Worksheet(TemplateSheetNameCaDem);

            var rowSymbolMap = CreateRowSymbolMap(sheet2, symbolColumn, symbolRow);
            if (rowSymbolMap.Count == 0) 
            {
                _logger.LogWarning("No symbols found in the template sheet at column {SymbolColumn}, row {SymbolRow}", symbolColumn, symbolRow);
                throw new InvalidOperationException("No symbols found in the template sheet.");
            }

            var dataIndex = BuildDataIndex(data, dateSelector, symbolSelector, tagNameSelector);

            var blocks = GetDateBlocks(from, to);
            if (blocks.Count == 0) 
            {
                _logger.LogWarning("Invalid date range: from date is after to date. ");
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
                var sheetNgay = sheetCaNgay.CopyTo($"Ca ngay {dayFrom} đến {dayTo}-{monthYear}");
                var sheetDem = sheetCaDem.CopyTo($"Ca dem {dayFrom} đến {dayTo}-{monthYear}");

                UpdateTitles(sheetOne, sheetNgay, sheetDem, block, dayFrom, dayTo);
                WriteHourlyData(sheet, block, rowSymbolMap, dataIndex, valueSelector, DataStartColumn);

                UpdateFormulaSheet(sheetOne, TemplateSheetNameTwo, sheetName);
                UpdateFormulaSheet(sheetCaNgay, TemplateSheetNameOne, sheetName);
                UpdateFormulaSheet(sheetCaDem, TemplateSheetNameOne, sheetName);

                blockIndex++;
            }

            HideOriginalSheets(templateWorkbook);

            using var stream = new MemoryStream();
            templateWorkbook.SaveAs(stream);
            stream.Position = 0;
            return stream.ToArray();

        }

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

        private static Dictionary<(DateTime Date, int Hour, string Symbol), T> BuildDataIndex<T>(
            List<T> data,
            Func<T, DateTime> dateSelector,
            Func<T, string> symbolSelector,
            Func<T, string> tagNameSelector)
        {
            return data.ToDictionary(x => (dateSelector(x).Date, dateSelector(x).Hour,
            tagNameSelector(x).Trim()), x => x);
        }

        private static List<DateTime> GetDateBlocks(DateTime from, DateTime to)
        {
            var blocks = new List<DateTime>();
            var blockStart = new DateTime(from.Year, from.Month, from.Day, 8, 0, 0);
            if (from.Hour < 8) blockStart = blockStart.AddDays(-1);
            var blockEnd = new DateTime(to.Year, to.Month, to.Day, 8, 0, 0);

            while(blockStart <= to && blockStart < blockEnd)
            {
                blocks.Add(blockStart);
                blockStart = blockStart.AddDays(1);
            }
            return blocks;
        }

        private static void WriteHourlyData<T>(
            IXLWorksheet sheet, DateTime blockStart,
            Dictionary<string, int> rowSymbolMap, 
            Dictionary<(DateTime dateTime, int hour, string symbol), T> dataIndex,
            Func<T, double?> valueSelector, int dateStartColumn)
        {
            for(int i = 0; i < HoursInBlock; i++)
            {
                var currentHour = blockStart.AddHours(i);
                int hour = currentHour.Hour;
                int col = dateStartColumn + ((hour - 8 + 24) % 24);
                sheet.Cell(5, col).Value = hour;
                foreach(var index in rowSymbolMap.Keys)
                {
                    int row = rowSymbolMap[index];
                    var key = (currentHour.Date, currentHour.Hour, index);
                    if(dataIndex.TryGetValue(key, out var record))
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

        private static void UpdateTitles(
            IXLWorksheet sheet, IXLWorksheet caNgaySheet,
            IXLWorksheet caDemSheet,DateTime blockStart,
            string dayFrom, string dayTo)
        {
            var dateTimeTitle = $"Từ {dayFrom} đến {dayTo} ngày {dayTo} thánh {blockStart:MM} năm {blockStart:yyyy}";
            // var title = "NHẬT KÝ VẬN HÀNH VÊ VIÊN";
            var subtitle = $"{dateTimeTitle}";

            var cellSheet = sheet.Cell("S4");
            cellSheet.Clear(XLClearOptions.Contents);
            var richTextNgay = cellSheet.GetRichText();
            //richTextNgay.AddText(title).SetFontSize(18).SetBold().SetItalic(false);
            richTextNgay.AddText(subtitle).SetFontSize(13).SetItalic().SetBold(false);

            var cellCaNgay = caNgaySheet.Cell("S4");
            cellCaNgay.Clear(XLClearOptions.Contents);
            var richNgay = cellCaNgay.GetRichText();
            //richTextNgay.AddText(title).SetFontSize(18).SetBold().SetItalic(false);
            richNgay.AddText(subtitle).SetFontSize(13).SetItalic().SetBold(false);

            var cellCaDem = caDemSheet.Cell("S4");
            cellCaNgay.Clear(XLClearOptions.Contents);
            var richDem = cellCaDem.GetRichText();
            //richTextNgay.AddText(title).SetFontSize(18).SetBold().SetItalic(false);
            richDem.AddText(subtitle).SetFontSize(13).SetItalic().SetBold(false);
        }

        private static void UpdateFormulaSheet(IXLWorksheet sheet, string oldSheetName,
            string newSheetName)
        {
            var cellsWithFormula = sheet.CellsUsed(c => c.HasFormula);
            foreach (var cell in cellsWithFormula)
            {
                string formula = cell.FormulaA1;
                if (formula.Contains(oldSheetName + "!", StringComparison.OrdinalIgnoreCase))
                {
                    cell.FormulaA1 = formula.Replace(oldSheetName + "!",
                        newSheetName + "!", StringComparison.OrdinalIgnoreCase);

                }
            }
        }

        private static void HideOriginalSheets(XLWorkbook workbook)
        {
            foreach (var ws in workbook.Worksheets)
            {
                if (ws.Name.StartsWith("Sheet", StringComparison.OrdinalIgnoreCase) ||
                    ws.Name.Equals(TemplateSheetNameOne, StringComparison.OrdinalIgnoreCase) ||
                    ws.Name.Equals(TemplateSheetNameCaNgay, StringComparison.OrdinalIgnoreCase) ||
                    ws.Name.Equals(TemplateSheetNameCaDem, StringComparison.OrdinalIgnoreCase)
                    )
                {
                    ws.Visibility = XLWorksheetVisibility.VeryHidden;
                }
            }
        }
    }
}
