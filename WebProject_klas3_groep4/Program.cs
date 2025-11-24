using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4;

namespace WebProject_klas3_groep4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Services
            builder.Services.AddControllers();

            // Configure EF Core to use SQL Server with connection string from appsettings
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                                   ?? "Server=localhost;Database=WebProjectDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddRouting();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReact", policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Ensure CORS is applied before mapping controllers
            app.UseCors("AllowReact");

            app.MapControllers();

            app.Run();
        }
    }
}
