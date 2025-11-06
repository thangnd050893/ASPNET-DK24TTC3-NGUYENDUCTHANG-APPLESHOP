using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppleShop.Models;
using AppleShop.Models.ViewModels;
using AppleShop.Utils;                 // SessionExtensions (GetObject/SetObject)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly AppleShopContext _db;
        public CheckoutController(AppleShopContext db) => _db = db;

        // =================================================================
        // SỬA 1: Thay đổi kiểu trả về từ List<CartItemVM> thành CartVM
        // =================================================================
        private CartVM GetCart() =>
            HttpContext.Session.GetObject<CartVM>("CART") ?? new CartVM();

        // GET /checkout
        [HttpGet("/checkout")]
        public IActionResult Index()
        {
            // 'cart' bây giờ là một đối tượng CartVM
            var cart = GetCart();

            // Nếu giỏ hàng không có sản phẩm, chuyển hướng về trang giỏ hàng
            if (cart.Items == null || !cart.Items.Any())
            {
                return Redirect("/cart");
            }

            // =================================================================
            // SỬA 2: Lấy Items từ cart.Items và tổng tiền từ cart.TongTien
            // =================================================================
            var vm = new CheckoutVM
            {
                Items = cart.Items,
                SubTotal = cart.TongTien, // Lấy tổng tiền đã tính toán từ CartVM
            };
            vm.Shipping = 0; // Bạn có thể thêm logic tính phí vận chuyển ở đây nếu cần
            vm.Total = vm.SubTotal + vm.Shipping;

            return View(vm);
        }

        // POST /checkout/place-order
        [HttpPost("/checkout/place-order")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutVM model)
        {
            // 'cart' là CartVM
            var cart = GetCart();

            // =================================================================
            // SỬA 3: Kiểm tra cart.Items (thay vì cart.Count)
            // =================================================================
            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                TempData["OrderError"] = "Giỏ hàng đang trống.";
                return RedirectToAction(nameof(Index));
            }

            // Nếu model không hợp lệ, trả về View với thông tin giỏ hàng
            if (!ModelState.IsValid)
            {
                // =================================================================
                // SỬA 4: Gán lại Items và TongTien từ CartVM
                // =================================================================
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

                    // =================================================================
                    // SỬA 5: Lấy tổng tiền trực tiếp từ cart.TongTien
                    // =================================================================
                    TongTien = cart.TongTien,

                    NgayTao = DateTime.Now,
                    MaDon = $"DH{DateTime.Now:yyyyMMddHHmmssfff}", // Tạo mã đơn duy nhất
                    ChiTietDonHangs = new List<ChiTietDonHang>()
                };

                // =================================================================
                // SỬA 6: Duyệt qua cart.Items (thay vì cart)
                // =================================================================
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

                // =================================================================
                // SỬA 7: Xoá giỏ hàng bằng cách lưu một CartVM rỗng
                // =================================================================
                HttpContext.Session.SetObject("CART", new CartVM());

                return RedirectToAction(nameof(Success), new { id = order.DonHangId });
            }
            catch
            {
                await tx.RollbackAsync();

                TempData["OrderError"] = "Có lỗi khi lưu đơn hàng. Vui lòng thử lại!";

                // Gán lại thông tin giỏ hàng nếu có lỗi
                model.Items = cart.Items;
                model.SubTotal = cart.TongTien;
                model.Shipping = 0;
                model.Total = model.SubTotal + model.Shipping;
                return View("Index", model);
            }
        }

        // GET /checkout/success/{id}
        [HttpGet("/checkout/success/{id:int}")]
        public async Task<IActionResult> Success(int id)
        {
            // Phần này không phụ thuộc vào session giỏ hàng nên đã đúng
            var order = await _db.DonHangs
                .Include(x => x.ChiTietDonHangs)
                .ThenInclude(x => x.SanPham)
                .FirstOrDefaultAsync(x => x.DonHangId == id);

            if (order == null)
                return RedirectToAction(nameof(Index));

            return View(order); // model = DonHang
        }
    }
}