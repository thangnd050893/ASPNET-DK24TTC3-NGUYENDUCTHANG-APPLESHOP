using System.ComponentModel.DataAnnotations;

namespace AppleShop.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public int MaChiTiet { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public int MaSanPham { get; set; }
        public SanPham? SanPham { get; set; }

        [Display(Name = "Số lượng")]
        public int SoLuong { get; set; }

        [Display(Name = "Giá bán")]
        public decimal GiaBan { get; set; }

        [Display(Name = "Mã đơn hàng")]
        public int MaDonHang { get; set; }
        public DonHang? DonHang { get; set; }
    }
}
