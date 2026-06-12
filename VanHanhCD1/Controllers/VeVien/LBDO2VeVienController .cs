using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.VeVien
{
    [ApiController]
    [Route("api/[controller]")]
    public class LBDO2VeVienController : ControllerBase
    {
        private readonly IVeVienRepository _veVienService;
        public LBDO2VeVienController(IVeVienRepository veVienService) {
            _veVienService = veVienService;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLBDO2()
        {
            var result = await _veVienService.GetLast24HoursLocBuiDaOngHais();
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _veVienService.GetLocBuiDaOngHaiMinValues();
            return result;
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLBDO2([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _veVienService.GetSearchTimeLocBuiDaOngHais(from, to);
            return Ok(result);

        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LBOD2VeVien.xlsx");
            var excelBytes = await _veVienService.ExportLocBuiDaOngHais(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LBDO2_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
