using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ST10441211_PROG6212_POE.Data;
using ST10441211_PROG6212_POE.Controllers;
using ST10441211_PROG6212_POE.Views;

namespace ST10441211_PROG6212_POE
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Lecturer Claims Management System";

            // Setup Dependency Injection
            var serviceProvider = ConfigureServices();

            // Ensure database is created
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();
            }

            // Get the application controller
            var appController = serviceProvider.GetRequiredService<ApplicationController>();

            // Run the application
            appController.Run();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Connection string - UPDATE THIS with your actual connection string
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=ClaimsManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true";

            // Add DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register Controllers
            services.AddScoped<ApplicationController>();
            services.AddScoped<HomeController>();
            services.AddScoped<AccountController>();
            services.AddScoped<DashboardController>();
            services.AddScoped<ClaimsController>();

            // Register Views (Console UI)
            services.AddScoped<ConsoleView>();

            // Register Session Manager
            services.AddSingleton<SessionManager>();

            return services.BuildServiceProvider();
        }
    }
}