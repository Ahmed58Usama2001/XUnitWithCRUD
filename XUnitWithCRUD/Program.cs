using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using Serilog;
using ServiceContracts;
using Services;
using XUnitWithCRUD.Filters.ActionFilters;

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

        builder.Logging.ClearProviders().AddConsole();

        builder.Services.ConfigureServices(builder.Configuration);

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
