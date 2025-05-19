using Microsoft.Extensions.DependencyInjection;
using Model; // For FinalProjectDbContext
using Service; // For IPersonService, PersonService, ProductService etc.
using System;
using System.Windows.Forms;

namespace View
{
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            using (var scope = ServiceProvider.CreateScope())
            {
                var mainForm = scope.ServiceProvider.GetRequiredService<frmMain>();
                Application.Run(mainForm);
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Register DbContext.
            // If FinalProjectDbContext is instantiated via DI (e.g., services.AddDbContext<FinalProjectDbContext>()),
            // it would use the constructor that takes DbContextOptions.
            // If those options are not configured (e.g. no .UseSqlServer() call here),
            // then its OnConfiguring method with the hardcoded string will be used.
            // For simplicity with your request, we can register it as Scoped or Transient.
            // Scoped means one instance per scope (e.g., per form if you create a scope per form).
            // Transient means a new instance every time it's requested.
            // Let's use Scoped, which is common for DbContext.
            services.AddScoped<FinalProjectDbContext>(); // This will use the parameterless constructor, then OnConfiguring.

            // Alternatively, if you wanted DI to pass empty options (which still triggers OnConfiguring):
            // services.AddDbContext<FinalProjectDbContext>(); // This is also effectively Scoped.

            // Register your services and their interfaces
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<ProductService>(); // Assuming ProductService takes FinalProjectDbContext

            // Register your forms
            services.AddTransient<frmMain>();
            services.AddTransient<frmPerson>();
            services.AddTransient<frmProduct>();
        }
    }
}