using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using VanHanhCD1.Exceptions;
using VanHanhCD1.ExportExcel;
using VanHanhCD1.Models;
using VanHanhCD1.Repository.Interfaces;
using VanHanhCD1.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);
// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IVeVienRepository, VeVienRepository>();
builder.Services.AddScoped<IThieuKetRepository, ThieuKetRepository>();
builder.Services.AddScoped<IPhuTroRepository, PhuTroRepository>();
builder.Services.AddScoped<IVoiXiMangRepository, VoiXiMangRepository>();
builder.Services.AddScoped<ILuyenCocRepository, LuyenCocRepository>();
builder.Services.AddScoped<ExportLBDO1>();
builder.Services.AddScoped<ExportQHCVeVien>();
builder.Services.AddScoped<ExportLBTDVeVien>();
builder.Services.AddScoped<ExportLBMTPLieuBLuocVeVien>();
builder.Services.AddScoped<ExportLBMT>();
builder.Services.AddScoped<ExportThieuKet>();
builder.Services.AddScoped<ExportQuatGio>();
builder.Services.AddScoped<ExportTurbine>();
builder.Services.AddScoped<ExportNoiHoiMatVongMot>();
builder.Services.AddScoped<ExportKhuKhiKhoi>();
builder.Services.AddScoped<ExportTramNuocTuanHoan>();
builder.Services.AddScoped<ExportLoVoiQuay>();
builder.Services.AddScoped<ExportVoiXiMang>();
builder.Services.AddScoped<ExportLuyenCocCDQ>();
builder.Services.AddScoped<ExportLuyenCocMayNghien>();
builder.Services.AddScoped<ExportLuyenCocQGTH>();
builder.Services.AddScoped<ExportLuyenCocLBMTMD>();
builder.Services.AddScoped<ExportLuyenCocLocBuiNhaSang>();

builder.Services.AddGlobalExceptionHandling();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

// JWT Auth
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]))
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "DA_CD1 API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Nhập 'Bearer' và token vào đây (VD: Bearer abc.def.ghi)",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseCors("AllowFrontend"); // 👈 Đặt CORS ở đây

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
