using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppleShop.Models;
using AppleShop.Models.ViewModels;
using AppleShop.Utils;
using System.Linq; // Cần thêm để sử dụng Linq

namespace AppleShop.Controllers
{
    public class CheckoutController : Controller
    {
        // Dùng hằng số key cho Session (nên thống nhất giữa các Controller)
        private const string CART_KEY = "CART";

        private readonly AppleShopContext _db;
        public CheckoutController(AppleShopContext db) => _db = db;

        // GET /checkout
        [HttpGet("/checkout")]
        public IActionResult Index()
        {
            // FIX LỖI QUAN TRỌNG NHẤT: Đọc đối tượng CartVM từ Session (Đúng kiểu dữ liệu đã lưu)
            var cartVM = HttpContext.Session.GetObject<CartVM>(CART_KEY)
                       ?? new CartVM();

            var cartItems = cartVM.Items; // Lấy danh sách sản phẩm từ CartVM

            // Nếu giỏ rỗng → về trang giỏ (Điều kiện này bây giờ đã hoạt động đúng)
            if (cartItems.Count == 0)
                return RedirectToAction("Index", "Cart");

            // Khởi tạo CheckoutVM
            var vm = new CheckoutVM
            {
                Items = cartItems,
                Shipping = 0 // Giả sử phí ship = 0 hoặc đã được tính
            };

            // Tính toán SubTotal và Total cho ViewModel
            // Dùng thuộc tính TongTien của CartVM nếu đã được tính sẵn, hoặc tính lại từ Items
            vm.SubTotal = cartItems.Sum(x => x.ThanhTien);
            vm.Total = vm.SubTotal + vm.Shipping;

            return View(vm);
        }

        // POST /checkout/place-order
        [ValidateAntiForgeryToken]
        [HttpPost("/checkout/place-order")]
        public async Task<IActionResult> PlaceOrder(CheckoutVM model)
        {
            // FIX: Lấy lại giỏ từ session dưới dạng CartVM
            var cartVM = HttpContext.Session.GetObject<CartVM>(CART_KEY) ?? new CartVM();
            var cartItems = cartVM.Items;

            if (cartItems.Count == 0)
            {
                TempData["OrderError"] = "Giỏ hàng rỗng.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                // Tính lại tổng để view render được
                model.Items = cartItems;
                model.SubTotal = cartItems.Sum(x => x.ThanhTien);
                model.Total = model.SubTotal + model.Shipping;
                return View("Index", model);
            }

            // ===== TẠO ĐƠN HÀNG =====
            var order = new DonHang
            {
                HoTen = model.HoTen,
                DienThoai = model.DienThoai,
                DiaChi = model.DiaChi,
                GhiChu = model.GhiChu,
                PhuongThucThanhToan = model.PhuongThucThanhToan,

                NgayTao = DateTime.Now,

                // Sử dụng tổng tiền đã tính hoặc tính lại từ cartItems
                TongTien = cartItems.Sum(x => x.ThanhTien),

                ChiTietDonHangs = new List<ChiTietDonHang>()
            };

            foreach (var i in cartItems)
            {
                order.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    SanPhamId = i.ProductId,
                    SoLuong = i.SoLuong,
                    DonGia = i.GiaBan // Đã được sửa để dùng GiaBan thay vì DonGia nếu cần
                });
            }

            _db.DonHangs.Add(order);
            await _db.SaveChangesAsync();

            // FIX: Xoá giỏ bằng cách lưu một CartVM MỚI (rỗng)
            HttpContext.Session.SetObject(CART_KEY, new CartVM());

            TempData["OrderSuccess"] = $"Đặt hàng thành công! Mã đơn: {order.DonHangId}";
            return RedirectToAction(nameof(Success));
        }

        // GET /checkout/success
        [HttpGet("/checkout/success")]
        public IActionResult Success()
        {
            ViewBag.Message = TempData["OrderSuccess"] as string;
            return View();
        }
    }
}