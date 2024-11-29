using Entities;
using Microsoft.EntityFrameworkCore;
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

            builder.Services.AddSingleton<ICountriesService, CountriesService>();
            builder.Services.AddSingleton<IPersonService, PersonsService>();

            builder.Services.AddDbContext<PersonsDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
            });


            var app = builder.Build();

            if(builder.Environment.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

         
            app.Run();
        }
    }
}
