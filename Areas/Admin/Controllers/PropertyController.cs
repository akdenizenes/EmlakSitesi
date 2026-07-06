using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using EmlakSitesi.Data;
using EmlakSitesi.Models.Entities;

namespace EmlakSitesi.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PropertyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PropertyController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var properties = _context.Properties.ToList();
            return View(properties);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Cities = new SelectList(_context.Cities.ToList(), "Id", "Name");
            return View();
        }

        [HttpGet]
        public IActionResult GetDistricts(int cityId)
        {
            var districts = _context.Districts
                                    .Where(d => d.CityId == cityId)
                                    .Select(d => new { id = d.Id, name = d.Name })
                                    .ToList();
            return Json(districts);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Property property, List<IFormFile> imageFiles)
        {
            ModelState.Remove("Slug");
            ModelState.Remove("City");
            ModelState.Remove("District");
            ModelState.Remove("Images");

            IActionResult ReturnWithFilledDropdowns()
            {
                ViewBag.Cities = new SelectList(_context.Cities.ToList(), "Id", "Name", property.CityId);
                if (property.CityId > 0)
                {
                    ViewBag.Districts = new SelectList(_context.Districts.Where(d => d.CityId == property.CityId).ToList(), "Id", "Name", property.DistrictId);
                }
                return View(property);
            }

            // AYNI BAŞLIK KONTROLÜ
            bool isTitleExist = await _context.Properties.AnyAsync(p => p.Title == property.Title);
            if (isTitleExist)
            {
                ModelState.AddModelError("Title", "Bu başlıkta zaten bir ilan mevcut! Lütfen farklı bir başlık girin.");
                return ReturnWithFilledDropdowns();
            }

            if (!ModelState.IsValid) return ReturnWithFilledDropdowns();

            if (imageFiles == null || !imageFiles.Any() || imageFiles.All(f => f.Length == 0))
            {
                ModelState.AddModelError("imageFiles", "Lütfen en az bir adet fotoğraf seçin.");
                return ReturnWithFilledDropdowns();
            }

            try
            {
                property.CreatedAt = DateTime.UtcNow;
                property.IsActive = true;

                if (string.IsNullOrEmpty(property.Slug) && !string.IsNullOrEmpty(property.Title))
                {
                    property.Slug = property.Title.ToLower()
                        .Replace(" ", "-").Replace("ç", "c").Replace("ş", "s")
                        .Replace("ı", "i").Replace("ğ", "g").Replace("ö", "o").Replace("ü", "u");
                }

                _context.Properties.Add(property);
                await _context.SaveChangesAsync();

                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "properties");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                int sortOrder = 0;
                foreach (var file in imageFiles)
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        sortOrder++;
                        _context.Set<PropertyImage>().Add(new PropertyImage
                        {
                            PropertyId = property.Id,
                            FilePath = "/images/properties/" + uniqueFileName,
                            SortOrder = sortOrder
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Bir hata oluştu, lütfen tekrar deneyin.");
                return ReturnWithFilledDropdowns();
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var property = _context.Properties.FirstOrDefault(x => x.Id == id);
            if (property == null) return NotFound();

            ViewBag.Cities = new SelectList(_context.Cities.ToList(), "Id", "Name", property.CityId);
            ViewBag.Districts = new SelectList(_context.Districts.Where(d => d.CityId == property.CityId).ToList(), "Id", "Name", property.DistrictId);
            return View(property);
        }

        [HttpPost]
        public IActionResult Edit(Property property)
        {
            ModelState.Remove("Slug");
            ModelState.Remove("City");
            ModelState.Remove("District");
            ModelState.Remove("Images");

            if (ModelState.IsValid)
            {
                _context.Properties.Update(property);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Cities = new SelectList(_context.Cities.ToList(), "Id", "Name", property.CityId);
            ViewBag.Districts = new SelectList(_context.Districts.Where(d => d.CityId == property.CityId).ToList(), "Id", "Name", property.DistrictId);
            return View(property);
        }

        public IActionResult Delete(int id)
        {
            var property = _context.Properties.FirstOrDefault(x => x.Id == id);
            if (property != null)
            {
                _context.Properties.Remove(property);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Images(int id)
        {
            var property = _context.Properties.Include(p => p.Images).FirstOrDefault(p => p.Id == id);
            return property == null ? NotFound() : View(property);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(int propertyId, List<IFormFile> files)
        {
            if (files != null && files.Any())
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "properties");
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        using (var stream = new FileStream(Path.Combine(uploadsFolder, uniqueFileName), FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        _context.Set<PropertyImage>().Add(new PropertyImage { PropertyId = propertyId, FilePath = "/images/properties/" + uniqueFileName });
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Images", new { id = propertyId });
        }
    }
}