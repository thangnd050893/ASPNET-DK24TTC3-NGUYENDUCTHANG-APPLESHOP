namespace AppleShop.Models.ViewModels
{
    public class CategoryVM
    {
        public string TenDanhMuc { get; set; } = "";
        public List<ProductCardVM> SanPhams { get; set; } = new();
    }
}
