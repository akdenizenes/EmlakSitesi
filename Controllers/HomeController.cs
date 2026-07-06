using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmlakSitesi.Data;
using EmlakSitesi.Helpers;
using EmlakSitesi.Models.Entities;
using System.Linq;

namespace EmlakSitesi.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // Ana Sayfa Vitrini ve Arama Motoru
        public IActionResult Index(string query)
        {
            var propertiesQuery = _context.Properties
                .Include(p => p.Images)
                .Include(p => p.City)
                .Include(p => p.District)
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(query))
            {
                var loweredQuery = query.ToLower();
                propertiesQuery = propertiesQuery.Where(p =>
                    p.Title.ToLower().Contains(loweredQuery) ||
                    (p.City != null && p.City.Name.ToLower().Contains(loweredQuery)) ||
                    (p.District != null && p.District.Name.ToLower().Contains(loweredQuery)) ||
                    p.Neighborhood.ToLower().Contains(loweredQuery)
                );
            }

            var properties = propertiesQuery.OrderByDescending(p => p.CreatedAt).ToList();

            ViewBag.SearchQuery = query;

            return View(properties);
        }

        // İlan Detay Sayfası
        public IActionResult Details(int id)
        {
            var property = _context.Properties
                .Include(p => p.Images)
                .Include(p => p.City)
                .Include(p => p.District)
                .FirstOrDefault(p => p.Id == id && p.IsActive);

            if (property == null)
            {
                return NotFound();
            }

            return View(property);
        }

        // Kategori Sayfası (Satılık / Kiralık Filtrelemesi + Sayfalama)
        public async Task<IActionResult> Category(string type, int page = 1)
        {
            if (page < 1) page = 1;
            int pageSize = 12;

            var propertiesQuery = _context.Properties
                .Include(p => p.Images)
                .Include(p => p.City)
                .Include(p => p.District)
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(type))
            {
                if (type.ToLower() == "satilik")
                {
                    propertiesQuery = propertiesQuery.Where(p => p.ListingType == ListingType.Satilik);
                    ViewBag.CategoryName = "Satılık İlanlar";
                }
                else if (type.ToLower() == "kiralik")
                {
                    propertiesQuery = propertiesQuery.Where(p => p.ListingType == ListingType.Kiralik);
                    ViewBag.CategoryName = "Kiralık İlanlar";
                }
            }
            else
            {
                ViewBag.CategoryName = "Tüm İlanlar";
            }

            var orderedQuery = propertiesQuery.OrderByDescending(p => p.CreatedAt);

            ViewBag.Type = type;

            var model = await PaginatedList<Property>.CreateAsync(orderedQuery, page, pageSize);
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}