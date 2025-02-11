

using Microsoft.EntityFrameworkCore;
using ShopBaker.Controllers.Data;
using ShopBaker.Session;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpContextAccessor and DbContext.
builder.Services.AddHttpContextAccessor(); // Đăng ký IHttpContextAccessor
builder.Services.AddScoped<SessionCustomer>(); // Đăng ký SessionCustomer

//// Define connection string.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // You should handle the case where the connection string is not found.
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Register the DbContext with the SQL Server provider.
builder.Services.AddDbContext<ShopBakerDbContext>(options =>
    options.UseSqlServer(connectionString));

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

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

