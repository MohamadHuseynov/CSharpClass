using Microsoft.Extensions.DependencyInjection;
using Model; 
using Service; 


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
            
            services.AddScoped<FinalProjectDbContext>(); // This will use the parameterless constructor, then OnConfiguring.


            // Register your services and their interfaces
            services.AddTransient<IPersonService, PersonService>();
            services.AddTransient<ProductService>(); // Assuming ProductService takes FinalProjectDbContext

            // Registering forms
            services.AddTransient<frmMain>();
            services.AddTransient<frmPerson>();
            services.AddTransient<frmProduct>();
        }
    }
}