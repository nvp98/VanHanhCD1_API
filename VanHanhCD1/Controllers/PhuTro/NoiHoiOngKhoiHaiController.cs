using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.PhuTro
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoiHoiOngKhoiHaiController : ControllerBase
    {
        private readonly IPhuTroRepository _service;
        public NoiHoiOngKhoiHaiController(IPhuTroRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursNoiHoiOngKhoiQuatGioLamMatVongHais()
        {
            var result = await _service.GetLast24HoursNoiHoiOngKhoiQuatGioLamMatVongHais();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeNoiHoiMatVongHais([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeNoiHoiOngKhoiQuatGioLamMatVongHais(from, to);
            return Ok(result);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_NH_OK_QGLTV2.xlsx");
            var excelBytes = await _service.ExportNoiHoiOngKhoiQuatGioLamMatVongHais(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_QGLTV1_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
