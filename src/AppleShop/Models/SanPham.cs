using System.ComponentModel.DataAnnotations;

namespace AppleShop.Models
{
    public class SanPham
    {
        [Key]
        public int MaSanPham { get; set; }

        [Required, StringLength(150)]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = "";

        [StringLength(100)]
        [Display(Name = "Loại sản phẩm")]
        public string? LoaiSanPham { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Giá bán")]
        public decimal GiaBan { get; set; }

        [StringLength(500)]
        [Display(Name = "Hình ảnh")]
        public string? HinhAnh { get; set; }

        [StringLength(2000)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }
    }
}
