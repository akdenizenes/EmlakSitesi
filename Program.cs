using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using EmlakSitesi.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// MySQL veritabanı bağlantısı
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseMySql(conn, ServerVersion.AutoDetect(conn)));

// Cookie ile admin giriş sistemi
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        // Yolları senin AccountController içindeki [HttpGet("/admin/login")] rotana göre ayarladık!
        opt.LoginPath = "/admin/login";
        opt.AccessDeniedPath = "/admin/login";
        
        // Tarayıcıdaki çerezin adı ve süresi
        opt.Cookie.Name = "EmlakSitesiAdmin";
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

// Uygulama açılırken seed çalıştır
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.Seed(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// GÜVENLİK KATMANLARI (Sıralaması motorun çalışması için hayati önem taşır)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();