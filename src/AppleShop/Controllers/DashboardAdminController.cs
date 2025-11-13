using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleShop.Models;

namespace AppleShop.Controllers
{
    [Route("admin/[controller]")]
    public class DashboardAdminController : Controller
    {
        private readonly AppleShopContext _db;
        public DashboardAdminController(AppleShopContext db) => _db = db;

        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(string? period = "month")
        {
            DateTime today = DateTime.Today;
            DateTime fromDate;

            switch ((period ?? "month").ToLower())
            {
                case "today": fromDate = today; break;
                case "yesterday": fromDate = today.AddDays(-1); break;
                case "week": fromDate = today.AddDays(-7); break;
                case "quarter": fromDate = today.AddMonths(-3); break;
                case "year": fromDate = new DateTime(today.Year, 1, 1); break;
                case "all": fromDate = DateTime.MinValue; break;
                case "month":
                default: fromDate = today.AddDays(-30); break;
            }

            var ordersQuery = _db.DonHangs.AsNoTracking().AsQueryable();
            if (fromDate > DateTime.MinValue)
                ordersQuery = ordersQuery.Where(o => o.NgayTao >= fromDate);

            var vm = new DashboardVM
            {
                Period = period ?? "month",
                From = fromDate > DateTime.MinValue ? fromDate : (DateTime?)null,
                To = today,
                TotalOrders = await ordersQuery.CountAsync(),
                TotalRevenue = await ordersQuery.SumAsync(o => (decimal?)o.TongTien) ?? 0m,
                PendingOrders = await ordersQuery.CountAsync(o => o.TrangThai == 1),
                ShippingOrders = await ordersQuery.CountAsync(o => o.TrangThai == 2),
                DoneOrders = await ordersQuery.CountAsync(o => o.TrangThai == 3),
                TotalProducts = await _db.SanPhams.AsNoTracking().CountAsync(),
                TotalCustomers = 0
            };
            // ======================= TOP SẢN PHẨM BÁN NHIỀU =========================
            var itemsQuery = _db.ChiTietDonHangs.AsNoTracking()
                .Include(i => i.SanPham)
                .Include(i => i.DonHang)
                .Where(i => i.DonHang != null && i.SanPham != null); // tránh null

            if (fromDate > DateTime.MinValue)
                itemsQuery = itemsQuery.Where(i => i.DonHang!.NgayTao >= fromDate);

            vm.TopByQty = await itemsQuery
                .GroupBy(i => new { i.SanPhamId, i.SanPham!.Ten, i.SanPham!.HinhAnh }) // ✅ Đã fix: Thêm '!'
                .Select(g => new TopQtyVM
                {
                    SanPhamId = g.Key.SanPhamId,
                    Ten = g.Key.Ten ?? "(Sản phẩm)",
                    HinhAnh = g.Key.HinhAnh, // Cân nhắc xử lý null ở đây nếu HinhAnh có thể null
                    SoLuongBan = g.Sum(x => x.SoLuong),

                
                    TongDoanhThu = g.Sum(x => ((decimal?)x.DonGia * x.SoLuong) ?? 0m)
                })
                .OrderByDescending(x => x.SoLuongBan)
                .ThenByDescending(x => x.TongDoanhThu)
                .Take(10)
                .ToListAsync();

            // =======================================================================

            ViewData["Title"] = "Tổng quan";
            return View(vm);
        }
    }

    // ====================== VIEWMODEL ======================
    public class DashboardVM
    {
        public string Period { get; set; } = "month";
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCustomers { get; set; }

        public int PendingOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int DoneOrders { get; set; }

        public List<TopQtyVM> TopByQty { get; set; } = new();
    }

    public class TopQtyVM
    {
        public int SanPhamId { get; set; }
        public string Ten { get; set; } = "";
        public string? HinhAnh { get; set; }
        public int SoLuongBan { get; set; }
        public decimal TongDoanhThu { get; set; }
        public decimal GiaTrungBinh => SoLuongBan == 0 ? 0 : decimal.Round(TongDoanhThu / SoLuongBan, 0);
    }
}
