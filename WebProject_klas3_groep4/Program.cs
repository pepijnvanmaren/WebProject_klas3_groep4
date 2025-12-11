using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.models;

var builder = WebApplication.CreateBuilder(args);   //initializes de app en dependency injection

builder.Services.AddDbContext<DatabaseContext>(options =>   //add service: databaseContext
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))    //database connectie uit appsettings.json halen
);

builder.Services.AddIdentity<GebruikerDB, IdentityRole<int>>(options =>
{
    //password requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<DatabaseContext>()
.AddDefaultTokenProviders();    //token provider

var jwtKey = builder.Configuration["Jwt:Key"];  //Haalt JWT key van configuration
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
    options.RequireHttpsMetadata = false;   //Hier nog ff naar kijken. Op true als het project klaar is?
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") //Localhost adres
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

//Swagger dingen
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebProject API", Version = "v1" });

    //Authenticatie plek in swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

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
app.MapControllers();

//Role Seeding
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    string[] roles = { "Admin", "Koper", "Aanvoerder", "Veilingmeester" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<int>(role));
    }
}

//Maakt Admin aan (Seeder)
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<GebruikerDB>>();
    var existingUser = await userManager.FindByNameAsync("adminUser");
    if (existingUser == null)
    {
        var adminUser = new GebruikerDB
        {
            UserName = "adminUser",
            Email = "admin@example.com",
            EmailConfirmed = true,
            Rol = "Admin"
        };
        var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
        if (createResult.Succeeded)
            await userManager.AddToRoleAsync(adminUser, "Admin");
        else
            foreach (var error in createResult.Errors)
                Console.WriteLine("[ADMIN ERROR] " + error.Description);
    }
}

app.Run();
