using AppleShop.Models;

public class BienTheSanPham
{
    public int BienTheSanPhamId { get; set; }
    public int SanPhamId { get; set; }
    public string? MauSac { get; set; }     
    public string? DungLuong { get; set; } 
    public decimal? GiaBan { get; set; }   
    public string? ImageUrl { get; set; }    
    public SanPham SanPham { get; set; } = default!;
}
