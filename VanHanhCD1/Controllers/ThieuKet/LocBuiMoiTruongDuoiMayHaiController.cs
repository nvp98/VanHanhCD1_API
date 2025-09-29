using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.ThieuKet
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocBuiMoiTruongDuoiMayHaiController : ControllerBase
    {
        private readonly IThieuKetRepository _service;
        public LocBuiMoiTruongDuoiMayHaiController(IThieuKetRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLocBuiMoiTruongDuoiMayHais()
        {
            var result = await _service.GetLast24HoursLocBuiMoiTruongDuoiMayHais();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLocBuiMoiTruongDuoiMayHais([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeLocBuiMoiTruongDuoiMayHais(from, to);
            return Ok(result);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_DuoiMay2.xlsx");
            var excelBytes = await _service.ExportLocBuiMoiTruongDuoiMayHais(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_DuoiMayHai_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
