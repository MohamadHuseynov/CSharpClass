using Service;
using Microsoft.Extensions.DependencyInjection;

namespace View
{
    public partial class frmMain : Form
    {
        private readonly IPersonService _personService;
        private readonly IProductService _productService;

        // Constructor now takes interfaces for both services
        public frmMain(IPersonService personService, IProductService productService)
        {
            InitializeComponent();
            _personService = personService ?? throw new ArgumentNullException(nameof(personService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.Text = "Main Application Form";
        }

        private void btnPerson_Click_1(object sender, EventArgs e)
        {
            using (var scope = Program.ServiceProvider.CreateScope())
            {
                var personForm = scope.ServiceProvider.GetRequiredService<frmPerson>();
                personForm.ShowDialog(this);
            }
        }

        private void btnProduct_Click_1(object sender, EventArgs e)
        {
           
            if (_productService == null) // This check is against the IProductService field now
            {
                MessageBox.Show("Internal error: Product service not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            using (var scope = Program.ServiceProvider.CreateScope())
            {
                // This assumes frmProduct's constructor now takes IProductService
                // and frmProduct is registered in Program.cs ConfigureServices
                var productForm = scope.ServiceProvider.GetRequiredService<frmProduct>();
                productForm.ShowDialog(this);
            }
        }
    }
}