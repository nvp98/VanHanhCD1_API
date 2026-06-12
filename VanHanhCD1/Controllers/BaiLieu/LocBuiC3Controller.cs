using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.BaiLieu
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocBuiC3Controller : ControllerBase
    {
        private readonly IBaiLieuRepository _service;
        public LocBuiC3Controller(IBaiLieuRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLocBuiC3s()
        {
            var result = await _service.GetLast24HoursDongCoLocBuiC3s();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeLocBuiC1s([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeDongCoLocBuiC3s(from, to);
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _service.GetDongCoLocBuiC3MinValues();
            return result;
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_BAILIEU_C3.xlsx");
            var excelBytes = await _service.ExportDongCoLocBuiC3s(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_BAILIEU_C3_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
