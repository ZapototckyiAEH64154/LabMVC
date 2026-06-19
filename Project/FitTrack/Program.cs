using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FitTrack.Data;
using FitTrack.Models;

var builder = WebApplication.CreateBuilder(args);

// Baza danych (SQLite) — rejestracja kontekstu EF Core.
builder.Services.AddDbContext<FitTrackContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("FitTrackContext")
        ?? "Data Source=fittrack.db"));

// Uwierzytelnianie oparte na ciasteczku/sesji (prosty system logowania).
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

// MVC — kontrolery z widokami.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Utworzenie bazy danych i wczytanie przykładowych danych przy starcie.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Konfiguracja potoku żądań HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Workouts}/{action=Index}/{id?}");

app.Run();
