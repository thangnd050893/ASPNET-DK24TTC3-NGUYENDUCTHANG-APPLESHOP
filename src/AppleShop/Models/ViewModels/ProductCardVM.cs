namespace AppleShop.Models.ViewModels
{
    public class ProductCardVM
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public decimal GiaBan { get; set; }
        public string? HinhAnh { get; set; }
    }
}
