using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace XUnitWithCRUD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ICountriesRepository , CountriesRepository>();
            builder.Services.AddScoped<IPersonsRepository , PersonsRepository>();
            builder.Services.AddScoped<ICountriesService, CountriesService>();
            builder.Services.AddScoped<IPersonsService, PersonsService>();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            });


            var app = builder.Build();

            if(builder.Environment.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");


            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

         
            app.Run();
        }
    }
}
