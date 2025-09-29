using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.PhuTro
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuatGioBaController : ControllerBase
    {
        private readonly IPhuTroRepository _service;
        public QuatGioBaController(IPhuTroRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursQuatGioBaThieuKetHais()
        {
            var result = await _service.GetLast24HoursQuatGioBaThieuKetHais();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeQuatGioBaThieuKetHais([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeQuatGioBaThieuKetHais(from, to);
            return Ok(result);
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_QG3ThieuKet2.xlsx");
            var excelBytes = await _service.ExportQuatGioMotThieuKetMots(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_QG1ThieuKet1_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
