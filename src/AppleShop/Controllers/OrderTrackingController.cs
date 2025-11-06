using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AppleShop.Models;
using AppleShop.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Controllers
{
    public class OrderTrackingController : Controller
    {
        private readonly AppleShopContext _db;
        public OrderTrackingController(AppleShopContext db) => _db = db;

        // GET /tra-cuu-don
        [HttpGet("/tra-cuu-don")]
        public IActionResult Index() => View(new OrderTrackingVM());

        // POST /tra-cuu-don — CHỈ CẦN SỐ ĐIỆN THOẠI
        [HttpPost("/tra-cuu-don")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(OrderTrackingVM input)
        {
            if (string.IsNullOrWhiteSpace(input.PhoneOrEmail))
            {
                ModelState.AddModelError("", "Vui lòng nhập Số điện thoại để tra cứu.");
                return View(input);
            }

            var phone = input.PhoneOrEmail.Trim();

            // Lấy ĐƠN MỚI NHẤT theo số điện thoại
            var order = await _db.DonHangs
                .AsNoTracking()
                .Where(o => o.DienThoai == phone)
                .OrderByDescending(o => o.NgayTao)
                .Select(o => new
                {
                    o.DonHangId,
                    o.MaDon,
                    o.HoTen,
                    o.DienThoai,
                    o.DiaChi,
                    o.TongTien,
                    o.NgayTao,
                    o.GhiChu,
                    o.PhuongThucThanhToan
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                ModelState.AddModelError("", "Không tìm thấy đơn hàng nào khớp với số điện thoại này.");
                return View(input);
            }

            // LẤY CHI TIẾT ĐƠN: JOIN qua SanPhams để lấy tên sản phẩm
            // LẤY CHI TIẾT ĐƠN: JOIN qua SanPhams để lấy tên + ảnh
            List<OrderItemVM> items = await _db.ChiTietDonHangs
                .AsNoTracking()
                .Where(c => c.DonHangId == order.DonHangId)
                .Join(_db.SanPhams.AsNoTracking(),
                      c => c.SanPhamId,
                      s => s.SanPhamId,
                      (c, s) => new OrderItemVM
                      {
                          TenSanPham = s.Ten,             // hoặc s.TenSanPham nếu DB của bạn dùng tên đó
                          SoLuong = c.SoLuong,
                          DonGia = c.DonGia,
                          HinhAnh = s.HinhAnh             // ← lấy ảnh từ bảng SanPhams
                      })
                .ToListAsync();


            var vm = new OrderTrackingVM
            {
                MaDon = order.MaDon,
                PhoneOrEmail = phone,
                DonHangId = order.DonHangId,
                TenKhach = order.HoTen ?? "",
                SoDienThoai = order.DienThoai ?? "",
                Email = "", // bảng DonHangs không có Email
                DiaChi = order.DiaChi ?? "",
                TrangThaiText = "Không xác định", // hiện tại không dùng cột TrangThai
                TongTien = order.TongTien,
                CreatedAt = order.NgayTao,
                Items = items
            };

            return View(vm);
        }
    }
}
