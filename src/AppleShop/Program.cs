using AppleShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; 

var builder = WebApplication.CreateBuilder(args);

// Dịch vụ MVC
builder.Services.AddControllersWithViews();

// Kết nối CSDL (EF Core + SQL Server)
builder.Services.AddDbContext<AppleShopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppleShopDb")));

// Bật Session
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".AppleShop.Cart";
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
});


// Thêm cấu hình Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/account/login";          // trang đăng nhập
        opt.LogoutPath = "/account/logout";        // đăng xuất
        opt.AccessDeniedPath = "/account/denied";  // nếu bị từ chối quyền
        opt.ExpireTimeSpan = TimeSpan.FromDays(7); // hết hạn cookie
        opt.SlidingExpiration = true;              // tự động gia hạn nếu còn hoạt động
    });

builder.Services.AddAuthorization(); // Cho phép dùng [Authorize]


var app = builder.Build();

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();

// Thêm middleware xác thực vào pipeline
app.UseAuthentication();
app.UseAuthorization();

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
