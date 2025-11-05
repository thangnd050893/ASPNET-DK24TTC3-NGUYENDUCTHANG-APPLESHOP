using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppleShop.Models
{
    public class DonHang
    {
        public int DonHangId { get; set; }

        // Thông tin KH
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
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}
