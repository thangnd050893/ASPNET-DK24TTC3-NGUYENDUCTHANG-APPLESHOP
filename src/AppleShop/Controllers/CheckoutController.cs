using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppleShop.Models;
using AppleShop.Models.ViewModels;
using AppleShop.Utils;            // SessionExtensions (GetObject/SetObject)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppleShopContext _db;
        public CheckoutController(AppleShopContext db) => _db = db;

        // Lấy giỏ hàng kiểu CartVM từ Session
        private CartVM GetCart() =>
            HttpContext.Session.GetObject<CartVM>("CART") ?? new CartVM();

        // GET /checkout
        [HttpGet("/checkout")]
        public IActionResult Index()
        {
            var cart = GetCart();

            if (cart.Items == null || !cart.Items.Any())
                return Redirect("/cart");

            var vm = new CheckoutVM
            {
                Items = cart.Items,
                SubTotal = cart.TongTien
            };
            vm.Shipping = 0;
            vm.Total = vm.SubTotal + vm.Shipping;

            return View(vm);
        }

        // POST /checkout/place-order
        [HttpPost("/checkout/place-order")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutVM model)
        {
            var cart = GetCart();

            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                TempData["OrderError"] = "Giỏ hàng đang trống.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                model.Items = cart.Items;
                model.SubTotal = cart.TongTien;
                model.Shipping = 0;
                model.Total = model.SubTotal + model.Shipping;
                return View("Index", model);
            }

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var order = new DonHang
                {
                    HoTen = model.HoTen,
                    DienThoai = model.DienThoai,
                    DiaChi = model.DiaChi,
                    GhiChu = model.GhiChu,
                    PhuongThucThanhToan = model.PhuongThucThanhToan,

                    TongTien = cart.TongTien,
                    NgayTao = DateTime.Now,
                    MaDon = $"DH{DateTime.Now:yyyyMMddHHmmssfff}",
                    ChiTietDonHangs = new List<ChiTietDonHang>()
                };

                foreach (var i in cart.Items)
                {
                    order.ChiTietDonHangs.Add(new ChiTietDonHang
                    {
                        SanPhamId = i.ProductId,
                        SoLuong = i.SoLuong,
                        DonGia = i.GiaBan
                    });
                }

                _db.DonHangs.Add(order);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                // Xoá giỏ hàng
                HttpContext.Session.SetObject("CART", new CartVM());

                // Redirect bằng DonHangId (int) -> tránh lỗi ép kiểu
                return RedirectToAction(nameof(Success), new { id = order.DonHangId });
            }
            catch
            {
                await tx.RollbackAsync();

                TempData["OrderError"] = "Có lỗi khi lưu đơn hàng. Vui lòng thử lại!";

                model.Items = cart.Items;
                model.SubTotal = cart.TongTien;
                model.Shipping = 0;
                model.Total = model.SubTotal + model.Shipping;
                return View("Index", model);
            }
        }

        // GET /checkout/success/{id}
        // Truy theo DonHangId (int) để khớp DB -> không còn InvalidCastException
        [HttpGet("/checkout/success/{id:int}")]
        public async Task<IActionResult> Success(int id)
        {
            var order = await _db.DonHangs
                .Include(x => x.ChiTietDonHangs)
                .ThenInclude(x => x.SanPham)
                .FirstOrDefaultAsync(x => x.DonHangId == id);

            if (order == null)
                return RedirectToAction(nameof(Index));

            return View(order);
        }
    }
}