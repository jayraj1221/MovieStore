using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MovieStore.Models;
using MovieStore.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// ✅ Authentication Service Registration
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect if not logged in
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect if no permission
    });

// ✅ Session Setup
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ DbContext and Repositories
var connectionString = builder.Configuration.GetConnectionString("MovieStoreDatabase");
builder.Services.AddDbContext<MovieStoreContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUser, SUser>();
builder.Services.AddScoped<IMovie, MovieRepository>();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "user",
    pattern: "User/{action=Index}/{id?}",
    defaults: new { controller = "User" });

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Index}/",
    defaults: new { controller = "Admin" });

app.Run();
