using Microsoft.Extensions.DependencyInjection;
using Model; // For FinalProjectDbContext
using Model.ServiceModels; // For PersonServiceModel, ProductServiceModel
using Service; // For PersonService, ProductService (orchestrating services)
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
            // Database Context (used by ServiceModels in the Model project)
            services.AddScoped<FinalProjectDbContext>();

            // Data Access "ServiceModels" (from Model.ServiceModels namespace)
            // These classes directly interact with FinalProjectDbContext.
            services.AddTransient<PersonServiceModel>();
            services.AddTransient<ProductServiceModel>();

            // Orchestration/Business Logic Services (from Service namespace)
            // These classes use the "ServiceModels" (data access classes) above.
            services.AddTransient<PersonService>();
            services.AddTransient<ProductService>();

            // Forms (from View namespace)
            // These classes use the Orchestration/Business Logic Services.
            services.AddTransient<frmMain>();
            services.AddTransient<frmPerson>();
            services.AddTransient<frmProduct>();
        }
    }
}