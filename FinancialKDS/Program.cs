using FinancialKDS.Data;
using Microsoft.EntityFrameworkCore;
using FinancialKDS.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC servisleri
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<RegressionService>();
builder.Services.AddScoped<NpvService>();

// Veritabaný baðlantýsý
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Hata yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// MVC route ayarý
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Projects}/{action=Index}/{id?}");

app.Run();