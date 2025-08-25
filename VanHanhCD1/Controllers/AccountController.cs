using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanHanhCD1.Models;
using VanHanhCD1.DTOs;
using VanHanhCD1.Common;

namespace VanHanhCD1.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (string.IsNullOrWhiteSpace(model.NewPassword))
                return BadRequest("Mật khẩu mới không được để trống.");

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return Unauthorized("Không xác định được người dùng từ token.");

            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == username);
            if (user == null)
                return NotFound("Người dùng không tồn tại.");

            string hashedCurrent = Encryptor.MD5Hash(model.CurrentPassword);
            if (user.MatKhau != hashedCurrent)
                return BadRequest("Mật khẩu hiện tại không đúng.");

            string hashedNew = Encryptor.MD5Hash(model.NewPassword);
            if (hashedCurrent == hashedNew)
                return BadRequest("Mật khẩu mới phải khác mật khẩu hiện tại.");

            user.MatKhau = hashedNew;
            await _context.SaveChangesAsync();

            return Ok("Đổi mật khẩu thành công.");
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized("Không xác định được người dùng.");

            var user = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.TenDangNhap == username);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            // Xóa refresh token
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _context.SaveChangesAsync();

            return Ok("Đăng xuất thành công.");
        }
}

}
