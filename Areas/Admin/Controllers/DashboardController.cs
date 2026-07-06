using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmlakSitesi.Data;

namespace EmlakSitesi.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        ViewBag.PropertyCount = _db.Properties.Count();
        ViewBag.ActiveCount = _db.Properties.Count(p => p.IsActive);
        return View();
    }
}