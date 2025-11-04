namespace AppleShop.Models.ViewModels
{
    public class ProductDetailVM
    {
        public int Id { get; set; }
        public string Ten { get; set; } = "";
        public decimal GiaBan { get; set; }
        public string? HinhAnh { get; set; }
        public string? MoTa { get; set; }
        public string? DanhMucTen { get; set; }

        // (tuỳ chọn) giá cũ nếu DB có cột
        public decimal? GiaCu { get; set; }

        // Gợi ý sản phẩm liên quan (cùng danh mục)
        public List<ProductCardVM> Related { get; set; } = new();
    }
}
