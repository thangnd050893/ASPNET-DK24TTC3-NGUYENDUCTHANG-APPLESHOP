using AppleShop.Models;
using AppleShop.Models.ViewModels;
using AppleShop.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AppleShop.Controllers
{
    public class CartController : Controller
    {
        private const string CART_KEY = "CART";
        private readonly AppleShopContext _db;
        public CartController(AppleShopContext db) => _db = db;

        // ===================== Helpers =====================
        private CartVM GetCart()
            => HttpContext.Session.GetObject<CartVM>(CART_KEY) ?? new CartVM();

        private void SaveCart(CartVM cart)
            => HttpContext.Session.SetObject(CART_KEY, cart);

        // ===================== Trang giỏ hàng =====================
        [HttpGet("/cart")]
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // ===================== Thêm sản phẩm =====================
        [HttpPost("/cart/add/{id:int}")]
        public async Task<IActionResult> Add(int id, int qty = 1)
        {
            if (qty < 1) qty = 1;

            // Lấy thông tin sản phẩm
            var p = await _db.SanPhams
                .AsNoTracking()
                .Where(x => x.SanPhamId == id)
                .Select(x => new
                {
                    x.SanPhamId,
                    x.Ten,
                    x.HinhAnh,
                    x.GiaBan
                })
                .FirstOrDefaultAsync();

            if (p == null)
                return NotFound("Không tìm thấy sản phẩm.");

            // Lấy giỏ hiện tại từ Session
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == id);

            if (item == null)
            {
                cart.Items.Add(new CartItemVM
                {
                    ProductId = p.SanPhamId,
                    Ten = p.Ten,
                    HinhAnh = p.HinhAnh,
                    GiaBan = p.GiaBan,
                    SoLuong = qty
                });
            }
            else
            {
                item.SoLuong += qty;
            }

            SaveCart(cart);

            // Nếu request đến từ AJAX, trả JSON về cho badge
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, count = cart.Items.Sum(x => x.SoLuong) });

            // Nếu không thì quay lại trang trước
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
                return Redirect(referer);

            return RedirectToAction("Index");
        }

        // ===================== Cập nhật số lượng =====================
        [HttpPost("/cart/update/{id:int}")]
        public IActionResult Update(int id, int qty = 1)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == id);

            if (item != null)
            {
                item.SoLuong = Math.Max(1, qty);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }

        // ===================== Xoá sản phẩm =====================
        [HttpPost("/cart/remove/{id:int}")]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            cart.Items.RemoveAll(i => i.ProductId == id);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ===================== Xoá toàn bộ giỏ =====================
        [HttpPost("/cart/clear")]
        public IActionResult Clear()
        {
            SaveCart(new CartVM());
            return RedirectToAction("Index");
        }

        // ===================== Đếm sản phẩm trong giỏ (cho badge 🛒) =====================
        [HttpGet("/cart/count")]
        public IActionResult Count()
        {
            var cart = GetCart();
            var total = cart.Items.Sum(x => x.SoLuong);
            return Content(total.ToString());
        }
    }
}
