using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppleShop.Models
{
    public class DonHang
    {
        public int DonHangId { get; set; }

        [MaxLength(50)]
        public string MaDon { get; set; } = string.Empty;

        // Thông tin người nhận
        [Required, MaxLength(150)]
        public string HoTen { get; set; } = "";

        [Required, MaxLength(20)]
        public string DienThoai { get; set; } = "";

        [Required, MaxLength(250)]
        public string DiaChi { get; set; } = "";

        [MaxLength(500)]
        public string? GhiChu { get; set; }

        [MaxLength(50)]
        public string PhuongThucThanhToan { get; set; } = "COD";

        // Tổng tiền
        public decimal TongTien { get; set; }

        // Nếu DB của bạn là NgayDat thì đổi lại tên property cho khớp:
        // public DateTime NgayDat { get; set; } = DateTime.Now;
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}
