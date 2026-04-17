
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanHanhCD1.DTOs.Request;
using VanHanhCD1.Models;

namespace VanHanhCD1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarningHistoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        public WarningHistoryController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("warning")]
        public async Task<IActionResult> GetWarningHistories()
        {
            var warningHistories = await _context.warningHistories
                .Select(w => new
                {
                    thoiGian = w.ThoiGian,
                    tagName = w.TagName,
                    khuVuc = w.KhuVuc,
                    tenThongSo = w.TenThongSo,  
                    donVi = w.DonVi,
                    giaTri = w.GiaTri,
                    trangThai = w.TrangThai,
                })
                .OrderByDescending(w => w.thoiGian)
                .ToListAsync();
            return Ok(warningHistories);
        }


        [HttpPost]
        public async Task<IActionResult> CreateWarningHistory([FromBody] CreateWarningHistory request)
        {
            var warningHistory = new WarningHistory
            {
                ThoiGian = request.ThoiGian,
                TagName = request.TagName,
                KhuVuc = request.KhuVuc,
                TenThongSo = request.TenThongSo,
                GiaTri = request.GiaTri,
                TrangThai = request.TrangThai,
                DonVi = request.DonVi
            };

            try
            {
                _context.Add(warningHistory);
                await _context.SaveChangesAsync();
                return Ok("Thanh Cong");
            }
            catch (DbUpdateException ex)
            {
                // 🔥 Bị trùng key (duplicate)
                return Conflict(new {message= "Da ton tai" }); 
            }
        }
    }
}
