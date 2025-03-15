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

        }
        Product product = new Product();

        private int selectedRowIndex = -1;
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

            if (e.RowIndex != -1)
            {
                DataGridViewRow dgvProduct = dataGridViewProduct.Rows[e.RowIndex];

                txtTitle.Text = dgvProduct.Cells[1].Value.ToString();
                txtUnitPrice.Text = dgvProduct.Cells[2].Value.ToString();
                txtQuantity.Text = dgvProduct.Cells[3].Value.ToString();

                selectedRowIndex = e.RowIndex;
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

            dataGridViewProduct.Rows.Add(ifCheckBox, product.Title, product.UnitPrice, product.Quantity);

            txtTitle.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();


        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex != -1 && selectedRowIndex < dataGridViewProduct.Rows.Count)
            {
                DataGridViewRow dgvEditRoduct = dataGridViewProduct.Rows[selectedRowIndex];
                dgvEditRoduct.Cells[1].Value = txtTitle.Text;
                dgvEditRoduct.Cells[2].Value = txtUnitPrice.Text;
                dgvEditRoduct.Cells[3].Value = txtQuantity.Text;

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex != -1 && selectedRowIndex < dataGridViewProduct.Rows.Count)
            {
                DataGridViewRow dgvDeleteRoduct = dataGridViewProduct.Rows[selectedRowIndex];
               
                dataGridViewProduct.Rows.RemoveAt(selectedRowIndex);

            }
            txtTitle.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();




        }
    }
}
