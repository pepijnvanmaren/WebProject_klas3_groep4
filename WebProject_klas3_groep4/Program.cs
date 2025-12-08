using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------
// Services
// ----------------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// ----------------------------------------------------------
// Database
// ----------------------------------------------------------
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=WebProject_klas3_groep4;Trusted_Connection=True;")
);

// ----------------------------------------------------------
// Identity
// ----------------------------------------------------------
builder.Services.AddIdentity<GebruikerDB, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<DatabaseContext>()
.AddDefaultTokenProviders();

// Dummy email sender
builder.Services.AddTransient<IEmailSender<GebruikerDB>, DummyEmailSender>();

// ----------------------------------------------------------
// Authentication — Cookies
// ----------------------------------------------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/auth/login";
    options.LogoutPath = "/api/auth/logout";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
});

// ----------------------------------------------------------
// CORS
// ----------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // React frontend
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ----------------------------------------------------------
// Swagger
// ----------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebProject API",
        Version = "v1"
    });

    // DateOnly / TimeOnly support
    options.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
    options.MapType<TimeOnly>(() => new OpenApiSchema { Type = "string", Format = "time" });
});

// ----------------------------------------------------------
// Build app
// ----------------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

// ----------------------------------------------------------
// Identity API Endpoints (login/register)
app.MapIdentityApi<GebruikerDB>();

// ----------------------------------------------------------
// Role Seeding - GECORRIGEERD
// ----------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    string[] roles = { "Admin", "Koper", "Aanvoerder", "Veilingmeester" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(role));
        }
    }
}

// ----------------------------------------------------------
// Admin User Seeding
// ----------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<GebruikerDB>>();
    string adminPassword = "Admin123!"; // In secrets.json zetten!

    var existingUser = await userManager.FindByNameAsync("adminUser");
    if (existingUser == null)
    {
        var adminUser = new GebruikerDB
        {
            UserName = "adminUser",
            Email = "admin@example.com",
            EmailConfirmed = true,
            Rol = "Admin"  // NIEUW: Zet de rol op Admin
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            foreach (var error in createResult.Errors)
                Console.WriteLine($"[ADMIN ERROR] {error.Description}");
        }
    }
}

// ----------------------------------------------------------
// Controllers
// ----------------------------------------------------------
app.MapControllers();
app.Run();

// Voor integratie tests
public partial class Program { }

// ----------------------------------------------------------
// Dummy Email Sender
// ----------------------------------------------------------
public class DummyEmailSender : IEmailSender<GebruikerDB>
{
    public Task SendConfirmationLinkAsync(GebruikerDB user, string email, string link) => Task.CompletedTask;
    public Task SendPasswordResetLinkAsync(GebruikerDB user, string email, string link) => Task.CompletedTask;
    public Task SendPasswordResetCodeAsync(GebruikerDB user, string email, string code) => Task.CompletedTask;
}