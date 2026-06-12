using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.LuyenCoc
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocBuiNhaSang2Controller : ControllerBase
    {
        private readonly ILuyenCocRepository _service;
        public LocBuiNhaSang2Controller(ILuyenCocRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLocBuiNhaSang2s()
        {
            var result = await _service.GetLast24HoursLuyenCocLocBuiNhaSang2s();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeGetLast24HoursLuyenCocLocBuiNhaSang2s([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeLuyenCocLocBuiNhaSang2s(from, to);
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _service.GetLuyenCocLocBuiNhaSang2MinValues();
            return result;
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LUYENCOC_LB.xlsx");
            var excelBytes = await _service.ExportLuyenCocLocBuiNhaSang2s(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LUYENCOC_LB_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
