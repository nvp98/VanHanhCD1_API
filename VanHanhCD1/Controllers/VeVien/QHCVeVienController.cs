using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.VeVien
{
    [Route("api/[controller]")]
    [ApiController]
    public class QHCVeVienController : ControllerBase
    {
        private readonly IVeVienRepository _veVienService;
        public QHCVeVienController(IVeVienRepository veVienService)
        {
            _veVienService = veVienService;
        }

        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursQHCVeVien()
        {
            var result = await _veVienService.GetLast24HoursQuatHutChinhs();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeQHC([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _veVienService.GetSearchTimeQuatHutChinhs(from, to);
            return Ok(result);

        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_QHCVeVien.xlsx");
            var excelBytes = await _veVienService.ExportQuatHutChinhs(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_QuatHutChinh_VeVien_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
