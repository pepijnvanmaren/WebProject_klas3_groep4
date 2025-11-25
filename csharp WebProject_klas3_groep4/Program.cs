using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using WebProject_klas3_groep4;

namespace WebProject_klas3_groep4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext registration (single, with connection string)
            builder.Services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=database.db"));

            // increase multipart body limit (adjust as needed)
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 50_000_000; // 50 MB
            });

            // CORS for frontend dev (adjust or restrict origins in production)
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddRouting();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Serve wwwroot so uploaded files are accessible via /uploads/...
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors();
            app.MapControllers();
            app.Run();
        }
    }
}