using AppleShop.Models;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Models
{
    public class AppleShopContext : DbContext
    {
        public AppleShopContext(DbContextOptions<AppleShopContext> opt) : base(opt) { }

        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<DanhMuc> DanhMucs { get; set; }
        public DbSet<DonHang> DonHangs { get; set; } = null!;
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map đúng tên bảng trong SQL Server
            modelBuilder.Entity<DanhMuc>().ToTable("DanhMuc");   // nếu bảng tên DanhMuc
            modelBuilder.Entity<SanPham>().ToTable("SanPham");   // nếu bảng tên SanPham
        }
    }
}
