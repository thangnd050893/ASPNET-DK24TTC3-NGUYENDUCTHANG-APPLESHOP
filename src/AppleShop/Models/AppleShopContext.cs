using Microsoft.EntityFrameworkCore;

namespace AppleShop.Models
{
    public class AppleShopContext : DbContext
    {
        public AppleShopContext(DbContextOptions<AppleShopContext> options)
            : base(options)
        {
        }

        // Các bảng trong cơ sở dữ liệu
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

        // Đặt tên bảng hiển thị tiếng Việt trong SQL Server
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SanPham>().ToTable("Sản phẩm");
            modelBuilder.Entity<DonHang>().ToTable("Đơn hàng");
            modelBuilder.Entity<ChiTietDonHang>().ToTable("Chi tiết đơn hàng");
        }
    }
}
