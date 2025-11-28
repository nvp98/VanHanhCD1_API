using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.LuyenCoc
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuatTuanHoan2Controller : ControllerBase
    {
        private readonly ILuyenCocRepository _service;
        public QuatTuanHoan2Controller(ILuyenCocRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLuyenCocQuatTuanHoan2s()
        {
            var result = await _service.GetLast24HoursLuyenCocQuatTuanHoan2s();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeGetLast24HoursLuyenCocQuatTuanHoan2s([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeLuyenCocQuatTuanHoan2s(from, to);
            return Ok(result);
        }
        //[HttpGet("export")]
        //public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        //{
        //    if (from >= to)
        //        return BadRequest("Thời gian không hợp lệ");
        //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_KhuKhiKhoiThieuKet2.xlsx");
        //    var excelBytes = await _service.ExportKhuKhiKhoiThieuKetHais(from, to, path);
        //    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_KhuKhiKhoiThieuKet2_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        //}
    }
}
