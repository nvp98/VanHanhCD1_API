using Microsoft.EntityFrameworkCore;

namespace VanHanhCD1.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<Quyen> Quyens { get; set; }
        public DbSet<VH_LoVeVien> VH_LoVeVien { get; set; }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<NguoiDung>().ToTable("NguoiDung");
            //modelBuilder.Entity<Quyen>().ToTable("Quyen");
            //modelBuilder.Entity<NhanVien>().ToTable("NhanVien");
            modelBuilder.Entity<VH_LoVeVien>().ToTable("VH_LoVeVien");

            base.OnModelCreating(modelBuilder);
        }

    }
}
