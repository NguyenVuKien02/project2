using Microsoft.EntityFrameworkCore;
using Project2.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework
builder.Services.AddDbContext<WebBanDoAnNhanhContext>(options =>
    options.UseSqlServer("Server=LAPTOP-KNT8DC45;Database=Web_ban_do_an_nhanh;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True"));

// Add Session support - QUAN TRỌNG!
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".AspNetCore.Session";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// QUAN TRỌNG: UseSession() phải được gọi TRƯỚC UseAuthorization()
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SanPham}/{action=Index}/{id?}");

app.Run();