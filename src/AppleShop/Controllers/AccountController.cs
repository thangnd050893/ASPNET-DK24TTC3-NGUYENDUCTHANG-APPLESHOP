using System.Security.Claims;
using System.Security.Cryptography;
using AppleShop.Models;
using AppleShop.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AccountController : Controller
{
    private readonly AppleShopContext _db;
    public AccountController(AppleShopContext db) => _db = db;

    // ====== REGISTER (mở tự do để vào chắc chắn) ======
    [AllowAnonymous]
    [HttpGet("/account/register")]
    public IActionResult Register() => View(new AppleShop.Models.ViewModels.RegisterVM());

    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [HttpPost("/account/register")]
    public async Task<IActionResult> Register(AppleShop.Models.ViewModels.RegisterVM vm)
    {
        if (!ModelState.IsValid) return View(vm);

        // chặn trùng username
        if (await _db.NguoiDungQuanTris.AnyAsync(x => x.TenDangNhap == vm.UserName))
        {
            ModelState.AddModelError(nameof(vm.UserName), "Tên đăng nhập đã tồn tại.");
            return View(vm);
        }

        CreatePasswordHash(vm.Password, out var hash, out var salt);
        _db.NguoiDungQuanTris.Add(new NguoiDungQuanTri
        {
            TenDangNhap = vm.UserName,
            HoTen = vm.FullName,
            MatKhauHash = hash,
            MatKhauSalt = salt,
            VaiTro = "QuanTriVien",
            TrangThai = true
        });
        await _db.SaveChangesAsync();

        TempData["msg"] = "Tạo tài khoản quản trị thành công. Hãy đăng nhập.";
        return RedirectToAction(nameof(Login));
    }


    // =========== ĐĂNG NHẬP ===========
    [HttpGet("/account/login")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginVM());
    }

    [ValidateAntiForgeryToken, HttpPost("/account/login")]
    public async Task<IActionResult> Login(LoginVM vm, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(vm);

        var u = await _db.NguoiDungQuanTris.FirstOrDefaultAsync(x => x.TenDangNhap == vm.UserName && x.TrangThai);
        if (u == null || !VerifyPassword(vm.Password, u.MatKhauHash, u.MatKhauSalt))
        {
            ModelState.AddModelError(string.Empty, "Sai tài khoản hoặc mật khẩu.");
            return View(vm);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, u.MaNguoiDung.ToString()),
            new Claim(ClaimTypes.Name, u.TenDangNhap),
            new Claim(ClaimTypes.Role, u.VaiTro),
            new Claim("HoTen", u.HoTen ?? "")
        };

        var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id),
            new AuthenticationProperties { IsPersistent = vm.Remember });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Admin");
    }

    // =========== ĐĂNG XUẤT ===========
    [Authorize, HttpPost("/account/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet("/account/denied")]
    public IActionResult Denied() => Content("Không có quyền truy cập.");

    // =========== Helpers ===========
    private static void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(16);
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        hash = pbkdf2.GetBytes(32);
    }
    private static bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var test = pbkdf2.GetBytes(32);
        return CryptographicOperations.FixedTimeEquals(hash, test);
    }
}
