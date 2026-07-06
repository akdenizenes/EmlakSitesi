using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using EmlakSitesi.Data;
using EmlakSitesi.Models.Entities;

namespace EmlakSitesi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DistrictController : Controller
    {
        private readonly AppDbContext _context;

        public DistrictController(AppDbContext context)
        {
            _context = context;
        }

        // İlçeleri Listeleme Ekranı
        public IActionResult Index()
        {
            // Şehirleri de Include ile dahil ediyoruz ki tabloda şehir adı da yazsın
            var districts = _context.Districts.Include(d => d.City).ToList();
            return View(districts);
        }

        // Yeni İlçe Ekleme Ekranını Açma
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Cities = new SelectList(_context.Cities.ToList(), "Id", "Name");
            return View();
        }

        // Formdan Gelen İlçeyi Veritabanına Kaydetme
        [HttpPost]
        public IActionResult Create(District district)
        {
            // Entity Framework'ün ilişkisel modeli (City) zorunlu tutmasını engelliyoruz
            ModelState.Remove("City");

            if (ModelState.IsValid)
            {
                _context.Districts.Add(district);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            
            // Hata olursa şehri tekrar doldur sayfayı geri ver
            ViewBag.Cities = new SelectList(_context.Cities.ToList(), "Id", "Name", district.CityId);
            return View(district);
        }

        // İlçe Silme
        public IActionResult Delete(int id)
        {
            var district = _context.Districts.FirstOrDefault(x => x.Id == id);
            if (district != null)
            {
                _context.Districts.Remove(district);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}