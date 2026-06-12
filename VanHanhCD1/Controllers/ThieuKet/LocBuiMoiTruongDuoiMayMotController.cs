using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.ThieuKet
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocBuiMoiTruongDuoiMayMotController : ControllerBase
    {
        private readonly IThieuKetRepository _service;
        public LocBuiMoiTruongDuoiMayMotController(IThieuKetRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLocBuiMoiTruongDuoiMayMots()
        {
            var result = await _service.GetLast24HoursLocBuiMoiTruongDuoiMayMots();
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _service.GetLocBuiMoiTruongDuoiMayMotMinValues();
            return result;
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLocBuiMoiTruongDuoiMayMots([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeLocBuiMoiTruongDuoiMayMots(from, to);
            return Ok(result);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_DuoiMay1.xlsx");
            var excelBytes = await _service.ExportLocBuiMoiTruongDuoiMayMots(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LBMTDuoiMayMot_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
