using AppleShop.Models;

public class BienTheSanPham
{
    public int BienTheSanPhamId { get; set; }
    public int SanPhamId { get; set; }
    public string? MauSac { get; set; }      // ví dụ: 'orange' / '#ff6600'
    public string? DungLuong { get; set; }   // '256GB' / '512GB'
    public decimal? GiaBan { get; set; }     // có thể null -> fallback GiaBan sản phẩm
    public string? ImageUrl { get; set; }    // ảnh chính của biến thể
    public SanPham SanPham { get; set; } = default!;
}
