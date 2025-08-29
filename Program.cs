using Microsoft.EntityFrameworkCore;
using Project2.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//var connectionString = builder.Configuration.GetConnectionString("WebBanDoAnNhanh");
//builder.Services.AddDbContext<WebBanDoAnNhanhContext>(x => x.UseSqlServer(connectionString));


// Cấu hình Database
builder.Services.AddDbContext<WebBanDoAnNhanhContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Web_ban_do_an_nhanh")));

// ⭐ QUAN TRỌNG: Cấu hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
