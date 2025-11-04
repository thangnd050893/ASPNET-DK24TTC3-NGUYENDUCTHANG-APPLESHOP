using System.Linq;
using System.Threading.Tasks;
using AppleShop.Models;
using AppleShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppleShopContext _db;
        public ProductsController(AppleShopContext db) => _db = db;

        // ======================================================
        // 1) TRANG CHI TIẾT SẢN PHẨM: /products/detail/{id}
        // ======================================================
        [HttpGet("/products/detail/{id:int}")]
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
                    DanhMucTen = p.DanhMuc != null ? p.DanhMuc.Ten : null
                })
                .FirstOrDefaultAsync();

            if (sp == null) return NotFound();

            // Lấy danh mục của sản phẩm hiện tại
            var cateId = await _db.SanPhams
                .Where(x => x.SanPhamId == id)
                .Select(x => x.DanhMucId)
                .FirstOrDefaultAsync();

            // Gợi ý sản phẩm liên quan (cùng danh mục, loại trừ chính nó)
            sp.Related = await _db.SanPhams
                .AsNoTracking()
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
            ViewData["Description"] = sp.MoTa ?? sp.Ten;

            return View(sp);
        }

        // ======================================================================
        // 2) TRANG DANH MỤC: /products/category/{id}?sort=new|price-asc|price-desc&page=1&pageSize=12
        // ======================================================================
        [HttpGet("/products/category/{id:int}")]
        public async Task<IActionResult> Category(int id, string sort = "new", int page = 1, int pageSize = 12)
        {
            // Lấy thông tin danh mục
            var danhMuc = await _db.DanhMucs
                .AsNoTracking()
                .Where(x => x.DanhMucId == id)
                .Select(x => new { x.DanhMucId, x.Ten })
                .FirstOrDefaultAsync();

            if (danhMuc == null) return NotFound();

            // Base query
            var q = _db.SanPhams
                .AsNoTracking()
                .Where(p => p.DanhMucId == id);

            // Sort
            sort = (sort ?? "new").ToLower();
            q = sort switch
            {
                "price-asc" => q.OrderBy(p => p.GiaBan),
                "price-desc" => q.OrderByDescending(p => p.GiaBan),
                _ => q.OrderByDescending(p => p.NgayTao)
            };

            // Pagination
            var total = await q.CountAsync();
            page = System.Math.Max(1, page);
            pageSize = System.Math.Clamp(pageSize, 6, 48);

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductCardVM
                {
                    Id = p.SanPhamId,
                    Ten = p.Ten,
                    GiaBan = p.GiaBan,
                    HinhAnh = p.HinhAnh
                })
                .ToListAsync();

            // Tạo ViewModel với properties đúng tên
            var vm = new CategoryVM
            {
                TenDanhMuc = danhMuc.Ten ?? "Danh mục",
                SanPhams = items
            };

            // Truyền thêm dữ liệu phân trang qua ViewData hoặc ViewBag
            ViewData["Sort"] = sort;
            ViewData["Page"] = page;
            ViewData["PageSize"] = pageSize;
            ViewData["TotalItems"] = total;
            ViewData["TotalPages"] = (int)System.Math.Ceiling(total / (double)pageSize);
            ViewData["CategoryId"] = id;

            // SEO sơ bộ
            ViewData["Title"] = vm.TenDanhMuc + " | AppleShop";
            ViewData["Description"] = "Danh mục " + vm.TenDanhMuc + " - AppleShop";

            return View(vm);
        }
    }
}