
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

        [HttpGet("search")]
        public async Task<IActionResult> SearchWarningHistories(
                [FromQuery] string category,
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 30)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                return BadRequest("Category is required.");
            }

            // Query cơ bản
            var search = _context.warningHistories
                .AsNoTracking()
                .Where(w => w.Xuong == category);

            // Tổng số bản ghi theo category
            var totalRecords = await search.CountAsync();

            if (totalRecords == 0)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy lịch sử cảnh báo."
                });
            }

            // Lấy dữ liệu phân trang
            var warningHistories = await search
                .OrderByDescending(w => w.ThoiGian)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new
                {
                    thoiGian = w.ThoiGian,
                    tagName = w.TagName,
                    khuVuc = w.KhuVuc,
                    tenThongSo = w.TenThongSo,
                    donVi = w.DonVi,
                    giaTri = w.GiaTri,
                    trangThai = w.TrangThai,
                    xuong = w.Xuong
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return Ok(new
            {
                data = warningHistories,
                pagination = new
                {
                    pageNumber,
                    pageSize,
                    totalRecords,
                    totalPages
                }
            });
        }

        [HttpGet("pagination")]
        public async Task<IActionResult> PaginationWarningHistories(
                [FromQuery] int pageNumber = 1,
                [FromQuery] int pageSize = 30)
        {

            var totalRecords = await _context.warningHistories.AsNoTracking().CountAsync();
            if (totalRecords == 0)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy lịch sử cảnh báo."
                });
            }
            // Lấy dữ liệu phân trang
            var warningHistories = await _context.warningHistories
                .OrderByDescending(w => w.ThoiGian)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(w => new
                {
                    thoiGian = w.ThoiGian,
                    tagName = w.TagName,
                    khuVuc = w.KhuVuc,
                    tenThongSo = w.TenThongSo,
                    donVi = w.DonVi,
                    giaTri = w.GiaTri,
                    trangThai = w.TrangThai,
                    xuong = w.Xuong ?? "",
                })
                .ToListAsync();
           
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return Ok(new
            {
                data = warningHistories,
                pagination = new
                {
                    pageNumber,
                    pageSize,
                    totalRecords,
                    totalPages
                }
            });
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
                DonVi = request.DonVi,
                Xuong = request.Xuong
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
