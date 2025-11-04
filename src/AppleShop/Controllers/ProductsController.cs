using AppleShop.Models;
using AppleShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProductsController : Controller
{
    private readonly AppleShopContext _db;
    public ProductsController(AppleShopContext db) => _db = db;

    [HttpGet("/roducts/detail/{id:int}")]
    public async Task<IActionResult> Detail(int id)
    {
        var sp = await _db.SanPhams
            .Include(p => p.DanhMuc)
            .Where(p => p.SanPhamId == id)
            .Select(p => new ProductDetailVM
            {
                Id = p.SanPhamId,
                Ten = p.Ten,
                GiaBan = p.GiaBan,
                HinhAnh = p.HinhAnh,
                MoTa = p.MoTa,
                DanhMucTen = p.DanhMuc != null ? p.DanhMuc.Ten : null,
                // Nếu có cột GiaCu thì map vào đây
                // GiaCu = p.GiaCu
            })
            .FirstOrDefaultAsync();

        if (sp == null) return NotFound();

        // Lấy sản phẩm liên quan (cùng danh mục, loại trừ chính nó)
        var cateId = await _db.SanPhams
            .Where(x => x.SanPhamId == id)
            .Select(x => x.DanhMucId)
            .FirstOrDefaultAsync();

        sp.Related = await _db.SanPhams
            .Where(x => x.DanhMucId == cateId && x.SanPhamId != id)
            .OrderByDescending(x => x.NgayTao)
            .Take(6)
            .Select(x => new ProductCardVM
            {
                Id = x.SanPhamId,
                Ten = x.Ten,
                GiaBan = x.GiaBan,
                HinhAnh = x.HinhAnh
            })
            .ToListAsync();

        // SEO sơ bộ
        ViewData["Title"] = sp.Ten + " | AppleShop";
        ViewData["Description"] = (sp.MoTa ?? sp.Ten);

        return View(sp);
    }

    // (tuỳ chọn) xem tất cả theo danh mục: /products/category/1
    [HttpGet("/products/category/{id:int}")]
    public async Task<IActionResult> Category(int id)
    {
        var items = await _db.SanPhams
            .Where(p => p.DanhMucId == id)
            .OrderByDescending(p => p.NgayTao)
            .ToListAsync();

        return View(items); // tạo view đơn giản sau
    }
}
