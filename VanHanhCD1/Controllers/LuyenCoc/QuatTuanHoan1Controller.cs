using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.LuyenCoc
{

    [Route("api/[controller]")]
    [ApiController]
    public class QuatTuanHoan1Controller : ControllerBase
    {
        private readonly ILuyenCocRepository _service;
        public QuatTuanHoan1Controller(ILuyenCocRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLuyenCocQuatTuanHoan1s()
        {
            var result = await _service.GetLast24HoursLuyenCocQuatTuanHoan1s();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeGetLast24HoursLuyenCocQuatTuanHoan1s([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeLuyenCocQuatTuanHoan1s(from, to);
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _service.GetLuyenCocLuyenCocQuatTuanHoan1MinValues();
            return result;
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LUYENCOC_QGTH1.xlsx");
            var excelBytes = await _service.ExportLuyenCocQuatTuanHoan1s(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LUYENCOC_QGTH1_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
