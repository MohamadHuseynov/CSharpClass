using Model;
using Service;

namespace View
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ProductService productService = new ProductService(new FinalProjectDbContext());
            frmProduct productForm = new frmProduct(productService);
            productForm.Show();
        }

        private void btnPerson_Click(object sender, EventArgs e)
        {
            frmPerson personForm = new frmPerson();
            personForm.ShowDialog();
            
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
