using Microsoft.EntityFrameworkCore;
using WebProject_klas3_groep4;
using Microsoft.Data.Sqlite;

namespace WebProject_klas3_groep4
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();

            // Configure EF to use SQL Server with the connection string from appsettings.json
            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
            app.MapControllers();
            app.UseCors("AllowReact");
            app.Run();
        }
    }
}
