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
            frmProduct frmProduct = new frmProduct();
            frmProduct.ShowDialog();
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
