using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.PhuTro
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurbineBonController : ControllerBase
    {
        private readonly IPhuTroRepository _service;
        public TurbineBonController(IPhuTroRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursTurbineBonThieuKetHais()
        {
            var result = await _service.GetLast24HoursTurbineBonThieuKetHais();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeTurbineBonThieuKetHais([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeTurbineBonThieuKetHais(from, to);
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_TB4ThieuKet2.xlsx");
            var excelBytes = await _service.ExportTurbineBonThieuKetHais(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_QG2ThieuKet1_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
