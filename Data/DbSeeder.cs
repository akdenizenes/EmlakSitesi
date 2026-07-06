using EmlakSitesi.Models.Entities;

namespace EmlakSitesi.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // İlk admin kullanıcısı (yoksa oluştur)
        if (!db.Users.Any())
        {
            db.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                FullName = "Site Yöneticisi"
            });
        }

        // Hakkımızda başlangıç kaydı
        if (!db.Abouts.Any())
        {
            db.Abouts.Add(new About
            {
                CompanyDescription = "Firma açıklaması buraya gelecek.",
                Mission = "Misyonumuz buraya gelecek.",
                Vision = "Vizyonumuz buraya gelecek.",
                FoundedYear = 2015
            });
        }

        // İletişim ayarları başlangıç kayıtları
        string[] keys = { "Phone", "WhatsApp", "Email", "Instagram", "Facebook", "Address", "MapEmbedUrl" };
        foreach (var key in keys)
        {
            if (!db.Settings.Any(s => s.Key == key))
                db.Settings.Add(new Setting { Key = key, Value = "" });
        }

        db.SaveChanges();
    }
}