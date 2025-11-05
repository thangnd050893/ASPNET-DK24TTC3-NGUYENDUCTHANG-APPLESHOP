using AppleShop.Models;
using Microsoft.EntityFrameworkCore;

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
app.UseAuthorization();

// Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
