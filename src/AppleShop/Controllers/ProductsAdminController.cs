using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleShop.Models;

[Authorize(Roles = "QuanTriVien")]
public class ProductsAdminController : Controller
{
    private readonly AppleShopContext _db;
    public ProductsAdminController(AppleShopContext db) => _db = db;

    // =============== LIST ===============
    // GET /admin/products?q=iphone
    [HttpGet("/admin/products")]
    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.SanPhams
                       .Include(x => x.DanhMuc)
                       .AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(x => x.Ten.Contains(q));

        ViewBag.Q = q;
        var data = await query.OrderByDescending(x => x.SanPhamId).ToListAsync();
        return View(data);
    }

    // =============== CREATE ===============
    // GET /admin/products/create
    [HttpGet("/admin/products/create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.Cats = await _db.DanhMucs.OrderBy(x => x.Ten).ToListAsync();
        return View(new SanPham());
    }

    // POST /admin/products/create
    [ValidateAntiForgeryToken]
    [HttpPost("/admin/products/create")]
    public async Task<IActionResult> Create(SanPham sp)
    {
        // Bảo vệ: bắt buộc tên, danh mục; giá >= 0
        if (string.IsNullOrWhiteSpace(sp.Ten))
            ModelState.AddModelError(nameof(sp.Ten), "Vui lòng nhập tên sản phẩm.");

        if (sp.DanhMucId == 0 || !await _db.DanhMucs.AnyAsync(x => x.DanhMucId == sp.DanhMucId))
            ModelState.AddModelError(nameof(sp.DanhMucId), "Vui lòng chọn danh mục hợp lệ.");

        if (sp.GiaBan < 0)
            ModelState.AddModelError(nameof(sp.GiaBan), "Giá bán phải ≥ 0.");

        // *** THÊM 2 DÒNG NÀY VÀO ***
        // Gán giá trị mặc định cho các trường không có trên form
        sp.NgayTao = DateTime.Now;
        sp.NoiBat = false; // Hoặc gán theo logic của bạn

        if (!ModelState.IsValid)
        {
            // Khi `NgayTao` đã được gán, ModelState sẽ không còn báo lỗi
            // trừ khi có các lỗi khác (như Ten, DanhMucId)
            ViewBag.Cats = await _db.DanhMucs.OrderBy(x => x.Ten).ToListAsync();
            return View(sp);
        }

        try
        {
            _db.SanPhams.Add(sp);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Đã thêm sản phẩm.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["err"] = "Không thể thêm sản phẩm. " + ex.Message;
            ViewBag.Cats = await _db.DanhMucs.OrderBy(x => x.Ten).ToListAsync();
            return View(sp);
        }
    }

    // =============== EDIT ===============
    // GET /admin/products/edit/5
    [HttpGet("/admin/products/edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var sp = await _db.SanPhams.FindAsync(id);
        if (sp == null) return NotFound();

        ViewBag.Cats = await _db.DanhMucs.OrderBy(x => x.Ten).ToListAsync();
        return View(sp);
    }

    // POST /admin/products/edit/5
    [ValidateAntiForgeryToken]
    [HttpPost("/admin/products/edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, SanPham sp)
    {
        var spDb = await _db.SanPhams.FindAsync(id);
        if (spDb == null) return NotFound();

        // Có thể thêm ràng buộc như ở Create nếu muốn
        if (!ModelState.IsValid)
        {
            ViewBag.Cats = await _db.DanhMucs.OrderBy(x => x.Ten).ToListAsync();
            sp.SanPhamId = id;
            return View(sp);
        }

        try
        {
            spDb.Ten = sp.Ten;
            spDb.GiaBan = sp.GiaBan;
            spDb.HinhAnh = sp.HinhAnh;
            spDb.MoTa = sp.MoTa;
            spDb.DanhMucId = sp.DanhMucId;

            await _db.SaveChangesAsync();
            TempData["msg"] = "Đã cập nhật sản phẩm.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["err"] = "Không thể cập nhật sản phẩm. " + ex.Message;
            ViewBag.Cats = await _db.DanhMucs.OrderBy(x => x.Ten).ToListAsync();
            sp.SanPhamId = id;
            return View(sp);
        }
    }

    // =============== DELETE ===============
    // POST /admin/products/delete/5
    [ValidateAntiForgeryToken]
    [HttpPost("/admin/products/delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sp = await _db.SanPhams.FindAsync(id);
        if (sp == null) return NotFound();

        try
        {
            _db.SanPhams.Remove(sp);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Đã xoá sản phẩm.";
        }
        catch (DbUpdateException)
        {
            TempData["err"] = "Không thể xoá vì sản phẩm đang được tham chiếu ở dữ liệu khác.";
        }
        catch (Exception ex)
        {
            TempData["err"] = "Không thể xoá sản phẩm. " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
