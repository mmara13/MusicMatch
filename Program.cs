using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();
Console.WriteLine("Connection String: " + connectionString);

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();


//builder.Services.AddIdentityCore<ApplicationUser>()
//                .AddUserManager<UserManager<ApplicationUser>>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<IRazorViewEngine, RazorViewEngine>();
builder.Services.AddControllersWithViews();
var app = builder.Build();

// PASUL 5 - useri si roluri

// CreateScope ofera acces la instanta curenta a aplicatiei
// variabila scope are un Service Provider folosit pentru a injecta dependentele 
// in aplicatie -> bd, cookie, sesiune, autentif, pachete, etc

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initialize(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map SignalR Hub and other endpoints
app.UseEndpoints(endpoints =>
{
    // SignalR hub mapping

    endpoints.MapHub<MusicMatch.Hubs.ChatHub>("/Chathub");


    // Default routes for controllers and Razor Pages
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});


// Custom redirection logic for the root URL
app.MapGet("/", async context =>
{
    if (context.User.Identity.IsAuthenticated)
    {
        // Get the logged-in user
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        // If userId is found, redirect to the user's profile page
        if (userId != null)
        {
            context.Response.Redirect($"/Profiles/Details/{userId}");
        }
        else
        {
            // If userId is not found (should not happen), redirect to home or another page
            context.Response.Redirect("/Home/Index");
        }
    }
    else
    {
        // If user is not authenticated, redirect to the register page
        context.Response.Redirect("/Identity/Account/Register");
    }
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

//create directory for profile photos if it doesn't exist
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.Run();
