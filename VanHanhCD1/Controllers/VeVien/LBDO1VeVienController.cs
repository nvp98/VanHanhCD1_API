using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Exceptions;
using VanHanhCD1.Models.VeVien;
using VanHanhCD1.Repository;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.VeVien
{
    [ApiController]
    [Route("api/[controller]")]
    public class LBDO1VeVienController : ControllerBase
    {
        private readonly IVeVienRepository _veVienService;
        public LBDO1VeVienController(IVeVienRepository veVienService) {
            _veVienService = veVienService;
        }
        [HttpGet]
        public IEnumerable<LBDO1VeVien> GetBDO1VeViens()
        {
            return _veVienService.GetLocBuiDaOngMots();
        }
        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLBDO1()
        {
            var result = await _veVienService.GetLast24HoursLocBuiDaOngMots();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLBDO1([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _veVienService.GetSearchTimeLocBuiDaOngMots(from, to);
            return Ok(result);

        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LBOD1VeVien.xlsx");
            var excelBytes = await _veVienService.ExportLocBuiDaOngMots(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LBDO1_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
