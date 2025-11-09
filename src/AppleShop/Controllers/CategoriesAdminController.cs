using AppleShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "QuanTriVien")]
[Route("admin/categories")]
public class CategoriesAdminController : Controller
{
    private readonly AppleShopContext _db;
    public CategoriesAdminController(AppleShopContext db) => _db = db;

    // GET /admin/categories
    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var list = await _db.DanhMucs
                            .Include(x => x.DanhMucCha)
                            .OrderBy(x => x.DanhMucId)
                            .ToListAsync();
        return View(list);
    }

    // GET /admin/categories/create
    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        ViewBag.ListChaSelect = new SelectList(await _db.DanhMucs.ToListAsync(), "DanhMucId", "Ten");
        return View(new DanhMuc());
    }

    // POST /admin/categories/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DanhMuc model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ListChaSelect = new SelectList(await _db.DanhMucs.ToListAsync(), "DanhMucId", "Ten");
            return View(model);
        }

        _db.DanhMucs.Add(model);
        await _db.SaveChangesAsync();
        TempData["ok"] = "Thêm danh mục thành công!";
        return RedirectToAction(nameof(Index));
    }

    // GET /admin/categories/edit/5
    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var dm = await _db.DanhMucs.FindAsync(id);
        if (dm == null) return NotFound();

        // Không cho chọn chính nó làm cha
        var list = await _db.DanhMucs.Where(x => x.DanhMucId != id).ToListAsync();
        ViewBag.ListChaSelect = new SelectList(list, "DanhMucId", "Ten", dm.DanhMucChaId);
        return View(dm);
    }

    // POST /admin/categories/edit/5
    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DanhMuc input)
    {
        if (id != input.DanhMucId) return BadRequest();

        if (input.DanhMucChaId == id)
            ModelState.AddModelError("DanhMucChaId", "Không thể chọn chính nó làm danh mục cha.");

        if (!ModelState.IsValid)
        {
            var list = await _db.DanhMucs.Where(x => x.DanhMucId != id).ToListAsync();
            ViewBag.ListChaSelect = new SelectList(list, "DanhMucId", "Ten", input.DanhMucChaId);
            return View(input);
        }

        var dm = await _db.DanhMucs.FindAsync(id);
        if (dm == null) return NotFound();

        dm.Ten = input.Ten;
        dm.DanhMucChaId = input.DanhMucChaId;

        await _db.SaveChangesAsync();
        TempData["ok"] = "Cập nhật thành công!";
        return RedirectToAction(nameof(Index));
    }

    // POST /admin/categories/delete/5
    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var dm = await _db.DanhMucs.Include(x => x.SanPhams)
                                   .FirstOrDefaultAsync(x => x.DanhMucId == id);
        if (dm == null) return NotFound();

        if (dm.SanPhams?.Any() == true)
        {
            TempData["err"] = "Không thể xóa: danh mục đang có sản phẩm.";
            return RedirectToAction(nameof(Index));
        }

        _db.DanhMucs.Remove(dm);
        await _db.SaveChangesAsync();
        TempData["ok"] = "Đã xóa danh mục.";
        return RedirectToAction(nameof(Index));
    }
}
