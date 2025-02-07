

using System.Diagnostics;
using System.Globalization;

namespace View
{
    public partial class frmProduct : Form
    {
        public frmProduct()
        {
            InitializeComponent();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
          
        }
    }
}
