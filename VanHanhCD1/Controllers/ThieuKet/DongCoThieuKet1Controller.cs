using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.ThieuKet
{
    [Route("api/[controller]")]
    [ApiController]
    public class DongCoThieuKet1Controller : ControllerBase
    {
        private readonly IThieuKetRepository _service;
        public DongCoThieuKet1Controller(IThieuKetRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursDongCoThieuKet1s()
        {
            var result = await _service.GetLast24HoursDongCoThieuKet1s();
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _service.GetDongCoThieuKet1MinValues();
            return result;
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeDongCoThieuKet1s([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeDongCoThieuKet1s(from, to);
            return Ok(result);
        }
        //[HttpGet("export")]
        //public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        //{
        //    if (from >= to)
        //        return BadRequest("Thời gian không hợp lệ");
        //    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_DuoiMay2.xlsx");
        //    var excelBytes = await _service.ExportLocBuiMoiTruongDuoiMayHais(from, to, path);
        //    return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_DuoiMayHai_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        //}
    }
}
