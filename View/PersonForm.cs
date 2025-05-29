// Required namespaces for UI elements, service interaction, DTOs, and system utilities.
using Service;         // For PersonService, which handles business logic.
using Service.DTOs;    // For Data Transfer Objects (GetPersonDto, PostPersonDto, etc.) and ServiceResult.
using System;          // For basic system functionalities like EventArgs, Exception, etc.
using System.Collections.Generic; // For List<T>.
using System.Linq;                // For LINQ methods like .Any().
using System.Text;                // For StringBuilder, used in formatting messages.
using System.Windows.Forms;       // For Windows Forms base classes and controls like Form, Button, DataGridView.

// The namespace for the View layer of the application.
namespace View
{
    /// <summary>
    /// Represents the main form for managing Person data.
    /// This form allows users to view, add, edit, and delete person records.
    /// It interacts with the PersonService for all data operations.
    /// </summary>
    public partial class frmPerson : Form // Inherits from System.Windows.Forms.Form.
    {
        // Instance of PersonService to handle business logic and data access for persons.
        // Marked as readonly because it's assigned in the constructor and not changed afterwards.
        private readonly PersonService _personService;

        // Stores the ID of the person currently selected (typically via a single checkbox tick)
        // and intended for an edit operation. Initialized to 0, indicating no person is selected for edit.
        private int _selectedPersonIdForEdit = 0;

        // Constants for DataGridView column names. Using constants helps avoid "magic strings"
        // and makes it easier to refactor if column names change in the designer or setup.
        // These MUST match the 'Name' property of the columns in frmPerson.Designer.cs
        // or how they are named if created programmatically in SetupDataGridViewColumns.
        private const string CheckBoxColumnName = "CheckBox";
        private const string IdColumnName = "colId";         // For the hidden ID column.
        private const string FirstNameColumnName = "personFirstName";
        private const string LastNameColumnName = "personLastName";
        private const string FullNameColumnName = "personFullName"; // For the derived FullName property.

        /// <summary>
        /// Initializes a new instance of the frmPerson class.
        /// This is the default constructor, typically used by Program.cs if not using dependency injection.
        /// It creates its own instance of PersonService.
        /// </summary>
        public frmPerson()
        {
            InitializeComponent(); // Standard WinForms method to set up controls defined in the Designer.
            _personService = new PersonService(); // PersonService internally creates its own PersonServiceModel.
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
                dataGridViewPerson.DataSource = null; // Explicitly ensure the grid is empty on initial load.
                txtFirstName.Clear();          // Clear textboxes.
                txtLastName.Clear();
                UpdateButtonStatesAndTextBoxes(); // Set initial enabled/disabled states for buttons based on the empty state.
            };

            // Wire up DataGridView event handlers for interactive behavior.
            // Note: dataGridViewPerson.CellContentClick is typically wired up in frmPerson.Designer.cs.
            // If it's wired in both places, the handler might execute twice. This code assumes it's handled.
            // The following lines ensure other necessary handlers are attached.

            // Decouple simple row selection (clicking a row without checkbox interaction) from directly populating textboxes.
            // The original dataGridViewPerson_SelectionChanged handler might be removed if its only purpose was textbox population.
            dataGridViewPerson.SelectionChanged -= dataGridViewPerson_SelectionChanged;
            // Add a new handler (or use existing one with modified logic) that primarily updates button states if needed.
            dataGridViewPerson.SelectionChanged += UpdateButtonStatesOnlyOnSelectionChanged;

            dataGridViewPerson.CurrentCellDirtyStateChanged += dataGridViewPerson_CurrentCellDirtyStateChanged;
            dataGridViewPerson.CellValueChanged += dataGridViewPerson_CellValueChanged;
        }

        /// <summary>
        /// Configures the columns for the dataGridViewPerson.
        /// Ensures that AutoGenerateColumns is false and sets DataPropertyName for data-bound columns
        /// and ReadOnly properties to prevent direct grid editing for data cells, while keeping checkboxes interactive.
        /// This method primarily adjusts properties of columns assumed to be defined in the Designer.
        /// </summary>
        private void SetupDataGridViewColumns()
        {
            // Must be false to use manually defined/designer-defined columns and control their properties.
            dataGridViewPerson.AutoGenerateColumns = false;

            // --- CheckBox Column Configuration ---
            // Assumes a column named "CheckBox" (matching CheckBoxColumnName) exists from the designer.
            if (dataGridViewPerson.Columns.Contains(CheckBoxColumnName))
            {
                dataGridViewPerson.Columns[CheckBoxColumnName].ReadOnly = false; // Checkboxes must be interactive.
                // You might also set Width, AutoSizeMode here if not set satisfactorily in the designer.
                // e.g., dataGridViewPerson.Columns[CheckBoxColumnName].Width = 50;
                // dataGridViewPerson.Columns[CheckBoxColumnName].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            }
            else // Fallback: If "CheckBox" column isn't found, add it programmatically.
            {
                var checkBoxCol = new DataGridViewCheckBoxColumn
                {
                    Name = CheckBoxColumnName,
                    HeaderText = "", // No header text for a simple checkbox column.
                    Width = 50,      // Set a fixed width.
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None, // Prevent auto-sizing.
                    ReadOnly = false // Ensure it's interactive.
                };
                dataGridViewPerson.Columns.Insert(0, checkBoxCol); // Insert as the first column.
            }

            // --- ID Column (Hidden) Configuration ---
            // Assumes a column named "colId" (matching IdColumnName) exists.
            if (dataGridViewPerson.Columns.Contains(IdColumnName))
            {
                dataGridViewPerson.Columns[IdColumnName].DataPropertyName = "Id"; // Bind to 'Id' property of GetPersonDto.
                dataGridViewPerson.Columns[IdColumnName].Visible = false;         // Keep it hidden from the user.
                dataGridViewPerson.Columns[IdColumnName].ReadOnly = true;         // ID should not be editable.
            }
            // Note: Similar "else" blocks could be added for other columns if programmatic creation is desired as a fallback.

            // --- Data Column Configuration (FirstName, LastName, FullName) ---
            // Ensure DataPropertyName is set to link grid columns to DTO properties.
            // Ensure ReadOnly is true to prevent direct editing in the grid cells.
            if (dataGridViewPerson.Columns.Contains(FirstNameColumnName))
            {
                dataGridViewPerson.Columns[FirstNameColumnName].DataPropertyName = "FirstName";
                dataGridViewPerson.Columns[FirstNameColumnName].ReadOnly = true;
            }
            if (dataGridViewPerson.Columns.Contains(LastNameColumnName))
            {
                dataGridViewPerson.Columns[LastNameColumnName].DataPropertyName = "LastName";
                dataGridViewPerson.Columns[LastNameColumnName].ReadOnly = true;
            }
            if (dataGridViewPerson.Columns.Contains(FullNameColumnName))
            {
                dataGridViewPerson.Columns[FullNameColumnName].DataPropertyName = "FullName";
                dataGridViewPerson.Columns[FullNameColumnName].ReadOnly = true; // FullName is derived in DTO.
            }

            // Set common DataGridView properties, likely already set in Designer.cs but good to confirm.
            dataGridViewPerson.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewPerson.AllowUserToAddRows = false; // User should use the 'Add' button.
            dataGridViewPerson.RowHeadersVisible = false;  // Hides the row header column on the left.
        }

        /// <summary>
        /// Event handler for SelectionChanged. Primarily updates button states.
        /// Textbox population is driven by checkbox interactions via UpdateButtonStatesAndTextBoxes.
        /// </summary>
        private void UpdateButtonStatesOnlyOnSelectionChanged(object sender, EventArgs e)
        {
            // This handler ensures button states are refreshed based on the overall grid state,
            // but avoids directly populating textboxes just from row navigation.
            UpdateButtonStatesAndTextBoxes();
        }

        /// <summary>
        /// Enables or disables main interactive controls on the form, 
        /// typically used during data operations to prevent concurrent user actions.
        /// </summary>
        /// <param name="enable">True to enable controls, false to disable them.</param>
        private void ToggleControls(bool enable)
        {
            txtFirstName.Enabled = enable;
            txtLastName.Enabled = enable;
            btnAdd.Enabled = enable;
            btnRefresh.Enabled = enable;
            btnBack.Enabled = enable;
            dataGridViewPerson.Enabled = enable;

            // When enabling controls, refresh the state of Edit/Delete buttons.
            // When disabling, explicitly disable Edit/Delete as their state depends on selection.
            if (enable)
            {
                UpdateButtonStatesAndTextBoxes();
            }
            else
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        /// <summary>
        /// Fetches person data using PersonService and populates the DataGridView.
        /// Handles success and failure scenarios from the service call and updates UI accordingly.
        /// </summary>
        private void LoadData()
        {
            ToggleControls(false); // Disable UI during the load operation.
            var result = _personService.GetAllPersons(); // Fetch data via the service.

            if (result.IsSuccess && result.Data != null)
            {
                // Successfully fetched data.
                dataGridViewPerson.DataSource = null;        // Clear existing data source to ensure proper refresh.
                dataGridViewPerson.DataSource = result.Data; // Bind the list of GetPersonDto.
            }
            else
            {
                // Failed to fetch data or no data returned.
                MessageBox.Show(result.Message ?? "An unknown error occurred while loading data.",
                                "Error Loading Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridViewPerson.DataSource = null; // Ensure grid is empty on error.
            }
            // After loading data, clear any previous input field values or selections.
            ClearAllFieldsAndSelections();
            ToggleControls(true); // Re-enable UI controls.
        }

        /// <summary>
        /// Handles the Click event for the "Add" button.
        /// Validates user input from textboxes, creates a PostPersonDto, 
        /// calls the service to add the new person, displays feedback, and refreshes the UI.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // --- Input Validation for new person ---
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please enter both First Name and Last Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus(); // Set focus to the first name field if validation fails.
                return; // Stop further processing.
            }

            // Create the Data Transfer Object for adding the person.
            var postDto = new PostPersonDto()
            {
                FirstName = txtFirstName.Text.Trim(), // Trim whitespace from string inputs.
                LastName = txtLastName.Text.Trim()
            };

            ToggleControls(false); // Disable UI during the service call.
            var result = _personService.AddPerson(postDto); // Call the service method.
            ToggleControls(true);  // Re-enable UI controls.

            if (result.IsSuccess)
            {
                // Display success message (potentially from the service).
                MessageBox.Show(result.Message, "Operation Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); // Refresh the grid to show the newly added person and reset selections.
            }
            else
            {
                // Display error message from the service.
                MessageBox.Show(result.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event for the "Refresh" button.
        /// Reloads person data into the DataGridView and resets associated UI elements.
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData(); // LoadData also calls ClearAllFieldsAndSelections.
            MessageBox.Show("Data has been refreshed.", "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles the Click event for the "Edit" button.
        /// Validates input for the selected person (identified by a single checked checkbox),
        /// calls the service to update the person, displays feedback, and refreshes the UI.
        /// </summary>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            // _selectedPersonIdForEdit is set when exactly one checkbox is ticked (via UpdateButtonStatesAndTextBoxes).
            if (_selectedPersonIdForEdit <= 0)
            {
                MessageBox.Show("Please check the box next to the person you want to edit.", "No Person Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- Input Validation for editing person ---
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("First Name and Last Name cannot be empty for editing.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return;
            }

            // Create DTO for updating the person.
            var updateDto = new UpdatePersonDto
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim()
            };

            ToggleControls(false);
            var result = _personService.UpdatePerson(_selectedPersonIdForEdit, updateDto); // Call service.
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
        /// Retrieves IDs of persons selected via checkboxes, confirms deletion with the user,
        /// calls the service to delete them, provides a summary, and refreshes the UI.
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectedIds = GetSelectedIdsFromCheckboxes(); // Get IDs from currently checked rows.
            if (!selectedIds.Any()) // Check if any persons are selected for deletion.
            {
                MessageBox.Show("Please check the box(es) for the person(s) you want to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Confirm with the user before proceeding with deletion.
            var confirmResult = MessageBox.Show($"Are you sure you want to delete {selectedIds.Count} selected person(s)?",
                                                 "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes) // Proceed only if user clicks "Yes".
            {
                ToggleControls(false);
                int successCount = 0;
                int failCount = 0;
                StringBuilder errors = new StringBuilder(); // To accumulate error messages if any deletions fail.

                // Iterate through each selected ID and attempt to delete the corresponding person.
                foreach (var id in selectedIds)
                {
                    var result = _personService.DeletePerson(id);
                    if (result.IsSuccess)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        errors.AppendLine($"- Person ID {id}: {result.Message}"); // Log specific error.
                    }
                }
                ToggleControls(true);

                // Build a summary message of the delete operation results.
                string summaryMessage = $"{successCount} person(s) deleted successfully.";
                if (failCount > 0)
                {
                    summaryMessage += $"\n{failCount} person(s) could not be deleted:\n{errors.ToString()}";
                }
                MessageBox.Show(summaryMessage, "Delete Operation Complete", MessageBoxButtons.OK,
                                failCount > 0 ? MessageBoxIcon.Error : MessageBoxIcon.Information); // Show Error icon if any failed.
                LoadData(); // Refresh grid and clear selections.
            }
        }

        /// <summary>
        /// Retrieves a list of IDs for all rows where the checkbox in the CheckBoxColumnName is currently checked.
        /// </summary>
        /// <returns>A list of integer IDs of selected persons.</returns>
        private List<int> GetSelectedIdsFromCheckboxes()
        {
            var ids = new List<int>();
            // Ensure the required columns (checkbox and ID) exist before trying to access them.
            if (dataGridViewPerson.Columns.Contains(CheckBoxColumnName) && dataGridViewPerson.Columns.Contains(IdColumnName))
            {
                foreach (DataGridViewRow row in dataGridViewPerson.Rows)
                {
                    // Skip the placeholder row for new entries if it's visible and user can add rows (though disabled here).
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
        /// Handles clicks within DataGridView cells. This version is wired in Designer.cs.
        /// If a checkbox is clicked, it ensures the edit is committed, allowing CellValueChanged to handle UI updates.
        /// Clicks on other cells do not directly populate textboxes to enforce checkbox-driven selection for edit.
        /// </summary>
        private void dataGridViewPerson_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Ignore clicks on the header row.

            // If the click was specifically on a cell in the checkbox column.
            if (dataGridViewPerson.Columns.Contains(CheckBoxColumnName) &&
                e.ColumnIndex == dataGridViewPerson.Columns[CheckBoxColumnName].Index)
            {
                // Commit the checkbox state change immediately. This ensures that if the user clicked 
                // the checkbox, its new state (checked/unchecked) is applied right away.
                // This then triggers the CellValueChanged event, which calls UpdateButtonStatesAndTextBoxes.
                dataGridViewPerson.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            // Per user request: Clicking other data cells does NOT directly populate textboxes.
            // Textbox population is driven by checkbox state changes (handled in UpdateButtonStatesAndTextBoxes,
            // which is called by CellValueChanged for checkboxes).
            // However, we might still want to update button states if a general cell click implies a change in context.
            // UpdateButtonStatesAndTextBoxes(); // Re-evaluating if this call is needed here or if CellValueChanged is sufficient.
            // If SelectionChanged also calls it, it might be redundant here.
            // For now, let CellValueChanged handle checkbox-driven updates.
        }

        /// <summary>
        /// Ensures that changes to a checkbox cell's value are committed immediately
        /// as the cell becomes "dirty" (i.e., its state is changed by user interaction).
        /// </summary>
        private void dataGridViewPerson_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // If the currently active cell is dirty (its value has been changed by the user but not yet committed)
            // AND that cell is in the checkbox column:
            if (dataGridViewPerson.IsCurrentCellDirty &&
                dataGridViewPerson.Columns.Contains(CheckBoxColumnName) &&
                dataGridViewPerson.CurrentCell.OwningColumn.Name == CheckBoxColumnName)
            {
                // Programmatically commit the edit. This is crucial for checkboxes because
                // their ValueChanged event might not fire until the cell loses focus otherwise.
                // Committing here makes the UI more responsive and ensures CellValueChanged fires promptly.
                dataGridViewPerson.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Handles the event fired after a cell's value has been changed and committed.
        /// This is primarily used to react to checkbox state changes, triggering updates
        /// to button states and potentially populating/clearing textboxes.
        /// </summary>
        private void dataGridViewPerson_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the event is for a valid row and specifically for the checkbox column.
            if (e.RowIndex >= 0 &&
                dataGridViewPerson.Columns.Contains(CheckBoxColumnName) &&
                e.ColumnIndex == dataGridViewPerson.Columns[CheckBoxColumnName].Index)
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
        private void dataGridViewPerson_SelectionChanged(object sender, EventArgs e)
        {
            // Textbox population is strictly driven by checkbox state changes via UpdateButtonStatesAndTextBoxes.
            // Calling it here ensures that button states (Edit/Delete) are refreshed if the selection
            // changes by other means (like keyboard navigation), reflecting the overall selection context.
            UpdateButtonStatesAndTextBoxes();
        }

        /// <summary>
        /// Populates the input textboxes (FirstName, LastName) with data from the provided DataGridViewRow.
        /// Also sets the _selectedPersonIdForEdit field with the ID of the person from that row.
        /// </summary>
        /// <param name="row">The DataGridViewRow containing person data.</param>
        private void PopulateFieldsFromRow(DataGridViewRow row)
        {
            // Basic validation to ensure the row and necessary column cells exist and are not null.
            if (row == null || row.IsNewRow ||
                !dataGridViewPerson.Columns.Contains(IdColumnName) ||
                !dataGridViewPerson.Columns.Contains(FirstNameColumnName) ||
                !dataGridViewPerson.Columns.Contains(LastNameColumnName) ||
                row.Cells[IdColumnName].Value == null) // Check specific cell for ID existence
            {
                _selectedPersonIdForEdit = 0; // Reset if row data is invalid or incomplete.
                // Textbox clearing is handled by UpdateButtonStatesAndTextBoxes if this method is called under those conditions.
                // Explicitly clearing here could be redundant if called from UpdateButtonStatesAndTextBoxes.
                // For safety, if this method is called directly and fails, ensure a clean state for selection:
                // txtFirstName.Clear(); 
                // txtLastName.Clear();
                return;
            }

            // Try to parse the ID and populate fields.
            if (int.TryParse(row.Cells[IdColumnName].Value.ToString(), out int id))
            {
                _selectedPersonIdForEdit = id; // Store the ID for a potential edit operation.
                txtFirstName.Text = row.Cells[FirstNameColumnName].Value?.ToString() ?? "";
                txtLastName.Text = row.Cells[LastNameColumnName].Value?.ToString() ?? "";
            }
            else
            {
                // If ID parsing fails, consider it an invalid selection for edit.
                _selectedPersonIdForEdit = 0;
                // txtFirstName.Clear(); // As above, clearing might be handled by the caller.
                // txtLastName.Clear();
            }
        }

        /// <summary>
        /// Unchecks all checkboxes in the DataGridView.
        /// </summary>
        private void ClearCheckboxes()
        {
            if (dataGridViewPerson.Columns.Contains(CheckBoxColumnName))
            {
                foreach (DataGridViewRow row in dataGridViewPerson.Rows)
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
            txtFirstName.Clear();
            txtLastName.Clear();
            _selectedPersonIdForEdit = 0; // Reset the ID used for editing.
            ClearCheckboxes();             // Uncheck all checkboxes.

            // If there are rows, clear any visual selection highlight in the grid.
            if (dataGridViewPerson.Rows.Count > 0)
            {
                dataGridViewPerson.ClearSelection();
                // Also remove focus from any cell to avoid confusion after a reset.
                if (dataGridViewPerson.CurrentCell != null)
                    dataGridViewPerson.CurrentCell = null;
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
                // Find the single checked row and populate textboxes and _selectedPersonIdForEdit from it.
                foreach (DataGridViewRow row in dataGridViewPerson.Rows)
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
                txtFirstName.Clear();    // Clear textboxes as no single item is selected for edit via checkbox.
                txtLastName.Clear();
                _selectedPersonIdForEdit = 0; // Reset the ID for editing, as selection is not for a single item.
            }
        }

        /// <summary>
        /// Handles the Click event for the "Back" button. Closes the current form.
        /// </summary>
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close(); // Closes this frmPerson instance.
        }

        /// <summary>
        /// Handles the Load event of the form. 
        /// This event is typically wired in the Designer.cs.
        /// Any one-time setup that needs to occur when the form loads can be placed here,
        /// though much of the initial UI setup is handled in InitializeForm and the Shown event in this example.
        /// </summary>
        private void frmPerson_Load(object sender, EventArgs e)
        {
            // Any additional one-time setup code for form load can go here.
            // For example, if not done in Shown or InitializeForm, could call ClearAllFieldsAndSelections().
        }
    }
}