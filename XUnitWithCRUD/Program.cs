using CRUDExample.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;

namespace XUnitWithCRUD;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {

            loggerConfiguration
            .ReadFrom.Configuration(context.Configuration) 
            .ReadFrom.Services(services); 
        });

        builder.Services.AddControllersWithViews(options => {

            var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

            options.Filters.Add(new ResponseHeaderActionFilter(logger, "My-Key-From-Global", "My-Value-From-Global"));
        });
        builder.Logging.ClearProviders().AddConsole();

        builder.Services.AddHttpLogging(options =>
        {
            options.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
        });

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

        app.UseHttpLogging();

        if (!builder.Environment.IsEnvironment("Test"))
            Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");


        app.UseStaticFiles();
        app.UseRouting();
        app.MapControllers();

     
        app.Run();
    }
}
