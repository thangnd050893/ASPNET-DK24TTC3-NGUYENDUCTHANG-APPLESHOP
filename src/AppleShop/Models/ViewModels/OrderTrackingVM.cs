namespace AppleShop.Models.ViewModels
{
    public class OrderItemVM
    {
        public string TenSanPham { get; set; } = "";
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
        public string? HinhAnh { get; set; }
    }

    public class OrderTrackingVM
    {
        // input
        public string? MaDon { get; set; }
        public string? PhoneOrEmail { get; set; }

        // output
        public int DonHangId { get; set; }
        public string TenKhach { get; set; } = "";
        public string SoDienThoai { get; set; } = "";
        public string Email { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public string TrangThaiText { get; set; } = "";
        public decimal TongTien { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemVM> Items { get; set; } = new();
    }
}
