using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.VeVien
{
    [Route("api/[controller]")]
    [ApiController]
    public class LBMTMangQuangVeVienController : ControllerBase
    {
        private readonly IVeVienRepository _veVienService;
        public LBMTMangQuangVeVienController(IVeVienRepository veVienService)
        {
            _veVienService = veVienService;
        }

        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLocBuiMoiTruongMangThanhPhamTrungChuyen23()
        {
            var result = await _veVienService.GetLast24HoursLocBuiMoiTruongMangThanhPhamTrungChuyen23();
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _veVienService.GetLocBuiMoiTruongMangQuangThanhPhamTrungChuyen23MinValues();
            return result;
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLocBuiMoiTruongMangThanhPhamTrungChuyen23([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _veVienService.GetSearchTimeLocBuiMoiTruongMangThanhPhamTrungChuyen23(from, to);
            return Ok(result);

        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LB_MQTP_TTCVeVien.xlsx");
            var excelBytes = await _veVienService.ExportLocBuiMoiTruongMangThanhPhamTrungChuyen23(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LocBuiMoiTruongMangThanhPhamTrungChuyen23_VeVien_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
