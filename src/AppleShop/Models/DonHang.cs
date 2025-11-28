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

        public decimal TongTien { get; set; }
         public int TrangThai { get; set; } = 1; // 0 Nháp, 1 Mới, 2 Đang giao, 3 Hoàn tất, 4 Hủy
        public DateTime NgayTao { get; set; } = DateTime.Now;

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}
