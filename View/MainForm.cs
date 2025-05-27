using Service; // For PersonService, ProductService (orchestrating services)
using Microsoft.Extensions.DependencyInjection; // For Program.ServiceProvider

namespace View
{
    public partial class frmMain : Form
    {
        // These are the orchestrating services from the Service project
        private readonly PersonService _personService;
        private readonly ProductService _productService;

        public frmMain(PersonService personService, ProductService productService)
        {
            InitializeComponent();
            _personService = personService ?? throw new ArgumentNullException(nameof(personService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Text = "My Application - Main Menu";
        }

        // Ensure your button in frmMain.Designer.cs is named btnOpenPersons
        // or update the method name here and in the designer.
        private void btnOpenPersons_Click(object sender, EventArgs e)
        {
            if (_personService == null) // Should not happen with DI
            {
                MessageBox.Show("Person Service is not available.", "Service Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (var scope = Program.ServiceProvider.CreateScope())
                {
                    var personForm = scope.ServiceProvider.GetRequiredService<frmPerson>();
                    personForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Person form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Ensure your button in frmMain.Designer.cs is named btnOpenProducts
        // or update the method name here and in the designer.
        private void btnOpenProducts_Click(object sender, EventArgs e)
        {
            if (_productService == null) // Should not happen with DI
            {
                MessageBox.Show("Product Service is not available.", "Service Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                using (var scope = Program.ServiceProvider.CreateScope())
                {
                    var productForm = scope.ServiceProvider.GetRequiredService<frmProduct>();
                    productForm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Product form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}