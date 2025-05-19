using Service;
using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace View
{
    public partial class frmMain : Form
    {
        private readonly IPersonService _personService;
        private readonly ProductService _productService; // Change to IProductService when ready

        public frmMain(IPersonService personService, ProductService productService)
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
            // Using direct instantiation for now if frmProduct is not DI-ready
            // This assumes frmProduct constructor takes ProductService
            if (_productService == null)
            {
                MessageBox.Show("Internal error: Product service not available.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            frmProduct productForm = new frmProduct(_productService); // Manual instantiation
            productForm.ShowDialog(this);

            // When frmProduct is DI-ready:
            /*
            using (var scope = Program.ServiceProvider.CreateScope())
            {
                var productForm = scope.ServiceProvider.GetRequiredService<frmProduct>();
                productForm.ShowDialog(this);
            }
            */
        }
    }
}