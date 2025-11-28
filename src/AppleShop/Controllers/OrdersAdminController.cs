using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
using AppleShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace AppleShop.Controllers
{
    // [Authorize(Roles="QuanTriVien")]
    public class OrdersAdminController : Controller
    {
        private readonly AppleShopContext _db;
        public OrdersAdminController(AppleShopContext db) => _db = db;

        // ====================== LIST ======================
        // GET /admin/orders
        [HttpGet("/admin/orders")]
        public async Task<IActionResult> Index(
            string? q,            // Tìm theo mã đơn / SĐT / tên
            int? status,          // 1=Mới, 2=Đang giao, 3=Hoàn tất, 4=Hủy
            DateTime? from,       // lọc từ ngày
            DateTime? to,         // lọc đến ngày
            int page = 1,
            int pageSize = 10)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 100) pageSize = 10;

            var query = _db.DonHangs.AsNoTracking().AsQueryable();

            // từ khóa
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.MaDon.Contains(q) ||
                    x.DienThoai.Contains(q) ||
                    x.HoTen.Contains(q));
            }

     
            if (status.HasValue)
                query = query.Where(x => x.TrangThai == status.Value);

            // theo ngày
            if (from.HasValue) query = query.Where(x => x.NgayTao >= from.Value.Date);
            if (to.HasValue) query = query.Where(x => x.NgayTao < to.Value.Date.AddDays(1));

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.NgayTao)
                .Select(x => new OrderListItemVM
                {
                    DonHangId = x.DonHangId,
                    MaDon = x.MaDon,
                    NgayTao = x.NgayTao,
                    HoTen = x.HoTen,
                    DienThoai = x.DienThoai,
                    DiaChi = x.DiaChi,
                    TongTien = x.TongTien,
                    TrangThai = x.TrangThai,
                    SoLuong = x.ChiTietDonHangs.Sum(c => (int?)c.SoLuong) ?? 0
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new OrdersIndexVM
            {
                Q = q,
                Status = status,
                From = from,
                To = to,
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items
            };

            return View(vm);
        }

  
        // GET /admin/orders/{id}
        [HttpGet("/admin/orders/{id:int}")]
        public async Task<IActionResult> Detail(int id)
        {
            var order = await _db.DonHangs
                .AsNoTracking()
                .Where(x => x.DonHangId == id)
                .Select(x => new OrderDetailVM
                {
                    DonHangId = x.DonHangId,
                    MaDon = x.MaDon,
                    NgayTao = x.NgayTao,
                    HoTen = x.HoTen,
                    DienThoai = x.DienThoai,
                    DiaChi = x.DiaChi,
                    GhiChu = x.GhiChu,
                    TrangThai = x.TrangThai,
                    TongTien = x.TongTien
                })
                .SingleOrDefaultAsync();

            if (order == null) return NotFound();

            order.Items = await _db.ChiTietDonHangs
                .AsNoTracking()
                .Where(c => c.DonHangId == id)
                .Select(c => new OrderDetailVM.ItemVM
                {
                   Ten = c.SanPham != null ? c.SanPham.Ten : "(Sản phẩm đã xóa)",
    HinhAnh = c.SanPham != null ? c.SanPham.HinhAnh : null,
    SoLuong = c.SoLuong,
    DonGia = c.DonGia
                })
                .ToListAsync();

            return View(order);
        }

   
        // POST /admin/orders/{id}/update-status
        [HttpPost("/admin/orders/{id:int}/update-status")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, int status, string? note)
        {
            var o = await _db.DonHangs.FindAsync(id);
            if (o == null) return NotFound();

            o.TrangThai = status; // DB dùng int
            if (!string.IsNullOrWhiteSpace(note))
                o.GhiChu = note.Trim();

            await _db.SaveChangesAsync();
            TempData["ok"] = "Đã cập nhật trạng thái đơn hàng.";
            return RedirectToAction(nameof(Detail), new { id });
        }

       
        public class OrderInfoInput
        {
            [Required, MaxLength(150)]
            public string HoTen { get; set; } = "";

            [Required, MaxLength(20)]
            public string DienThoai { get; set; } = "";

            [Required, MaxLength(250)]
            public string DiaChi { get; set; } = "";

            [MaxLength(500)]
            public string? GhiChu { get; set; }
        }

        // POST /admin/orders/{id}/update-info
        [HttpPost("/admin/orders/{id:int}/update-info")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo(int id, OrderInfoInput input)
        {
            if (!ModelState.IsValid)
            {
                TempData["err"] = "Dữ liệu chưa hợp lệ. Vui lòng kiểm tra lại.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            var o = await _db.DonHangs.FindAsync(id);
            if (o == null) return NotFound();

            o.HoTen = input.HoTen.Trim();
            o.DienThoai = input.DienThoai.Trim();
            o.DiaChi = input.DiaChi.Trim();
            o.GhiChu = string.IsNullOrWhiteSpace(input.GhiChu) ? null : input.GhiChu.Trim();

            TempData["ok"] = "Đã cập nhật thông tin người nhận.";
            return RedirectToAction(nameof(Detail), new { id });
        }
    }

    // ================== VIEWMODELS ==================
    public class OrdersIndexVM
    {
        public string? Q { get; set; }
        public int? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);

        public List<OrderListItemVM> Items { get; set; } = new();
    }

    public class OrderListItemVM
    {
        public int DonHangId { get; set; }
        public string MaDon { get; set; } = "";
        public DateTime NgayTao { get; set; }
        public string? HoTen { get; set; }
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
        public decimal TongTien { get; set; }
        public int TrangThai { get; set; }   
        public int SoLuong { get; set; }
    }

    public class OrderDetailVM
    {
        public int DonHangId { get; set; }
        public string MaDon { get; set; } = "";
        public DateTime NgayTao { get; set; }
        public string? HoTen { get; set; }
        public string? DienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? GhiChu { get; set; }
        public int TrangThai { get; set; }  
        public decimal TongTien { get; set; }

        public List<ItemVM> Items { get; set; } = new();
        public class ItemVM
        {
            public string Ten { get; set; } = "";
            public string? HinhAnh { get; set; }
            public int SoLuong { get; set; }
            public decimal DonGia { get; set; }
            public decimal ThanhTien => SoLuong * DonGia;
        }
    }
}
