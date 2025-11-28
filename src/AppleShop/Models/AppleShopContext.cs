using Microsoft.EntityFrameworkCore;

namespace AppleShop.Models
{
    public class AppleShopContext : DbContext
    {
        public AppleShopContext(DbContextOptions<AppleShopContext> opt) : base(opt) { }

        // DbSets
        public DbSet<SanPham> SanPhams { get; set; } = null!;
        public DbSet<DanhMuc> DanhMucs { get; set; } = null!;
        public DbSet<DonHang> DonHangs { get; set; } = null!;
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; } = null!;
        public DbSet<NguoiDungQuanTri> NguoiDungQuanTris { get; set; } = null!;

    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

       
            modelBuilder.Entity<DanhMuc>(entity =>
            {
                entity.ToTable("DanhMuc");
                entity.HasKey(e => e.DanhMucId);

                entity.Property(e => e.Ten)
                      .HasMaxLength(100)
                      .IsRequired();

           
                entity.HasOne(e => e.DanhMucCha)
                      .WithMany(e => e.DanhMucCon)
                      .HasForeignKey(e => e.DanhMucChaId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });

     
            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.ToTable("SanPham");
                entity.HasKey(e => e.SanPhamId);

                entity.HasOne(e => e.DanhMuc)
                      .WithMany(d => d.SanPhams)
                      .HasForeignKey(e => e.DanhMucId)
                      .OnDelete(DeleteBehavior.Restrict); 
            });

      
            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.ToTable("DonHangs");
                entity.HasKey(e => e.DonHangId);

              
            });

            // ---- ChiTietDonHang (bảng thật: ChiTietDonHangs) ----
            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.ToTable("ChiTietDonHangs");
                entity.HasKey(e => e.ChiTietDonHangId);

        
                entity.HasOne(ct => ct.DonHang)
                      .WithMany(o => o.ChiTietDonHangs)
                      .HasForeignKey(ct => ct.DonHangId)
                      .OnDelete(DeleteBehavior.Cascade);

      
                entity.HasOne(ct => ct.SanPham)
                      .WithMany()
                      .HasForeignKey(ct => ct.SanPhamId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ---- NguoiDungQuanTri (bảng thật: NguoiDungQuanTri) ----
            modelBuilder.Entity<NguoiDungQuanTri>(entity =>
            {
                entity.ToTable("NguoiDungQuanTri");
                entity.HasKey(x => x.MaNguoiDung);

                entity.HasIndex(x => x.TenDangNhap).IsUnique();

                entity.Property(x => x.TenDangNhap)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(x => x.HoTen)
                      .HasMaxLength(150);

                entity.Property(x => x.VaiTro)
                      .HasMaxLength(50);

                entity.Property(x => x.TrangThai).HasDefaultValue(true);
                entity.Property(x => x.NgayTao).HasDefaultValueSql("SYSDATETIME()");
            });
            modelBuilder.Entity<DonHang>(e =>
            {
                e.ToTable("DonHangs");
                e.Property(x => x.MaDon).IsRequired(false);
                e.Property(x => x.HoTen).IsRequired(false);
                e.Property(x => x.DienThoai).IsRequired(false);
                e.Property(x => x.DiaChi).IsRequired(false);
                e.Property(x => x.GhiChu).IsRequired(false);
                e.Property(x => x.PhuongThucThanhToan).IsRequired(false);

                // nếu cột tiền là decimal(18,2) trong SQL:
                e.Property(x => x.TongTien).HasColumnType("decimal(18,2)");
            });

        }
    }
}
