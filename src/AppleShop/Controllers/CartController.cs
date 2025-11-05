using AppleShop.Models;
using AppleShop.Models.ViewModels;
using AppleShop.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppleShop.Controllers
{
    public class CartController : Controller
    {
        private const string CART_KEY = "CART";
        private readonly AppleShopContext _db;
        public CartController(AppleShopContext db) => _db = db;

        // Helpers ------------------------------
        private CartVM GetCart()
            => HttpContext.Session.GetObject<CartVM>(CART_KEY) ?? new CartVM();

        private void SaveCart(CartVM cart)
            => HttpContext.Session.SetObject(CART_KEY, cart);

        // GET /cart ----------------------------
        [HttpGet("/cart")]
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // POST /cart/add/5?qty=1 ---------------
        [HttpPost("/cart/add/{id:int}")]
        public async Task<IActionResult> Add(int id, int qty = 1)
        {
            if (qty < 1) qty = 1;

            var p = await _db.SanPhams.AsNoTracking()
                .Where(x => x.SanPhamId == id)
                .Select(x => new { x.SanPhamId, x.Ten, x.HinhAnh, x.GiaBan })
                .FirstOrDefaultAsync();

            if (p == null) return NotFound();

            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == id);

            if (item == null)
            {
                cart.Items.Add(new CartItemVM
                {
                    ProductId = p.SanPhamId,
                    Ten = p.Ten,
                    HinhAnh = p.HinhAnh,
                    DonGia = p.GiaBan,  // nếu DB là 'Gia' thì đổi p.Gia ?? 0
                    SoLuong = qty
                });
            }
            else
            {
                item.SoLuong += qty;
            }

            SaveCart(cart);

            // Quay lại trang trước nếu có; mặc định về /cart
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer)) return Redirect(referer);
            return RedirectToAction("Index");
        }

        // POST /cart/update/5 ------------------
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

        // POST /cart/remove/5 ------------------
        [HttpPost("/cart/remove/{id:int}")]
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            cart.Items.RemoveAll(i => i.ProductId == id);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // GET /cart/count  (cho badge giỏ hàng) -
        [HttpGet("/cart/count")]
        public IActionResult Count()
        {
            var cart = GetCart();
            var total = cart.Items.Sum(x => x.SoLuong);
            return Content(total.ToString());
        }

        // POST /cart/clear ---------------------
        [HttpPost("/cart/clear")]
        public IActionResult Clear()
        {
            SaveCart(new CartVM());
            return RedirectToAction("Index");
        }
    }
}
