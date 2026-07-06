using Microsoft.AspNetCore.Mvc;
using System.Linq;
using EmlakSitesi.Data; 
using EmlakSitesi.Models.Entities; 

namespace EmlakSitesi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CityController : Controller
    {
        private readonly AppDbContext _context;

        public CityController(AppDbContext context)
        {
            _context = context;
        }

        // Şehirleri Listeleme
        public IActionResult Index()
        {
            var cities = _context.Cities.ToList();
            return View(cities);
        }

        // Yeni Şehir Ekleme Sayfası
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Yeni Şehri Kaydetme
        [HttpPost]
        public IActionResult Create(City city)
        {
            if (ModelState.IsValid)
            {
                _context.Cities.Add(city);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(city);
        }

        // Şehir Silme
        public IActionResult Delete(int id)
        {
            var city = _context.Cities.FirstOrDefault(x => x.Id == id);
            if (city != null)
            {
                _context.Cities.Remove(city);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}