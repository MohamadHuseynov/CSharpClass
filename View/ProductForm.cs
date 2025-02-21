

using Microsoft.EntityFrameworkCore.Infrastructure;
using Model.DomainModels;
using System.Drawing.Text;

namespace View
{
    public partial class frmProduct : Form
    {
        public frmProduct()
        {
            InitializeComponent();
            btnDelete.Enabled = false;
            btnEdit.Enabled = false;
            txtId.Focus();

        }
        Product product = new Product();
    
        public bool ifCheckBox = false;

        

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (CheckBox.Selected = true)
            {

                btnDelete.Enabled = true;
                btnEdit.Enabled = true;
            }
            //dataGridViewProduct.Rows.GetRowCount(DataGridViewElementStates.Selected);
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
           
            product.Title = txtTitle.Text;
            product.UnitPrice = Convert.ToInt32(txtUnitPrice.Text);
            product.Quantity = Convert.ToInt32(txtQuantity.Text);

            dataGridViewProduct.Rows.Add(ifCheckBox,product.Title, product.UnitPrice, product.Quantity);

            txtTitle.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();


        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in dataGridViewProduct.Rows)
            {
                if (ifCheckBox == true)
                {
                    dataGridViewProduct.Rows.RemoveAt(item.Index);
                }
            }



        }
    }
}
