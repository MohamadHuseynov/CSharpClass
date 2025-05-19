using Service; 
using Service.DTOs;


namespace View
{
    public partial class frmProduct : Form
    {
        private readonly IProductService _productService;
        private int _selectedProductIdForEdit = 0;

        // --- These names MUST match the (Name) property of your DataGridView columns in the Designer ---
        private const string CheckBoxColumnName = "CheckBox";
        private const string IdColumnName = "colId";
        private const string TitleColumnName = "productTitle";
        private const string UnitPriceColumnName = "productUnitPrice";
        private const string QuantityColumnName = "productQuantity";
        // --- End of column name definitions ---

        public frmProduct(IProductService productService)
        {
            InitializeComponent();
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));

            this.Shown += frmProduct_Shown;
            // We will use CellContentClick as the primary handler for checkbox interaction
            // Remove CurrentCellDirtyStateChanged and CellValueChanged for this simplified approach
        }

        private void frmProduct_Shown(object sender, EventArgs e)
        {
            ClearAllFieldsAndSelections();
            UpdateButtonStatesAndTextBoxes();
            ToggleGeneralControls(true);
        }

        private async Task LoadDataAsync()
        {
            this.Cursor = Cursors.WaitCursor;
            ToggleGeneralControls(false);
            try
            {
                var result = await _productService.GetAllProductsAsync();
                // Detach CellContentClick during programmatic changes if it causes issues
                // this.dataGridViewProduct.CellContentClick -= dataGridViewProduct_CellContentClick;
                dataGridViewProduct.Rows.Clear();

                if (result.IsSuccess && result.Data != null && result.Data.Any())
                {
                    foreach (var productDto in result.Data)
                    {
                        dataGridViewProduct.Rows.Add(false, productDto.Id, productDto.Title, productDto.UnitPrice, productDto.Quantity);
                    }
                }
                else if (!result.IsSuccess) { MessageBox.Show(result.Message, "Error Loading Product Data", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            catch (Exception ex) { MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally
            {
                // this.dataGridViewProduct.CellContentClick += dataGridViewProduct_CellContentClick; // Re-attach
                ClearAllFieldsAndSelections();
                UpdateButtonStatesAndTextBoxes();
                ToggleGeneralControls(true);
                this.Cursor = Cursors.Default;
            }
        }

        private void ToggleGeneralControls(bool enabled)
        {
            btnAdd.Enabled = enabled;
            btnRefresh.Enabled = enabled;
            txtTitle.Enabled = enabled;
            txtUnitPrice.Enabled = enabled;
            txtQuantity.Enabled = enabled;
            dataGridViewProduct.Enabled = enabled;
            btnBack.Enabled = enabled;
            if (enabled) UpdateButtonStatesAndTextBoxes(); else { btnEdit.Enabled = false; btnDelete.Enabled = false; }
        }

     
        private void dataGridViewProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Click on header, ignore

            // Check if the click is on the checkbox column
            if (e.ColumnIndex == dataGridViewProduct.Columns[CheckBoxColumnName].Index)
            {
                // Commit any pending edit for the current cell to ensure its value is up-to-date
                // before we try to read or change it. This is often needed for checkboxes.
                dataGridViewProduct.CommitEdit(DataGridViewDataErrorContexts.Commit);

                // Now, manually toggle the value of the checkbox cell that was clicked
                DataGridViewCheckBoxCell checkBoxCell = dataGridViewProduct.Rows[e.RowIndex].Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                if (checkBoxCell != null)
                {
                    bool currentValue = checkBoxCell.Value != null && Convert.ToBoolean(checkBoxCell.Value);
                    checkBoxCell.Value = !currentValue; // Toggle it
                }

                // After manually toggling and committing, immediately update the UI state
                UpdateButtonStatesAndTextBoxes();
            }
            // Optional: Handle clicks on other (non-checkbox) cells if needed
            // else
            // {
            //    // Logic for clicking data cells, e.g., populating textboxes without affecting checkboxes
            //    // For now, we focus on checkbox-driven interaction for edit/delete
            // }
        }


        private void UpdateButtonStatesAndTextBoxes()
        {
            var checkedIds = GetSelectedProductIdsFromGridCheckboxes();
            int checkedCount = checkedIds.Count;
            DataGridViewRow firstSelectedRowForEditVisuals = null;
            _selectedProductIdForEdit = 0;

            if (checkedCount == 1)
            {
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                int singleId = checkedIds.First();
                _selectedProductIdForEdit = singleId;
                foreach (DataGridViewRow row in dataGridViewProduct.Rows)
                {
                    if (!row.IsNewRow && row.Cells[IdColumnName].Value != null && Convert.ToInt32(row.Cells[IdColumnName].Value) == singleId)
                    {
                        firstSelectedRowForEditVisuals = row;
                        break;
                    }
                }
            }
            else if (checkedCount > 1)
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = true;
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }

            if (firstSelectedRowForEditVisuals != null)
            {
                txtTitle.Text = firstSelectedRowForEditVisuals.Cells[TitleColumnName].Value?.ToString() ?? "";
                txtUnitPrice.Text = firstSelectedRowForEditVisuals.Cells[UnitPriceColumnName].Value?.ToString() ?? "";
                txtQuantity.Text = firstSelectedRowForEditVisuals.Cells[QuantityColumnName].Value?.ToString() ?? "";
            }
            else
            {
                txtTitle.Clear();
                txtUnitPrice.Clear();
                txtQuantity.Clear();
            }
        }

        private List<int> GetSelectedProductIdsFromGridCheckboxes()
        {
            var ids = new List<int>();
            foreach (DataGridViewRow row in dataGridViewProduct.Rows)
            {
                if (row.IsNewRow) continue;
                DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                if (checkBoxCell != null && checkBoxCell.Value != null && Convert.ToBoolean(checkBoxCell.Value))
                {
                    if (row.Cells[IdColumnName].Value != null && int.TryParse(row.Cells[IdColumnName].Value.ToString(), out int id))
                    {
                        ids.Add(id);
                    }
                }
            }
            return ids;
        }

        private void ClearAllFieldsAndSelections()
        {
            txtTitle.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();
            _selectedProductIdForEdit = 0;
            if (dataGridViewProduct.Rows.Count > 0)
            {
                // No need to detach/re-attach CellContentClick if its logic is robust
                foreach (DataGridViewRow row in dataGridViewProduct.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    if (checkBoxCell != null) { checkBoxCell.Value = false; }
                }
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputFields(out CreateProductDto createDto)) return;
            this.Cursor = Cursors.WaitCursor;
            ToggleGeneralControls(false);
            var result = await _productService.AddProductAsync(createDto);
            this.Cursor = Cursors.Default;
            if (result.IsSuccess) { await LoadDataAsync(); }
            else { ToggleGeneralControls(true); MessageBox.Show(result.Message, "Error Adding Product", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedProductIdForEdit == 0)
            {
                MessageBox.Show("Please select exactly one product using the checkbox to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ValidateInputFields(out UpdateProductDto updateDto)) return;
            this.Cursor = Cursors.WaitCursor;
            ToggleGeneralControls(false);
            var result = await _productService.UpdateProductAsync(_selectedProductIdForEdit, updateDto);
            this.Cursor = Cursors.Default;
            if (result.IsSuccess) { await LoadDataAsync(); }
            else { ToggleGeneralControls(true); MessageBox.Show(result.Message, "Error Editing Product", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var idsToDelete = GetSelectedProductIdsFromGridCheckboxes();
            if (!idsToDelete.Any())
            {
                MessageBox.Show("Please select at least one product using the checkboxes to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show($"Are you sure you want to delete {idsToDelete.Count} selected product(s)?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;
                ToggleGeneralControls(false);
                int successCount = 0; int failCount = 0; string errorDetails = "";
                foreach (var idInList in idsToDelete)
                {
                    var result = await _productService.DeleteProductAsync(idInList);
                    if (result.IsSuccess) successCount++;
                    else { failCount++; errorDetails += $"ID {idInList}: {result.Message}\n"; }
                }
                this.Cursor = Cursors.Default;
                string finalMessage = "";
                if (successCount > 0) finalMessage += $"{successCount} product(s) deleted successfully.\n";
                if (failCount > 0) finalMessage += $"{failCount} product(s) failed to delete.\nDetails:\n{errorDetails}";
                MessageBox.Show(finalMessage.Trim(), "Delete Result", MessageBoxButtons.OK, (failCount > 0) ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                await LoadDataAsync();
            }
        }

        private bool ValidateInputFields(out CreateProductDto dto)
        {
            dto = null;
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Product title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus(); return false;
            }
            if (!int.TryParse(txtUnitPrice.Text, out int unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Unit Price must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus(); return false;
            }
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Quantity must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus(); return false;
            }
            dto = new CreateProductDto { Title = txtTitle.Text.Trim(), UnitPrice = unitPrice, Quantity = quantity };
            return true;
        }

        private bool ValidateInputFields(out UpdateProductDto dto)
        {
            dto = null;
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Product title is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus(); return false;
            }
            if (!int.TryParse(txtUnitPrice.Text, out int unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Unit Price must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus(); return false;
            }
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Quantity must be a valid non-negative number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus(); return false;
            }
            dto = new UpdateProductDto { Title = txtTitle.Text.Trim(), UnitPrice = unitPrice, Quantity = quantity };
            return true;
        }

        private async void btnRefresh_Click(object sender, EventArgs e) { await LoadDataAsync(); }
        private void btnBack_Click(object sender, EventArgs e) { this.Close(); }
        private void frmProduct_Load(object sender, EventArgs e) { /* Initial one-time setup */ }
    }
}