using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.ThieuKet
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThieuKetMotController : ControllerBase
    {
        private readonly IThieuKetRepository _service;
        public ThieuKetMotController(IThieuKetRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursThieuKetMots()
        {
            var result = await _service.GetLast24HoursThieuKetMots();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeThieuKetMots([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeThieuKetMots(from, to);
            return Ok(result);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_ThieuKet1.xlsx");
            var excelBytes = await _service.ExportThieuKetMots(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_ThieuKet1_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
