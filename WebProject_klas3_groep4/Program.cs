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

// Dummy email sender (nodig voor MapIdentityApi)
builder.Services.AddTransient<IEmailSender<GebruikerDB>, DummyEmailSender>();

// ----------------------------------------------------------
// Authentication — Bearer Token 
// ----------------------------------------------------------
builder.Services.AddAuthentication()
    .AddBearerToken(IdentityConstants.BearerScheme, options =>
    {
        options.BearerTokenExpiration = TimeSpan.FromMinutes(60);
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

    // Support voor DateOnly / TimeOnly
    options.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
    options.MapType<TimeOnly>(() => new OpenApiSchema { Type = "string", Format = "time" });

    // Bearer token support
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Voer je Bearer token in.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
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
app.UseAuthentication();
app.UseAuthorization();

// ----------------------------------------------------------
// Identity API Endpoints (login/register/token)
// ----------------------------------------------------------
app.MapIdentityApi<GebruikerDB>();

// ----------------------------------------------------------
// Role Seeding
// ----------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    string[] roles = { "Admin", "Manager", "Teamlead", "User" };

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
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);

        if (createResult.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        else
        {
            foreach (var error in createResult.Errors)
            {
                Console.WriteLine($"[ADMIN ERROR] {error.Description}");
            }
        }
    }
}

app.MapControllers();
app.Run();

// ----------------------------------------------------------
// Dummy Email Sender
// ----------------------------------------------------------
public class DummyEmailSender : IEmailSender<GebruikerDB>
{
    public Task SendConfirmationLinkAsync(GebruikerDB user, string email, string link)
        => Task.CompletedTask;

    public Task SendPasswordResetLinkAsync(GebruikerDB user, string email, string link)
        => Task.CompletedTask;

    public Task SendPasswordResetCodeAsync(GebruikerDB user, string email, string code)
        => Task.CompletedTask;
}
