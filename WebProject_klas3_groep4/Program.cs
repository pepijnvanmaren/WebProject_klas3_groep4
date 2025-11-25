using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------
// Add Services
// ------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        // voorkom object-cycli bij serialisatie (bijv. EF navigation properties)
        opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebProject API",
        Version = "v1"
    });

    // Map DateOnly / TimeOnly naar strings
    c.MapType<DateOnly>(() => new OpenApiSchema { Type = "string", Format = "date" });
    c.MapType<TimeOnly>(() => new OpenApiSchema { Type = "string", Format = "time" });
});

// Database
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Server=(localdb)\\mssqllocaldb;Database=WebProject_klas3_groep4;Trusted_Connection=True;")
);

// Identity
builder.Services.AddIdentity<GebruikerDB, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<DatabaseContext>()
.AddDefaultTokenProviders();

// ------------------------------------------
// Build App
// ------------------------------------------
var app = builder.Build();

// Developer diagnostics — laat details zien bij runtime-fouten
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebProject API V1");
        // geen RoutePrefix instellen = standaard /swagger/index.html
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
