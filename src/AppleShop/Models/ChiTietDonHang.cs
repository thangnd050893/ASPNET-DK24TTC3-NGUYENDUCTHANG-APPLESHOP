namespace AppleShop.Models
{
    public class ChiTietDonHang
    {
        public int ChiTietDonHangId { get; set; }

        public int DonHangId { get; set; }
        public DonHang DonHang { get; set; } = null!;

        public int SanPhamId { get; set; }
        public SanPham? SanPham { get; set; }

        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }   // giá tại thời điểm đặt
        public decimal ThanhTien => SoLuong * DonGia;
    }
}
