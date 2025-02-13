using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MusicMatch.Data;
using MusicMatch.Hubs;
using MusicMatch.Models;
using MusicMatch.Services;
using WebOptimizer; //to improve performance using lighthouse analysis

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



builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddSingleton<IRazorViewEngine, RazorViewEngine>();
builder.Services.AddControllersWithViews();

//services for notifications
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<IMyEmailSender, EmailService>();
builder.Services.AddSignalR();
builder.Services.AddScoped<ReportService>();


//services to improve performance using lighthouse analysis
builder.Services.AddControllersWithViews();
builder.Services.AddWebOptimizer();

builder.Services.AddControllersWithViews();
// bundling service for CSS files for optimization purposes
builder.Services.AddWebOptimizer(pipeline =>
{
    // for css
    pipeline.AddCssBundle("/css/bundle.css",
        "css/site.css",
        "css/events.css",
        "css/chatroom.css",
        "css/artist.css",
        "css/eventDetails.css"
    );
});
var app = builder.Build();


// middleware for performance optimization
app.UseWebOptimizer();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
            "Cache-Control", "public,max-age=604800"
        );
    }
});


app.MapHub<NotificationHub>("/notificationHub");

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
    //app.UseMigrationsEndPoint();
    app.UseDeveloperExceptionPage();
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
/*app.MapGet("/", async context =>
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
});*/


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


/*-------------------------------*/
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller=Events}/{action=GetEvents}/{id?}");
/*-------------------------------*/

//create directory for profile photos if it doesn't exist
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.Run();
