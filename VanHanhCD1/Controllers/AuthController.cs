using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VanHanhCD1.Models;
using VanHanhCD1.DTOs;
using VanHanhCD1.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;

namespace VanHanhCD1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            string hashed = Encryptor.MD5Hash(model.Password);

            var user = await _context.NguoiDungs
                .Include(x => x.NhanVien)
                .Include(x => x.Quyen)
                .FirstOrDefaultAsync(x => x.TenDangNhap == model.Username && x.MatKhau == hashed && x.IsLock == 0);

            if (user == null)
                return Unauthorized("Sai tên đăng nhập hoặc mật khẩu");

            // 1. Generate claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.TenDangNhap),
                new Claim(ClaimTypes.Role, user.Quyen?.TenQuyen ?? "User"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // 2. Generate AccessToken
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:ExpireMinutes"])),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // 3. Generate RefreshToken
            string refreshToken = Guid.NewGuid().ToString();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                accessToken,
                refreshToken,
                expiresIn = 300,
                user = new
                {
                    user.IDNguoiDung,
                    user.TenDangNhap,
                    user.NhanVien?.HoTen,
                    user.NhanVien?.MaNV,
                    user.IDQuyen,
                    user.Quyen?.TenQuyen
                }
            });
        }

        [Authorize(Roles = "Quản trị")]
        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts()
        {
            var accounts = await _context.NguoiDungs
                .Include(u => u.NhanVien)
                    .ThenInclude(nv => nv.PhongBan) // 👈 Join thêm PhongBan
                .Include(u => u.Quyen)
                .Select(u => new
                {
                    u.IDNguoiDung,
                    u.TenDangNhap,
                    HoTen = u.NhanVien.HoTen,
                    MaNV = u.NhanVien.MaNV,
                    TenPhongBan = u.NhanVien.PhongBan != null ? u.NhanVien.PhongBan.TenPhongBan : null, // 👈 Thêm tên phòng ban
                    Quyen = u.Quyen.TenQuyen,
                    IsLocked = u.IsLock == 1
                })
                .ToListAsync();

            return Ok(accounts);
        }

        [HttpGet("nhanvien")]
        [Authorize(Roles = "Quản trị")]
        public async Task<IActionResult> GetNhanViens()
        {
            var list = await _context.NhanViens
                .Select(nv => new
                {
                    nv.ID,
                    nv.MaNV,
                    nv.HoTen
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("quyen")]
        [Authorize(Roles = "Quản trị")]
        public async Task<IActionResult> GetQuyens()
        {
            var list = await _context.Quyens
                .Select(q => new
                {
                    q.IDQuyen,
                    q.TenQuyen
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Quản trị")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDTO model)
        {
            // 1. Kiểm tra tên đăng nhập đã tồn tại chưa
            var exists = await _context.NguoiDungs
                .AnyAsync(x => x.TenDangNhap == model.TenDangNhap);
            if (exists)
                return BadRequest("Tên đăng nhập đã tồn tại.");

            // 2. Kiểm tra nhân viên tồn tại không
            var nhanVien = await _context.NhanViens.FindAsync(model.NhanVienID);
            if (nhanVien == null)
                return BadRequest("Nhân viên không tồn tại.");

            // 3. Tạo mới tài khoản
            var user = new NguoiDung
            {
                TenDangNhap = model.TenDangNhap,
                MatKhau = Encryptor.MD5Hash(model.MatKhau),
                IDQuyen = model.IDQuyen,
                NhanVienID = model.NhanVienID,
                IsLock = 0
            };

            _context.NguoiDungs.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tạo tài khoản thành công.",
                user.IDNguoiDung,
                user.TenDangNhap
            });
        }


        [HttpPut("update-role/{idNguoiDung}")]
        [Authorize(Roles = "Quản trị")]
        public async Task<IActionResult> UpdateRoleAndStatus(int idNguoiDung, [FromBody] UpdateRoleStatusDTO model)
        {
            var user = await _context.NguoiDungs.FindAsync(idNguoiDung);
            if (user == null)
                return NotFound("Không tìm thấy người dùng.");

            // Cập nhật quyền nếu khác
            if (user.IDQuyen != model.IDQuyen)
                user.IDQuyen = model.IDQuyen;

            // Cập nhật trạng thái khóa/mở nếu khác
            var newIsLock = model.IsLock ? 1 : 0;
            if (user.IsLock != newIsLock)
                user.IsLock = newIsLock;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thành công." });
        }

    }
}
