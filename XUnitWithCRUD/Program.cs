namespace XUnitWithCRUD
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            builder.Services.AddControllersWithViews();

            if(builder.Environment.IsDevelopment()) 
                app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();
            app.MapControllers();

         
            app.Run();
        }
    }
}
