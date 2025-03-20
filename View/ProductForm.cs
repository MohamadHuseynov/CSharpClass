using Model;
using Model.DomainModels;
using System.Drawing.Text;
using System.Windows.Forms;

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

        #region [-dataGridViewProduct_CellContentClick OLD CODES-]
        //private void dataGridViewProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{

        //    if (CheckBox.Selected = true)
        //    {
        //        btnDelete.Enabled = true;
        //        btnEdit.Enabled = true;

        //    }

        //    if (e.RowIndex != -1)
        //    {
        //        DataGridViewRow dgvProduct = dataGridViewProduct.Rows[e.RowIndex];

        //        txtTitle.Text = dgvProduct.Cells[1].Value.ToString();
        //        txtUnitPrice.Text = dgvProduct.Cells[2].Value.ToString();
        //        txtQuantity.Text = dgvProduct.Cells[3].Value.ToString();

        //        selectedRowIndex = e.RowIndex;
        //    }


        //    // بررسی اینکه آیا کاربر روی ستون چک باکس کلیک کرده است
        //    if (e.ColumnIndex == 0 && e.RowIndex >= 0) // فرض کنید ستون چک باکس در ایندکس 0 است
        //    {
        //        // تغییر وضعیت چک باکس
        //        DataGridViewCheckBoxCell checkBoxCell = (DataGridViewCheckBoxCell)dataGridViewProduct.Rows[e.RowIndex].Cells[0];
        //        checkBoxCell.Value = !(checkBoxCell.Value != null && (bool)checkBoxCell.Value);
        //        dataGridViewProduct.EndEdit(); // اتمام ویرایش

        //        UpdateButtonStates(); // به‌روزرسانی وضعیت دکمه‌ها
        //    } 


        //}
        #endregion

        private void dataGridViewProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // جلوگیری از کلیک روی هدر ستون

            DataGridViewRow dgvProduct = dataGridViewProduct.Rows[e.RowIndex];

            txtTitle.Text = dgvProduct.Cells[1].Value?.ToString();
            txtUnitPrice.Text = dgvProduct.Cells[2].Value?.ToString();
            txtQuantity.Text = dgvProduct.Cells[3].Value?.ToString();

            selectedRowIndex = e.RowIndex;

            // چک کردن اینکه روی چک‌باکس کلیک شده یا نه
            if (e.ColumnIndex == 0)
            {
                DataGridViewCheckBoxCell checkBoxCell = (DataGridViewCheckBoxCell)dgvProduct.Cells[0];
                checkBoxCell.Value = !(checkBoxCell.Value != null && (bool)checkBoxCell.Value);
                dataGridViewProduct.EndEdit();
                UpdateButtonStates();
            }
        }

        private void txtTitle_TextChanged(object sender, EventArgs e)
        {
        }
        #region [-BTN ADD OLD CODES-]

        //private void btnAdd_Click(object sender, EventArgs e)
        //{

        //    product.Title = txtTitle.Text;

        //    // متغیر برای بررسی اعتبار ورودی‌ها
        //    bool isValid = true;

        //    try
        //    {
        //        product.UnitPrice = Convert.ToInt32(txtUnitPrice.Text);
        //    }
        //    catch
        //    {
        //        MessageBox.Show("Please enter only numbers in unit price", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        isValid = false; // ورودی نامعتبر است
        //    }

        //    try
        //    {
        //        product.Quantity = Convert.ToInt32(txtQuantity.Text);
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Please enter only numbers in quantity", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        isValid = false; // ورودی نامعتبر است
        //    }

        //    // اگر ورودی‌ها معتبر بودند، ردیف را اضافه کنید
        //    if (isValid)
        //    {
        //        // کد اضافه کردن ردیف به دیتاگرید ویو
        //        // مثلاً:
        //        dataGridViewProduct.Rows.Add(ifCheckBox, product.Title, product.UnitPrice, product.Quantity);
        //    }


        //    txtTitle.Clear();
        //    txtUnitPrice.Clear();
        //    txtQuantity.Clear();
        //}


        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (var db = new FinalProjectDbContext())
            {
                Product newProduct = new Product
                {
                    Title = txtTitle.Text
                };

                bool isValid = true;

                try
                {
                    newProduct.UnitPrice = Convert.ToInt32(txtUnitPrice.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter only numbers in unit price", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isValid = false;
                }

                try
                {
                    newProduct.Quantity = Convert.ToInt32(txtQuantity.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter only numbers in quantity", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isValid = false;
                }

                if (isValid)
                {
                    db.Product.Add(newProduct); // ذخیره در دیتابیس
                    db.SaveChanges(); // ذخیره تغییرات
                    LoadData(); // رفرش لیست بعد از اضافه شدن
                }

                txtTitle.Clear();
                txtUnitPrice.Clear();
                txtQuantity.Clear();
            }
        }

        #region [-BTN EDIT OLD CODES-]
        //private void btnEdit_Click(object sender, EventArgs e)
        //{

        //    if (selectedRowIndex != -1 && selectedRowIndex < dataGridViewProduct.Rows.Count)
        //    {
        //        DataGridViewRow dgvEditRoduct = dataGridViewProduct.Rows[selectedRowIndex];
        //        dgvEditRoduct.Cells[1].Value = txtTitle.Text;
        //        dgvEditRoduct.Cells[2].Value = txtUnitPrice.Text;
        //        dgvEditRoduct.Cells[3].Value = txtQuantity.Text;


        //    }


        //} 
        #endregion

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedRowIndex == -1 || selectedRowIndex >= dataGridViewProduct.Rows.Count) return;

            using (var db = new FinalProjectDbContext())
            {
                DataGridViewRow row = dataGridViewProduct.Rows[selectedRowIndex];
                string title = row.Cells[1].Value?.ToString();

                var product = db.Product.FirstOrDefault(p => p.Title == title);
                if (product != null)
                {
                    product.Title = txtTitle.Text;
                    product.UnitPrice = Convert.ToInt32(txtUnitPrice.Text);
                    product.Quantity = Convert.ToInt32(txtQuantity.Text);

                    db.SaveChanges();
                    LoadData(); // لیست را دوباره بارگذاری کن
                }
            }
        }


        #region [-BTN DELETE OLD CODES-]
        //private void btnDelete_Click(object sender, EventArgs e)
        //{
        //    #region [-FIRST OLD CODE-]

        //    //if (selectedRowIndex != -1 && selectedRowIndex < dataGridViewProduct.Rows.Count)
        //    //{
        //    //    DataGridViewRow dgvDeleteRoduct = dataGridViewProduct.Rows[selectedRowIndex];

        //    //    dataGridViewProduct.Rows.RemoveAt(selectedRowIndex);

        //    //}
        //    //txtTitle.Clear();
        //    //txtUnitPrice.Clear();
        //    //txtQuantity.Clear(); 
        //    #endregion

        //    #region [-SECOND OLD CODE-]

        //    // حذف آیتم فقط در صورتی که چک‌باکس تیک خورده باشد

        //    ////if (selectedRowIndex != -1 && selectedRowIndex < dataGridViewProduct.Rows.Count)
        //    ////{
        //    ////    DataGridViewRow dgvDeleteProduct = dataGridViewProduct.Rows[selectedRowIndex];
        //    ////    DataGridViewCheckBoxCell checkBox = dgvDeleteProduct.Cells[0] as DataGridViewCheckBoxCell;

        //    ////    // فقط زمانی حذف انجام شود که چک‌باکس تیک خورده باشد
        //    ////    if (checkBox != null && checkBox.Value != null && (bool)checkBox.Value)
        //    ////    {
        //    ////        dataGridViewProduct.Rows.RemoveAt(selectedRowIndex);
        //    ////        txtTitle.Clear();
        //    ////        txtUnitPrice.Clear();
        //    ////        txtQuantity.Clear();
        //    ////    }
        //    ////} 
        //    #endregion

        //    // استفاده از یک لیست برای ذخیره ردیف‌های که باید حذف شوند
        //    List<DataGridViewRow> rowsToDelete = new List<DataGridViewRow>();

        //    foreach (DataGridViewRow row in dataGridViewProduct.Rows)
        //    {
        //        if (Convert.ToBoolean(row.Cells[0].Value)) // فرض کنید ستون چک باکس در ایندکس 0 است
        //        {
        //            rowsToDelete.Add(row); // ردیف‌های تیک‌خورده را به لیست اضافه کنید
        //        }
        //    }

        //    // حذف ردیف‌ها از دیتاگرید ویو
        //    foreach (DataGridViewRow row in rowsToDelete)
        //    {
        //        dataGridViewProduct.Rows.Remove(row);
        //    }

        //} 
        #endregion

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (var db = new FinalProjectDbContext())
            {
                List<Product> productsToDelete = new List<Product>();

                foreach (DataGridViewRow row in dataGridViewProduct.Rows)
                {
                    if (Convert.ToBoolean(row.Cells[0].Value))
                    {
                        string title = row.Cells[1].Value?.ToString();
                        var product = db.Product.FirstOrDefault(p => p.Title == title);
                        if (product != null)
                        {
                            productsToDelete.Add(product);
                        }
                    }
                }

                if (productsToDelete.Count > 0)
                {
                    db.Product.RemoveRange(productsToDelete);
                    db.SaveChanges();
                    LoadData();
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();

        }




        private void frmProduct_Load(object sender, EventArgs e)
        {

        }

        private void LoadData()
        {
            using (var db = new FinalProjectDbContext())
            {
                var products = db.Product.ToList(); // دریافت محصولات از دیتابیس

                dataGridViewProduct.Rows.Clear(); // پاک کردن داده‌های قبلی
                foreach (var product in products)
                {
                    dataGridViewProduct.Rows.Add(false, product.Title, product.UnitPrice, product.Quantity);
                }
            }
        }
        private void UpdateButtonStates()
        {
            int checkedCount = 0;

            // شمارش تعداد چک باکس‌های تیک خورده
            foreach (DataGridViewRow row in dataGridViewProduct.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value)) // فرض کنید ستون چک باکس در ایندکس 0 است
                {
                    checkedCount++;
                }
            }

            // فعال و غیرفعال کردن دکمه‌ها بر اساس تعداد چک باکس‌های تیک خورده
            if (checkedCount == 1)
            {
                btnEdit.Enabled = true; // فقط دکمه ادیت فعال می‌شود
                btnDelete.Enabled = true; // دکمه دیلیت هم فعال می‌شود
            }
            else if (checkedCount > 1)
            {
                btnEdit.Enabled = false; // دکمه ادیت غیرفعال می‌شود
                btnDelete.Enabled = true; // فقط دکمه دیلیت فعال می‌شود
            }
            else
            {
                btnEdit.Enabled = false; // هیچ دکمه‌ای فعال نیست
                btnDelete.Enabled = false; // هیچ دکمه‌ای فعال نیست
            }
        }

        private void txtUnitPrice_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
