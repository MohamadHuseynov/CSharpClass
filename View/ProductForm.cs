using Service;         // For ProductService
using Service.DTOs;    // For Product DTOs and ServiceResult
using System.Text;

namespace View
{
    public partial class frmProduct : Form
    {
        private readonly ProductService _productService;
        private int _selectedProductIdForEdit = 0; // Store ID of product selected for editing

        // --- Column Name Constants from your frmProduct.Designer.cs ---
        private const string CheckBoxColumnName = "CheckBox";
        // We'll add a hidden ID column programmatically if not in designer
        private const string IdColumnName = "colId";
        private const string TitleColumnName = "productTitle";
        private const string UnitPriceColumnName = "productUnitPrice";
        private const string QuantityColumnName = "productQuantity";

        // Constructor if ProductService is to be injected (preferred for testing/flexibility)
        // public frmProduct(ProductService productService)
        // {
        //     InitializeComponent();
        //     _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        //     InitializeForm();
        // }

        // Parameterless constructor (assuming Program.cs might instantiate it this way for now)
        public frmProduct()
        {
            InitializeComponent();
            _productService = new ProductService(); // ProductService instantiates ProductServiceModel
            InitializeForm();
        }

        private void InitializeForm()
        {
            SetupDataGridViewColumns();

            this.Shown += (s, e) =>
            {
                ClearAllFieldsAndSelections();
                ToggleControls(true);
                dataGridViewProduct.DataSource = null;
                txtTitle.Clear();
                txtUnitPrice.Clear();
                txtQuantity.Clear();
                UpdateButtonStatesAndTextBoxes();
            };

            // Event handlers (some might already be wired in your Designer.cs)
            // dataGridViewProduct.CellContentClick += dataGridViewProduct_CellContentClick; // Already in your Designer
            dataGridViewProduct.SelectionChanged += dataGridViewProduct_SelectionChanged;
            dataGridViewProduct.CurrentCellDirtyStateChanged += dataGridViewProduct_CurrentCellDirtyStateChanged;
            dataGridViewProduct.CellValueChanged += dataGridViewProduct_CellValueChanged;
        }

        private void SetupDataGridViewColumns()
        {
            dataGridViewProduct.AutoGenerateColumns = false;

            // Ensure columns from Designer.cs have correct DataPropertyName and ReadOnly settings
            // Or, if you prefer full programmatic setup after Columns.Clear():
            // dataGridViewProduct.Columns.Clear();
            // Then add all columns programmatically like in frmPerson's example.

            // For now, let's assume columns exist from Designer and we adjust them:

            // CheckBox Column (Name: "CheckBox" in your designer)
            if (dataGridViewProduct.Columns.Contains(CheckBoxColumnName))
            {
                dataGridViewProduct.Columns[CheckBoxColumnName].ReadOnly = false;
                dataGridViewProduct.Columns[CheckBoxColumnName].Width = 50;
                dataGridViewProduct.Columns[CheckBoxColumnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            else
            { // Add if missing
                var checkBoxCol = new DataGridViewCheckBoxColumn { Name = CheckBoxColumnName, HeaderText = "", Width = 50, AutoSizeMode = DataGridViewAutoSizeColumnMode.None, ReadOnly = false };
                dataGridViewProduct.Columns.Insert(0, checkBoxCol);
            }

            // ID Column (Hidden) - Add if not present
            if (!dataGridViewProduct.Columns.Contains(IdColumnName))
            {
                var idCol = new DataGridViewTextBoxColumn { Name = IdColumnName, DataPropertyName = "Id", Visible = false, ReadOnly = true };
                dataGridViewProduct.Columns.Insert(1, idCol); // Insert after checkbox
            }
            else
            {
                dataGridViewProduct.Columns[IdColumnName].DataPropertyName = "Id";
                dataGridViewProduct.Columns[IdColumnName].Visible = false;
                dataGridViewProduct.Columns[IdColumnName].ReadOnly = true;
            }

            // Title Column (Name: "productTitle" in your designer)
            if (dataGridViewProduct.Columns.Contains(TitleColumnName))
            {
                dataGridViewProduct.Columns[TitleColumnName].DataPropertyName = "Title";
                dataGridViewProduct.Columns[TitleColumnName].ReadOnly = true;
            }

            // UnitPrice Column (Name: "productUnitPrice" in your designer)
            if (dataGridViewProduct.Columns.Contains(UnitPriceColumnName))
            {
                dataGridViewProduct.Columns[UnitPriceColumnName].DataPropertyName = "UnitPrice";
                dataGridViewProduct.Columns[UnitPriceColumnName].ReadOnly = true;
            }

            // Quantity Column (Name: "productQuantity" in your designer)
            if (dataGridViewProduct.Columns.Contains(QuantityColumnName))
            {
                dataGridViewProduct.Columns[QuantityColumnName].DataPropertyName = "Quantity";
                dataGridViewProduct.Columns[QuantityColumnName].ReadOnly = true;
            }

            dataGridViewProduct.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // AllowUserToAddRows is false and RowHeadersVisible is false in your Designer.cs, which is good.
        }

        private void ToggleControls(bool enable)
        {
            txtTitle.Enabled = enable;
            txtUnitPrice.Enabled = enable;
            txtQuantity.Enabled = enable;
            btnAdd.Enabled = enable;
            btnRefresh.Enabled = enable;
            btnBack.Enabled = enable;
            dataGridViewProduct.Enabled = enable;

            if (enable) UpdateButtonStatesAndTextBoxes();
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void LoadData()
        {
            ToggleControls(false);
            var result = _productService.GetAllProducts();
            if (result.IsSuccess && result.Data != null)
            {
                dataGridViewProduct.DataSource = null;
                dataGridViewProduct.DataSource = result.Data;
            }
            else
            {
                MessageBox.Show(result.Message ?? "An unknown error occurred while loading data.", "Error Loading Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridViewProduct.DataSource = null;
            }
            // ClearAllFieldsAndSelections(); // To be called by refresh button or after operations
            ToggleControls(true);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a Title.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }
            if (!int.TryParse(txtUnitPrice.Text, out int unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Please enter a valid positive Unit Price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus();
                return;
            }
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Please enter a valid positive Quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return;
            }

            var postDto = new PostProductDto()
            {
                Title = txtTitle.Text.Trim(),
                UnitPrice = unitPrice,
                Quantity = quantity
            };

            ToggleControls(false);
            var result = _productService.AddProduct(postDto);
            ToggleControls(true);

            if (result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Operation Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearAllFieldsAndSelections(); // Clear after successful add and load
            }
            else
            {
                MessageBox.Show(result.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
            ClearAllFieldsAndSelections(); // Ensure clean state after refresh
            MessageBox.Show("Data has been refreshed.", "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedProductIdForEdit <= 0)
            {
                MessageBox.Show("Please check the box next to the product you want to edit.", "No Product Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Title cannot be empty for editing.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return;
            }
            if (!int.TryParse(txtUnitPrice.Text, out int unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Please enter a valid positive Unit Price for editing.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus();
                return;
            }
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Please enter a valid positive Quantity for editing.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return;
            }

            var updateDto = new UpdateProductDto
            {
                Title = txtTitle.Text.Trim(),
                UnitPrice = unitPrice,
                Quantity = quantity
            };

            ToggleControls(false);
            var result = _productService.UpdateProduct(_selectedProductIdForEdit, updateDto);
            ToggleControls(true);

            if (result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show(result.Message, "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedIds = GetSelectedIdsFromCheckboxes();
            if (!selectedIds.Any())
            {
                MessageBox.Show("Please check the box(es) for the product(s) you want to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmResult = MessageBox.Show($"Are you sure you want to delete {selectedIds.Count} selected product(s)?",
                                     "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                ToggleControls(false);
                int successCount = 0;
                int failCount = 0;
                StringBuilder errors = new StringBuilder();

                foreach (var id in selectedIds)
                {
                    var result = _productService.DeleteProduct(id);
                    if (result.IsSuccess)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        errors.AppendLine($"- ID {id}: {result.Message}");
                    }
                }
                ToggleControls(true);

                string summaryMessage = $"{successCount} product(s) deleted successfully.";
                if (failCount > 0)
                {
                    summaryMessage += $"\n{failCount} product(s) could not be deleted:\n{errors.ToString()}";
                }
                MessageBox.Show(summaryMessage, "Delete Operation Complete", MessageBoxButtons.OK,
                                failCount > 0 ? MessageBoxIcon.Error : MessageBoxIcon.Information);
                LoadData();
            }
        }

        private List<int> GetSelectedIdsFromCheckboxes()
        {
            var ids = new List<int>();
            if (dataGridViewProduct.Columns.Contains(CheckBoxColumnName) && dataGridViewProduct.Columns.Contains(IdColumnName))
            {
                foreach (DataGridViewRow row in dataGridViewProduct.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    if (checkBoxCell?.Value != null && Convert.ToBoolean(checkBoxCell.Value))
                    {
                        if (row.Cells[IdColumnName].Value != null && int.TryParse(row.Cells[IdColumnName].Value.ToString(), out int id))
                        {
                            ids.Add(id);
                        }
                    }
                }
            }
            return ids;
        }

        private void dataGridViewProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridViewProduct.Columns.Contains(CheckBoxColumnName) && e.ColumnIndex == dataGridViewProduct.Columns[CheckBoxColumnName].Index)
            {
                dataGridViewProduct.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            // No direct textbox population from non-checkbox cell clicks
            // UpdateButtonStatesAndTextBoxes will be called by CellValueChanged or SelectionChanged
        }

        private void dataGridViewProduct_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewProduct.IsCurrentCellDirty &&
                dataGridViewProduct.Columns.Contains(CheckBoxColumnName) &&
                dataGridViewProduct.CurrentCell.OwningColumn.Name == CheckBoxColumnName)
            {
                dataGridViewProduct.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridViewProduct_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridViewProduct.Columns.Contains(CheckBoxColumnName) && e.ColumnIndex == dataGridViewProduct.Columns[CheckBoxColumnName].Index)
            {
                UpdateButtonStatesAndTextBoxes();
            }
        }

        private void dataGridViewProduct_SelectionChanged(object sender, EventArgs e)
        {
            // This event should not populate textboxes by itself if we want checkbox-driven population.
            // It will simply ensure button states are updated.
            UpdateButtonStatesAndTextBoxes();
        }

        private void PopulateFieldsFromRow(DataGridViewRow row)
        {
            if (row == null || row.IsNewRow ||
                !dataGridViewProduct.Columns.Contains(IdColumnName) || row.Cells[IdColumnName].Value == null ||
                !dataGridViewProduct.Columns.Contains(TitleColumnName) ||
                !dataGridViewProduct.Columns.Contains(UnitPriceColumnName) ||
                !dataGridViewProduct.Columns.Contains(QuantityColumnName))
            {
                _selectedProductIdForEdit = 0;
                return;
            }

            if (int.TryParse(row.Cells[IdColumnName].Value.ToString(), out int id))
            {
                _selectedProductIdForEdit = id;
                txtTitle.Text = row.Cells[TitleColumnName].Value?.ToString() ?? "";
                txtUnitPrice.Text = row.Cells[UnitPriceColumnName].Value?.ToString() ?? "";
                txtQuantity.Text = row.Cells[QuantityColumnName].Value?.ToString() ?? "";
            }
            else
            {
                _selectedProductIdForEdit = 0;
            }
        }

        private void ClearCheckboxes()
        {
            if (dataGridViewProduct.Columns.Contains(CheckBoxColumnName))
            {
                foreach (DataGridViewRow row in dataGridViewProduct.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    if (checkBoxCell != null) checkBoxCell.Value = false;
                }
            }
        }

        private void ClearAllFieldsAndSelections()
        {
            txtTitle.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();
            _selectedProductIdForEdit = 0;
            ClearCheckboxes();
            if (dataGridViewProduct.Rows.Count > 0)
            {
                dataGridViewProduct.ClearSelection();
                if (dataGridViewProduct.CurrentCell != null)
                    dataGridViewProduct.CurrentCell = null;
            }
            UpdateButtonStatesAndTextBoxes();
        }

        private void UpdateButtonStatesAndTextBoxes()
        {
            var selectedIdsFromCheckboxes = GetSelectedIdsFromCheckboxes();
            int checkedCount = selectedIdsFromCheckboxes.Count;

            btnDelete.Enabled = checkedCount > 0;

            if (checkedCount == 1)
            {
                btnEdit.Enabled = true;
                foreach (DataGridViewRow row in dataGridViewProduct.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    if (checkBoxCell?.Value != null && Convert.ToBoolean(checkBoxCell.Value))
                    {
                        PopulateFieldsFromRow(row);
                        break;
                    }
                }
            }
            else
            {
                btnEdit.Enabled = false;
                txtTitle.Clear();
                txtUnitPrice.Clear();
                txtQuantity.Clear();
                _selectedProductIdForEdit = 0;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmProduct_Load(object sender, EventArgs e)
        {

        }

       
    }
}