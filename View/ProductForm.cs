// Required namespaces for UI elements, service interaction, DTOs, and system utilities.
using Service;         // For ProductService, which handles business logic for products.
using Service.DTOs;    // For Product Data Transfer Objects (GetProductDto, PostProductDto, etc.) and ServiceResult.
using System;          // For basic system functionalities like EventArgs, Exception, etc.
using System.Collections.Generic; // For List<T>.
using System.Linq;                // For LINQ methods like .Any().
using System.Text;                // For StringBuilder, used in formatting messages.
using System.Windows.Forms;       // For Windows Forms base classes and controls like Form, Button, DataGridView.

// The namespace for the View layer of the application.
namespace View
{
    /// <summary>
    /// Represents the main form for managing Product data.
    /// This form allows users to view, add, edit, and delete products.
    /// It interacts with the ProductService for all product-related operations,
    /// following the UI patterns established in frmPerson.
    /// </summary>
    public partial class frmProduct : Form // Inherits from System.Windows.Forms.Form.
    {
        // Service layer instance to handle business logic and data access for products.
        // Marked as readonly because it's assigned in the constructor and not changed afterwards.
        private readonly ProductService _productService;

        // Stores the ID of the product currently selected via a single checkbox tick, 
        // intended for an edit operation. Initialized to 0, indicating no product is selected for edit.
        private int _selectedProductIdForEdit = 0;

        // Constants for DataGridView column names to ensure consistency and avoid "magic strings".
        // These names MUST match the 'Name' property of the columns in frmProduct.Designer.cs
        // or how they are named if created programmatically in SetupDataGridViewColumns.
        private const string CheckBoxColumnName = "CheckBox";
        private const string IdColumnName = "colId";         // For the hidden ID column.
        private const string TitleColumnName = "productTitle";
        private const string UnitPriceColumnName = "productUnitPrice";
        private const string QuantityColumnName = "productQuantity";

        /// <summary>
        /// Initializes a new instance of the frmProduct class.
        /// This is the default constructor, typically used by Program.cs if not using dependency injection.
        /// It creates its own instance of ProductService.
        /// </summary>
        public frmProduct()
        {
            InitializeComponent(); // Standard WinForms method to set up controls defined in the Designer.
            _productService = new ProductService(); // ProductService internally creates its own ProductServiceModel.
            InitializeForm(); // Call for custom form setup logic.
        }

        /// <summary>
        /// Performs additional initialization for the form after standard component initialization.
        /// This includes setting up DataGridView columns and attaching event handlers for form behavior.
        /// </summary>
        private void InitializeForm()
        {
            SetupDataGridViewColumns(); // Configure columns for the DataGridView.

            // Attach an event handler to the 'Shown' event of the form.
            // This code runs once the form is fully displayed for the first time,
            // ensuring that initial UI state is set correctly after the form is visible.
            this.Shown += (s, e) =>
            {
                ClearAllFieldsAndSelections(); // Set a clean initial state for input fields and grid selections.
                ToggleControls(true);          // Enable interactive controls.
                dataGridViewProduct.DataSource = null; // Explicitly ensure the grid is empty on initial load.
                txtTitle.Clear();              // Clear textboxes.
                txtUnitPrice.Clear();
                txtQuantity.Clear();
                UpdateButtonStatesAndTextBoxes(); // Set initial enabled/disabled states for buttons based on the empty state.
            };

            // Wire up DataGridView event handlers for interactive behavior.
            // Note: dataGridViewProduct.CellContentClick is typically wired up in frmProduct.Designer.cs.
            // If it's wired in both places, the handler might execute twice. Ensure single wiring.
            dataGridViewProduct.SelectionChanged += dataGridViewProduct_SelectionChanged;
            dataGridViewProduct.CurrentCellDirtyStateChanged += dataGridViewProduct_CurrentCellDirtyStateChanged;
            dataGridViewProduct.CellValueChanged += dataGridViewProduct_CellValueChanged;
        }

        /// <summary>
        /// Configures the columns for the dataGridViewProduct.
        /// Ensures that AutoGenerateColumns is false. It then checks for existing columns (presumably from the designer)
        /// and sets their DataPropertyName and ReadOnly properties. If key columns are missing,
        /// it provides a fallback to add them programmatically.
        /// </summary>
        private void SetupDataGridViewColumns()
        {
            // Must be false to use manually defined/designer-defined columns and control their properties.
            dataGridViewProduct.AutoGenerateColumns = false;

            // --- CheckBox Column Configuration ---
            // Assumes a column named "CheckBox" (matching CheckBoxColumnName) exists from the designer.
            if (dataGridViewProduct.Columns.Contains(CheckBoxColumnName))
            {
                dataGridViewProduct.Columns[CheckBoxColumnName].ReadOnly = false; // Checkboxes must be interactive.
                dataGridViewProduct.Columns[CheckBoxColumnName].Width = 50;       // Set preferred width.
                dataGridViewProduct.Columns[CheckBoxColumnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None; // Use fixed width.
            }
            else // Fallback: If "CheckBox" column isn't found, add it programmatically.
            {
                var checkBoxCol = new DataGridViewCheckBoxColumn
                {
                    Name = CheckBoxColumnName,
                    HeaderText = "",
                    Width = 50,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    ReadOnly = false
                };
                dataGridViewProduct.Columns.Insert(0, checkBoxCol); // Insert as the first column.
            }

            // --- ID Column (Hidden) Configuration ---
            // Assumes a column named "colId" (matching IdColumnName) might exist from the designer.
            if (!dataGridViewProduct.Columns.Contains(IdColumnName))
            {
                // If not found, add it programmatically.
                var idCol = new DataGridViewTextBoxColumn
                {
                    Name = IdColumnName,
                    DataPropertyName = "Id", // Bind to 'Id' property of GetProductDto.
                    Visible = false,
                    ReadOnly = true               // Hidden and not editable.
                };
                dataGridViewProduct.Columns.Insert(1, idCol); // Insert after the checkbox column.
            }
            else
            {
                // Ensure properties of the designer-added ID column are correct.
                dataGridViewProduct.Columns[IdColumnName].DataPropertyName = "Id";
                dataGridViewProduct.Columns[IdColumnName].Visible = false;
                dataGridViewProduct.Columns[IdColumnName].ReadOnly = true;
            }

            // --- Title Column Configuration ---
            // Assumes a column named "productTitle" (matching TitleColumnName) exists from the designer.
            if (dataGridViewProduct.Columns.Contains(TitleColumnName))
            {
                dataGridViewProduct.Columns[TitleColumnName].DataPropertyName = "Title"; // Bind to 'Title'.
                dataGridViewProduct.Columns[TitleColumnName].ReadOnly = true;        // Prevent direct grid edit.
            }

            // --- UnitPrice Column Configuration ---
            // Assumes a column named "productUnitPrice" (matching UnitPriceColumnName) exists.
            if (dataGridViewProduct.Columns.Contains(UnitPriceColumnName))
            {
                dataGridViewProduct.Columns[UnitPriceColumnName].DataPropertyName = "UnitPrice"; // Bind to 'UnitPrice'.
                dataGridViewProduct.Columns[UnitPriceColumnName].ReadOnly = true;
            }

            // --- Quantity Column Configuration ---
            // Assumes a column named "productQuantity" (matching QuantityColumnName) exists.
            if (dataGridViewProduct.Columns.Contains(QuantityColumnName))
            {
                dataGridViewProduct.Columns[QuantityColumnName].DataPropertyName = "Quantity"; // Bind to 'Quantity'.
                dataGridViewProduct.Columns[QuantityColumnName].ReadOnly = true;
            }

            // Set common DataGridView properties.
            dataGridViewProduct.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            // These are often set in Designer.cs but are good to confirm.
            // dataGridViewProduct.AllowUserToAddRows = false; (Set in Designer for this project)
            // dataGridViewProduct.RowHeadersVisible = false;  (Set in Designer for this project)
        }

        /// <summary>
        /// Enables or disables main interactive controls on the form, 
        /// typically used during data operations to prevent concurrent user actions.
        /// </summary>
        /// <param name="enable">True to enable controls, false to disable them.</param>
        private void ToggleControls(bool enable)
        {
            txtTitle.Enabled = enable;
            txtUnitPrice.Enabled = enable;
            txtQuantity.Enabled = enable;
            btnAdd.Enabled = enable;
            btnRefresh.Enabled = enable;
            btnBack.Enabled = enable;
            dataGridViewProduct.Enabled = enable;

            // When enabling controls, refresh the state of Edit/Delete buttons.
            // When disabling, explicitly disable Edit/Delete as their state depends on selection.
            if (enable)
                UpdateButtonStatesAndTextBoxes();
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        /// <summary>
        /// Fetches product data using ProductService and populates the DataGridView.
        /// Handles success and failure scenarios from the service call and updates UI accordingly.
        /// </summary>
        private void LoadData()
        {
            ToggleControls(false); // Disable UI during the load operation.
            var result = _productService.GetAllProducts(); // Fetch data via the service.

            if (result.IsSuccess && result.Data != null)
            {
                // Successfully fetched data.
                dataGridViewProduct.DataSource = null;        // Clear existing data source to ensure proper refresh.
                dataGridViewProduct.DataSource = result.Data; // Bind the list of GetProductDto.
            }
            else
            {
                // Failed to fetch data or no data returned.
                MessageBox.Show(result.Message ?? "An unknown error occurred while loading products.",
                                "Error Loading Products", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridViewProduct.DataSource = null; // Ensure grid is empty on error.
            }
            // Note: ClearAllFieldsAndSelections() is typically called by the initiating action (e.g., btnRefresh_Click) 
            // after LoadData completes, to ensure the UI is reset consistently.
            ToggleControls(true); // Re-enable UI controls.
        }

        /// <summary>
        /// Handles the Click event for the "Add" button.
        /// Validates user input from textboxes, creates a PostProductDto, 
        /// calls the service to add the new product, displays feedback, and refreshes the UI.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // --- Input Validation for new product ---
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a Title.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus(); // Set focus to the problematic field.
                return; // Stop further processing.
            }
            // Validate UnitPrice: must be a valid integer and non-negative.
            if (!int.TryParse(txtUnitPrice.Text, out int unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Please enter a valid positive Unit Price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUnitPrice.Focus();
                return;
            }
            // Validate Quantity: must be a valid integer and non-negative.
            if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Please enter a valid positive Quantity.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                return;
            }

            // Create the Data Transfer Object for adding the product.
            var postDto = new PostProductDto()
            {
                Title = txtTitle.Text.Trim(), // Trim whitespace from string inputs.
                UnitPrice = unitPrice,
                Quantity = quantity
            };

            ToggleControls(false); // Disable UI during the service call.
            var result = _productService.AddProduct(postDto); // Call the service method.
            ToggleControls(true);  // Re-enable UI controls.

            if (result.IsSuccess)
            {
                // Display success message (potentially from the service).
                MessageBox.Show(result.Message, "Operation Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); // Refresh the grid to show the newly added product.
                ClearAllFieldsAndSelections(); // Clear input fields and selections after successful add.
            }
            else
            {
                // Display error message from the service.
                MessageBox.Show(result.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event for the "Refresh" button.
        /// Reloads product data into the DataGridView and resets associated UI elements.
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData(); // Load data from the service.
            ClearAllFieldsAndSelections(); // Ensure a clean state after refresh, as per user request.
            MessageBox.Show("Data has been refreshed.", "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles the Click event for the "Edit" button.
        /// Validates input for the selected product (identified by a single checked checkbox),
        /// calls the service to update it, displays feedback, and refreshes the UI.
        /// </summary>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            // _selectedProductIdForEdit is set when exactly one checkbox is ticked (via UpdateButtonStatesAndTextBoxes).
            if (_selectedProductIdForEdit <= 0)
            {
                MessageBox.Show("Please check the box next to the product you want to edit.", "No Product Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // --- Input Validation for editing product ---
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

            // Create DTO for updating the product.
            var updateDto = new UpdateProductDto
            {
                Title = txtTitle.Text.Trim(),
                UnitPrice = unitPrice,
                Quantity = quantity
            };

            ToggleControls(false);
            var result = _productService.UpdateProduct(_selectedProductIdForEdit, updateDto); // Call service.
            ToggleControls(true);

            if (result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); // Refresh grid and clear selections.
            }
            else
            {
                MessageBox.Show(result.Message, "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event for the "Delete" button.
        /// Retrieves IDs of products selected via checkboxes, confirms deletion with the user,
        /// calls the service to delete them, provides a summary, and refreshes the UI.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedIds = GetSelectedIdsFromCheckboxes(); // Get IDs from currently checked rows.
            if (!selectedIds.Any()) // Check if any products are selected for deletion.
            {
                MessageBox.Show("Please check the box(es) for the product(s) you want to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Confirm with the user before proceeding with deletion of multiple items.
            var confirmResult = MessageBox.Show($"Are you sure you want to delete {selectedIds.Count} selected product(s)?",
                                                 "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes) // Proceed only if user clicks "Yes".
            {
                ToggleControls(false);
                int successCount = 0;
                int failCount = 0;
                StringBuilder errors = new StringBuilder(); // To accumulate error messages if any deletions fail.

                // Iterate through each selected ID and attempt to delete the corresponding product.
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
                        errors.AppendLine($"- Product ID {id}: {result.Message}"); // Log specific error for this ID.
                    }
                }
                ToggleControls(true);

                // Build a summary message of the delete operation results.
                string summaryMessage = $"{successCount} product(s) deleted successfully.";
                if (failCount > 0)
                {
                    summaryMessage += $"\n{failCount} product(s) could not be deleted:\n{errors.ToString()}";
                }
                MessageBox.Show(summaryMessage, "Delete Operation Complete", MessageBoxButtons.OK,
                                failCount > 0 ? MessageBoxIcon.Error : MessageBoxIcon.Information); // Show Error icon if any failed.
                LoadData(); // Refresh grid and clear selections.
            }
        }

        /// <summary>
        /// Retrieves a list of product IDs from rows where the checkbox in the CheckBoxColumnName is currently checked.
        /// </summary>
        /// <returns>A list of integer IDs of selected products.</returns>
        private List<int> GetSelectedIdsFromCheckboxes()
        {
            var ids = new List<int>();
            // Ensure the required columns (checkbox and ID) exist before trying to access them.
            if (dataGridViewProduct.Columns.Contains(CheckBoxColumnName) && dataGridViewProduct.Columns.Contains(IdColumnName))
            {
                foreach (DataGridViewRow row in dataGridViewProduct.Rows)
                {
                    // Skip the placeholder row for new entries if it's visible (though AllowUserToAddRows is false).
                    if (row.IsNewRow) continue;

                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    // Check if the checkbox cell exists and its value is true (checked).
                    if (checkBoxCell?.Value != null && Convert.ToBoolean(checkBoxCell.Value))
                    {
                        // Try to parse the ID from the hidden ID column.
                        if (row.Cells[IdColumnName].Value != null &&
                            int.TryParse(row.Cells[IdColumnName].Value.ToString(), out int id))
                        {
                            ids.Add(id);
                        }
                    }
                }
            }
            return ids;
        }

        /// <summary>
        /// Handles clicks within DataGridView cells. This is wired in Designer.cs.
        /// If a checkbox is clicked, it ensures the edit is committed, allowing CellValueChanged to handle subsequent UI updates.
        /// Other cell clicks do not directly populate textboxes in this UI design.
        /// </summary>
        private void dataGridViewProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Ignore clicks on the header row.

            // If the click was specifically on a cell in the checkbox column.
            if (dataGridViewProduct.Columns.Contains(CheckBoxColumnName) &&
                e.ColumnIndex == dataGridViewProduct.Columns[CheckBoxColumnName].Index)
            {
                // Commit the checkbox state change immediately. This ensures that if the user clicked 
                // the checkbox, its new state (checked/unchecked) is applied right away.
                // This then triggers the CellValueChanged event, which calls UpdateButtonStatesAndTextBoxes.
                dataGridViewProduct.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            // Per user request: Clicking other data cells does NOT directly populate textboxes.
            // Textbox population is driven by checkbox state changes.
        }

        /// <summary>
        /// Ensures that changes to a checkbox cell's value are committed immediately
        /// as the cell becomes "dirty" (i.e., its state is changed by user interaction but not yet formally saved).
        /// </summary>
        private void dataGridViewProduct_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // If the currently active cell is dirty (its value has been changed by the user but not yet committed)
            // AND that cell is in the checkbox column:
            if (dataGridViewProduct.IsCurrentCellDirty &&
                dataGridViewProduct.Columns.Contains(CheckBoxColumnName) &&
                dataGridViewProduct.CurrentCell.OwningColumn.Name == CheckBoxColumnName)
            {
                // Programmatically commit the edit. This is crucial for checkboxes because
                // their ValueChanged event might not fire until the cell loses focus otherwise.
                // Committing here makes the UI more responsive and ensures CellValueChanged fires promptly.
                dataGridViewProduct.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Handles the event fired after a cell's value has been changed and committed.
        /// This is primarily used to react to checkbox state changes, triggering updates
        /// to button states and potentially populating/clearing textboxes.
        /// </summary>
        private void dataGridViewProduct_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the event is for a valid row and specifically for the checkbox column.
            if (e.RowIndex >= 0 &&
                dataGridViewProduct.Columns.Contains(CheckBoxColumnName) &&
                e.ColumnIndex == dataGridViewProduct.Columns[CheckBoxColumnName].Index)
            {
                // A checkbox state has changed, so update button states and 
                // (if applicable) populate/clear textboxes based on the new selection.
                UpdateButtonStatesAndTextBoxes();
            }
        }

        /// <summary>
        /// Handles changes in the DataGridView's current row selection (e.g., user navigates with arrow keys).
        /// This method primarily ensures button states are refreshed, but does not directly populate textboxes
        /// as per the requirement that textboxes are driven by checkbox state for editing.
        /// </summary>
        private void dataGridViewProduct_SelectionChanged(object sender, EventArgs e)
        {
            // Textbox population is strictly driven by checkbox state changes via UpdateButtonStatesAndTextBoxes.
            // Calling it here ensures that button states (Edit/Delete) are refreshed if the selection
            // changes by other means (like keyboard navigation), reflecting the overall selection context.
            UpdateButtonStatesAndTextBoxes();
        }

        /// <summary>
        /// Populates the input textboxes (Title, UnitPrice, Quantity) with data from the provided DataGridViewRow.
        /// Also sets the _selectedProductIdForEdit field with the ID of the product from that row.
        /// </summary>
        /// <param name="row">The DataGridViewRow containing product data.</param>
        private void PopulateFieldsFromRow(DataGridViewRow row)
        {
            // Basic validation to ensure the row and necessary column cells exist and are not null.
            if (row == null || row.IsNewRow ||
                !dataGridViewProduct.Columns.Contains(IdColumnName) || row.Cells[IdColumnName].Value == null ||
                !dataGridViewProduct.Columns.Contains(TitleColumnName) ||
                !dataGridViewProduct.Columns.Contains(UnitPriceColumnName) ||
                !dataGridViewProduct.Columns.Contains(QuantityColumnName))
            {
                _selectedProductIdForEdit = 0; // Reset if row data is invalid or incomplete.
                // Textbox clearing will be handled by UpdateButtonStatesAndTextBoxes if this leads to checkedCount != 1.
                return;
            }

            // Try to parse the ID and populate fields.
            if (int.TryParse(row.Cells[IdColumnName].Value.ToString(), out int id))
            {
                _selectedProductIdForEdit = id; // Store the ID for a potential edit operation.
                txtTitle.Text = row.Cells[TitleColumnName].Value?.ToString() ?? "";
                txtUnitPrice.Text = row.Cells[UnitPriceColumnName].Value?.ToString() ?? "";
                txtQuantity.Text = row.Cells[QuantityColumnName].Value?.ToString() ?? "";
            }
            else
            {
                // If ID parsing fails, consider it an invalid selection for edit.
                _selectedProductIdForEdit = 0;
                // Textboxes will be cleared by UpdateButtonStatesAndTextBoxes if this state persists.
            }
        }

        /// <summary>
        /// Unchecks all checkboxes in the DataGridView.
        /// </summary>
        private void ClearCheckboxes()
        {
            if (dataGridViewProduct.Columns.Contains(CheckBoxColumnName))
            {
                foreach (DataGridViewRow row in dataGridViewProduct.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    if (checkBoxCell != null)
                    {
                        checkBoxCell.Value = false; // Set to false to uncheck. Avoid setting to null.
                    }
                }
            }
        }

        /// <summary>
        /// Resets the form to a clean state: clears all input textboxes,
        /// unchecks all checkboxes in the grid, clears any grid selection,
        /// and resets the ID used for editing.
        /// </summary>
        private void ClearAllFieldsAndSelections()
        {
            txtTitle.Clear();
            txtUnitPrice.Clear();
            txtQuantity.Clear();
            _selectedProductIdForEdit = 0; // Reset the ID used for editing.
            ClearCheckboxes();             // Uncheck all checkboxes.

            // If there are rows, clear any visual selection highlight in the grid.
            if (dataGridViewProduct.Rows.Count > 0)
            {
                dataGridViewProduct.ClearSelection();
                // Also remove focus from any cell to avoid confusion after a reset.
                if (dataGridViewProduct.CurrentCell != null)
                    dataGridViewProduct.CurrentCell = null;
            }
            UpdateButtonStatesAndTextBoxes(); // Update button states to reflect the cleared state.
        }

        /// <summary>
        /// Centralized logic to update the enabled state of Edit/Delete buttons
        /// AND to populate or clear textboxes based on the number of currently checked checkboxes.
        /// Textboxes are populated only if exactly one checkbox is ticked.
        /// </summary>
        private void UpdateButtonStatesAndTextBoxes()
        {
            var selectedIdsFromCheckboxes = GetSelectedIdsFromCheckboxes();
            int checkedCount = selectedIdsFromCheckboxes.Count;

            // Delete button is enabled if one or more checkboxes are ticked.
            btnDelete.Enabled = checkedCount > 0;

            if (checkedCount == 1) // Exactly one checkbox is ticked.
            {
                btnEdit.Enabled = true; // Enable the Edit button.
                // Find the single checked row and populate textboxes and _selectedProductIdForEdit from it.
                foreach (DataGridViewRow row in dataGridViewProduct.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    // Ensure cell and value are not null before attempting to convert.
                    if (checkBoxCell?.Value != null && Convert.ToBoolean(checkBoxCell.Value))
                    {
                        PopulateFieldsFromRow(row);
                        break; // Found the single checked row, no need to continue loop.
                    }
                }
            }
            else // Zero or more than one checkbox is ticked.
            {
                btnEdit.Enabled = false; // Disable Edit button.
                txtTitle.Clear();        // Clear textboxes as no single item is selected for edit via checkbox.
                txtUnitPrice.Clear();
                txtQuantity.Clear();
                _selectedProductIdForEdit = 0; // Reset the ID for editing, as selection is not for a single item.
            }
        }

        /// <summary>
        /// Handles the Click event for the "Back" button. Closes the current form.
        /// </summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); // Closes this frmProduct instance.
        }

        /// <summary>
        /// Handles the Load event of the form. 
        /// This event is typically wired in the Designer.cs.
        /// Any one-time setup that needs to occur when the form loads can be placed here.
        /// In this design, most initial setup is deferred to the 'Shown' event via InitializeForm().
        /// </summary>
        private void frmProduct_Load(object sender, EventArgs e)
        {
            // This method is present in your original frmPerson.cs.
            // You can add any specific one-time load logic here if needed,
            // though the InitializeForm() method called from the constructor handles most setup.
        }
    }
}