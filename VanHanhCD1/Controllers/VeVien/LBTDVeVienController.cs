using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Models.VeVien;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.VeVien
{
    [Route("api/[controller]")]
    [ApiController]
    public class LBTDVeVienController : ControllerBase
    {
        private readonly IVeVienRepository _veVienService;
        public LBTDVeVienController(IVeVienRepository veVienService)
        {
            _veVienService = veVienService;
        }

       
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLocBuiTinhDien()
        {
            var result = await _veVienService.GetLast24HoursLocBuiTinhDien();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLocBuiTinhDien([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _veVienService.GetSearchTimeLocBuiTinhDien(from, to);
            return Ok(result);

        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LBTDVeVien.xlsx");
            var excelBytes = await _veVienService.ExportLocBuiTinhDien(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LocBuiTinhDien_VeVien_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
