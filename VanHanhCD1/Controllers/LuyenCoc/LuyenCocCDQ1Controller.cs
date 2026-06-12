using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.LuyenCoc
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuyenCocCDQ1Controller : ControllerBase
    {
        private readonly ILuyenCocRepository _service;
        public LuyenCocCDQ1Controller(ILuyenCocRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLuyenCocCDQ1s()
        {
            var result = await _service.GetLast24HoursLuyenCocCDQ1s();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeGetLast24HoursLuyenCocCDQ1s([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeLuyenCocCDQ1s(from, to);
            return Ok(result);
        }
        
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LUYENCOC_CDQ1.xlsx");
            var excelBytes = await _service.ExportLuyenCocCDQ1s(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_CDQ1_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
