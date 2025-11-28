using System.ComponentModel.DataAnnotations;

namespace AppleShop.Models
{
    public class SanPham
    {
        public int SanPhamId { get; set; }      // cột: SanPhamId
        public string Ten { get; set; } = "";   // cột: Ten
        public string? MoTa { get; set; }       // cột: MoTa
        public decimal GiaBan { get; set; }     // cột: GiaBan
        public string? HinhAnh { get; set; }    // cột: HinhAnh (ví dụ: /imgs/iphone/iphone17.png)
        public int DanhMucId { get; set; }      // cột: DanhMucId 
        public DanhMuc? DanhMuc { get; set; }
        public bool NoiBat { get; set; }        // cột: NoiBat
        public DateTime NgayTao { get; set; }   // cột: NgayTao
    }
}