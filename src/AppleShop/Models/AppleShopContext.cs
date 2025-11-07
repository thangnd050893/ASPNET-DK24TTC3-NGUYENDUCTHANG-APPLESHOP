using Microsoft.EntityFrameworkCore;

namespace AppleShop.Models
{
    public class AppleShopContext : DbContext
    {
        public AppleShopContext(DbContextOptions<AppleShopContext> opt) : base(opt) { }

        public DbSet<SanPham> SanPhams { get; set; } = null!;
        public DbSet<DanhMuc> DanhMucs { get; set; } = null!;
        public DbSet<DonHang> DonHangs { get; set; } = null!;
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; } = null!;
        public DbSet<NguoiDungQuanTri> NguoiDungQuanTris { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Giữ nguyên mapping cũ
            modelBuilder.Entity<DanhMuc>().ToTable("DanhMuc");
            modelBuilder.Entity<SanPham>().ToTable("SanPham");

            // ===== BỔ SUNG: cấu hình bảng NguoiDungQuanTri =====
            modelBuilder.Entity<NguoiDungQuanTri>(entity =>
            {
                entity.ToTable("NguoiDungQuanTri");      // tên bảng trong SQL Server
                entity.HasKey(x => x.MaNguoiDung);       // vì tên khác "Id" nên chỉ định khóa chính

                entity.HasIndex(x => x.TenDangNhap).IsUnique(); // chặn trùng username

                entity.Property(x => x.TenDangNhap)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.HoTen)
                      .HasMaxLength(150);

                entity.Property(x => x.VaiTro)
                      .HasMaxLength(50);

                // Giá trị mặc định giống DB: TrangThai = 1, NgayTao = SYSDATETIME()
                entity.Property(x => x.TrangThai).HasDefaultValue(true);
                entity.Property(x => x.NgayTao).HasDefaultValueSql("SYSDATETIME()");
            });
        }
    }
}
