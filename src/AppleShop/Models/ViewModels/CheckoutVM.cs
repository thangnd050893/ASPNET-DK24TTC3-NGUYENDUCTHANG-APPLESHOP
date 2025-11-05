using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AppleShop.Models.ViewModels
{
    public class CheckoutVM
    {
        // Thông tin KH
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string DienThoai { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string DiaChi { get; set; } = "";

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        [Display(Name = "Phương thức thanh toán")]
        public string PhuongThucThanhToan { get; set; } = "COD";

        // Giỏ hàng
        public List<CartItemVM> Items { get; set; } = new();

        // Tổng tiền
        public decimal SubTotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
    }
}
