using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.VoiXiMang
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoVoiDungMotController : ControllerBase
    {
        private readonly IVoiXiMangRepository _service;
        public LoVoiDungMotController(IVoiXiMangRepository service)
        {
            _service = service;
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLoVoiDungMots()
        {
            var result = await _service.GetLast24HoursLoVoiDungMots();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLoVoiDungMots([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _service.GetSearchTimeLoVoiDungMots(from, to);
            return Ok(result);
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LoVoiDung1.xlsx");
            var excelBytes = await _service.ExportLoVoiDungMots(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LoVoiDung1_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
