using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MovieStore.Models;
using MovieStore.Repository;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Security option
    options.Cookie.IsEssential = true; // Required for GDPR compliance
});


var connectionString = builder.Configuration.GetConnectionString("MovieStoreDatabase");

builder.Services.AddDbContext<MovieStoreContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUser, SUser>();
builder.Services.AddScoped<IMovie, MovieRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseSession();
app.UseStaticFiles();

app.UseRouting();

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
