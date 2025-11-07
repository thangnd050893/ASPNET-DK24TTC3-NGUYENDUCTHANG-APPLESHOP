using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "QuanTriVien")]
public class AdminController : Controller
{
    [HttpGet("/admin")]
    public IActionResult Index() => View();
}
