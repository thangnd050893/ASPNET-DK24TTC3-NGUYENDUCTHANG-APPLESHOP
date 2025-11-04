namespace AppleShop.Models.ViewModels
{
    public class HomeIndexVM
    {
        public List<ProductCardVM> Iphones { get; set; } = new();
        public List<ProductCardVM> Macbooks { get; set; } = new();
        public List<ProductCardVM> Ipads { get; set; } = new();
    }
}
