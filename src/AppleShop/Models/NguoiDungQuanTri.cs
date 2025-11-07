using System;
using System.ComponentModel.DataAnnotations;  

namespace AppleShop.Models
{
    public class NguoiDungQuanTri
    {
        [Key]                                 
        public int MaNguoiDung { get; set; }
        public string TenDangNhap { get; set; } = null!;
        public string? HoTen { get; set; }
        public byte[] MatKhauHash { get; set; } = Array.Empty<byte>();
        public byte[] MatKhauSalt { get; set; } = Array.Empty<byte>();
        public string VaiTro { get; set; } = "QuanTriVien";
        public bool TrangThai { get; set; } = true;
        public DateTime NgayTao { get; set; } = DateTime.UtcNow;
    }
}
