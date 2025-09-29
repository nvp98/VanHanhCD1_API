using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.ThieuKet
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocBuiMoiTruongMangQuangController : ControllerBase
    {
        private readonly IThieuKetRepository _service;
        public LocBuiMoiTruongMangQuangController(IThieuKetRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLocBuiMoiTruongMangQuangs()
        {
            var result = await _service.GetLast24HoursLocBuiMoiTruongMangQuangs();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLocBuiMoiTruongMangQuangs([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeLocBuiMoiTruogMangQuangs(from, to);
            return Ok(result);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_MangQuang.xlsx");
            var excelBytes = await _service.ExportLocBuiMoiTruongMangQuangs(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_MangQuang_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
