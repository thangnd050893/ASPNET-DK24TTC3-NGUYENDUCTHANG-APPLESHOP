// Data/AppDbContext.cs
using AppleShop.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    public DbSet<SanPham> SanPhams => Set<SanPham>();
    public DbSet<DanhMuc> DanhMucs => Set<DanhMuc>();
    public DbSet<DonHang> DonHangs { get; set; } = null!;
    public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; } = null!;

}
