using AppleShop.Models;
using AppleShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly AppleShopContext _db;
    public HomeController(AppleShopContext db) => _db = db;
    public async Task<IActionResult> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return View(new List<ProductCardVM>());
        }

        var products = await _db.SanPhams
            .Where(p => p.Ten.Contains(keyword))
            .OrderByDescending(p => p.NgayTao)
            .Select(p => new ProductCardVM
            {
                Id = p.SanPhamId,
                Ten = p.Ten,
                GiaBan = p.GiaBan,
                HinhAnh = p.HinhAnh
            })
            .ToListAsync();

        ViewBag.Keyword = keyword;

        return View(products);
    }

    public async Task<IActionResult> Index()
    {
        // Lấy toàn bộ danh mục cần dùng trước
        var categories = await _db.DanhMucs
            .Where(d => d.Ten == "iPhone" || d.Ten == "IPHONE" ||
                        d.Ten == "Macbook" || d.Ten == "MACBOOK" || d.Ten == "Mac" ||
                        d.Ten == "iPad" || d.Ten == "IPAD")
            .Select(d => new { d.DanhMucId, d.Ten })
            .ToListAsync();

        int? iphoneId = categories.FirstOrDefault(x => x.Ten.Equals("iPhone", StringComparison.OrdinalIgnoreCase))?.DanhMucId;
        int? macbookId = categories.FirstOrDefault(x => x.Ten.Equals("Macbook", StringComparison.OrdinalIgnoreCase) || x.Ten.Equals("Mac", StringComparison.OrdinalIgnoreCase))?.DanhMucId;
        int? ipadId = categories.FirstOrDefault(x => x.Ten.Equals("iPad", StringComparison.OrdinalIgnoreCase))?.DanhMucId;

       
        async Task<List<ProductCardVM>> GetProducts(int? cateId)
        {
            if (cateId == null) return new List<ProductCardVM>();

            return await _db.SanPhams
                .Where(p => p.DanhMucId == cateId)
                .OrderByDescending(p => p.NgayTao)
                .Take(8)
                .Select(p => new ProductCardVM
                {
                    Id = p.SanPhamId,
                    Ten = p.Ten,
                    GiaBan = p.GiaBan,
                    HinhAnh = p.HinhAnh
                })
                .ToListAsync();
        }

        // Gán dữ liệu vào ViewModel
        var vm = new HomeIndexVM
        {
            Iphones = await GetProducts(iphoneId),
            Macbooks = await GetProducts(macbookId),
            Ipads = await GetProducts(ipadId)
        };

        return View(vm);
    }
}
