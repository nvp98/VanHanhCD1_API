using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VanHanhCD1.Repository.Interfaces;

namespace VanHanhCD1.Controllers.VeVien
{
    [Route("api/[controller]")]
    [ApiController]
    public class LBMTPLieuBLuocVeVienController : ControllerBase
    {
        private readonly IVeVienRepository _veVienService;
        public LBMTPLieuBLuocVeVienController(IVeVienRepository veVienService)
        {
            _veVienService = veVienService;
        }

        [HttpGet("last-24h")]
        public async Task<IActionResult> GetLast24HoursLocBuiMoiTruongPhoiLieuBanLuoc()
        {
            var result = await _veVienService.GetLast24HoursLocBuiMoiTruongPhoiLieuBanLuoc();
            return Ok(result);
        }
        [HttpGet("min-value")]
        public IEnumerable<object> GetMinValue()
        {
            var result = _veVienService.GetLocBuiMoiTruongPhoiLieuBanLuocMinValues();
            return result;
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByTimeRangeLocBuiMoiTruongPhoiLieuBanLuoc([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var result = await _veVienService.GetSearchTimeLocBuiMoiTruongPhoiLieuBanLuoc(from, to);
            return Ok(result);

        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportExcel([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from >= to)
                return BadRequest("Thời gian không hợp lệ");
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/BMVH_LBMT_PhoiLieu_BanLuoc_VeVien.xlsx");
            var excelBytes = await _veVienService.ExportLocBuiMoiTruongPhoiLieuBanLuoc(from, to, path);
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Report_LocBuiMoiTruong_VeVien_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
        }
    }
}
