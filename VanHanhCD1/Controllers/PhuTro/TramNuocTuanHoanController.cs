using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.PhuTro
{
    [Route("api/[controller]")]
    [ApiController]
    public class TramNuocTuanHoanController : ControllerBase
    {
        private readonly IPhuTroRepository _service;
        public TramNuocTuanHoanController(IPhuTroRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursTramNuocTuanHoans()
        {
            var result = await _service.GetLast24HoursTramNuocTuanHoans();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeGetLast24TramNuocTuanHoans([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeTramNuocTuanHoans(from, to);
            return Ok(result);

        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_TramNuocTuanHoan.xlsx");
            var excelBytes = await _service.ExportTramNuocTuanHoans(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_TramNuocTuanHoan_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
