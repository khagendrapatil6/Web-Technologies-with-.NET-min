var builder = WebApplication.CreateBuilder(args);

// Listen on HTTP 5000 and HTTPS 5001 (terminal and VS Code behave the same).
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

// MVC (controllers + Razor views)
builder.Services.AddControllersWithViews();

// MySQL data layer
builder.Services.AddSingleton<CollegeFestMVC.Data.ParticipantRepository>();

// Tell the HTTPS-redirection middleware which port to redirect to.
builder.Services.AddHttpsRedirection(options => options.HttpsPort = 5001);

var app = builder.Build();

// Create the database + tables + seed events (needs MySQL running and a valid appsettings.json).
await CollegeFestMVC.Data.DbInitializer.InitializeAsync(
    builder.Configuration.GetConnectionString("FestDb")!);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Fest/Welcome");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   // serves wwwroot (css, js)
app.UseRouting();
app.UseAuthorization();

// Default route -> Fest/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Fest}/{action=Index}/{id?}");

app.Run();
