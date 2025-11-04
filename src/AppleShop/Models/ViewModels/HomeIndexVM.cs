namespace AppleShop.Models.ViewModels
{
    public class ProductCardVM
    {
        public int Id { get; set; }
        public string Ten { get; set; } = "";
        public decimal GiaBan { get; set; }
        public string? HinhAnh { get; set; }
    }

    public class HomeIndexVM
    {
        public List<ProductCardVM> Iphones { get; set; } = new();
        public List<ProductCardVM> Macbooks { get; set; } = new();
        public List<ProductCardVM> Ipads { get; set; } = new();
        // public List<ProductCardVM> Macbooks { get; set; } = new();
    }
}
