using FinancialKDS.Data;
using FinancialKDS.Models;
using FinancialKDS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext Tanýmlamasý
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity Servislerinin Tanýmlanmasý
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Þifre Kurallarý
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie Ayarlarý ve Oturum Yönlendirmeleri
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";

    // Uygulama kapandýðýnda oturumun tarayýcý çerezinde kalmayýp her açýlýþta tekrar giriþ istenmesini saðlar
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<FinancialAnalysisService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Kimlik Doðrulama ve Yetkilendirme Middleware
app.UseAuthentication();
app.UseAuthorization();

// Varsayýlan Rota (Default Route): Proje ilk açýldýðýnda Account/Login sayfasýna yönlendirir
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();