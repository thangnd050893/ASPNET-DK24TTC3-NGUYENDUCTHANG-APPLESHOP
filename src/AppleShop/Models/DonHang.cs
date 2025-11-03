using System.ComponentModel.DataAnnotations;

namespace AppleShop.Models
{
    public class DonHang
    {
        [Key]
        public int MaDonHang { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime NgayTao { get; set; } = DateTime.Now;

        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}
