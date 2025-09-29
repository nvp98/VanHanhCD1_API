using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.VoiXiMang
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoVoiQuayController : ControllerBase
    {
        private readonly IVoiXiMangRepository _service;
        public LoVoiQuayController(IVoiXiMangRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLoVoiQuays()
        {
            var result = await _service.GetLast24HoursLoVoiQuay();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLoVoiQuays([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeLoVoiQuay(from, to);
            return Ok(result);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LoVoiQuay.xlsx");
            var excelBytes = await _service.ExportLoVoiQuay(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LoVoiQuay_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
