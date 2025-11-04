namespace AppleShop.Models
{
    public class DanhMuc
    {
        public int DanhMucId { get; set; }
        public string Ten { get; set; } = "";
        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}
