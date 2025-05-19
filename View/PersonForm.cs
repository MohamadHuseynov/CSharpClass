using Service; 
using Service.DTOs;


namespace View
{
    public partial class frmPerson : Form
    {
        private readonly IPersonService _personService;
        private int _selectedPersonIdForEdit = 0;

        // --- These names MUST match the (Name) property of your DataGridView columns in the Designer ---
        private const string CheckBoxColumnName = "CheckBox";
        private const string IdColumnName = "colId";
        private const string FirstNameColumnName = "personFirstName";
        private const string LastNameColumnName = "personLastName";
        private const string FullNameColumnName = "personFullName";
        // --- End of column name definitions ---

        public frmPerson(IPersonService personService)
        {
            InitializeComponent();
            _personService = personService ?? throw new ArgumentNullException(nameof(personService));

            this.Shown += frmPerson_Shown;
            this.dataGridViewPerson.CurrentCellDirtyStateChanged += dataGridViewPerson_CurrentCellDirtyStateChanged;
            this.dataGridViewPerson.CellValueChanged += dataGridViewPerson_CellValueChanged;
            // CellClick is generally not needed for checkbox logic if the above two are used correctly.
            // this.dataGridViewPerson.CellClick += dataGridViewPerson_CellClick;
        }

        private void dataGridViewPerson_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewPerson.IsCurrentCellDirty)
            {
                // If the dirty cell is our checkbox cell, commit the edit immediately.
                // This will then reliably trigger CellValueChanged.
                if (dataGridViewPerson.CurrentCell is DataGridViewCheckBoxCell &&
                    dataGridViewPerson.CurrentCell.OwningColumn.Name == CheckBoxColumnName)
                {
                    dataGridViewPerson.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }

        private void dataGridViewPerson_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // This event fires AFTER a cell's value has been committed.
            // We are interested if it's our checkbox column.
            if (e.RowIndex >= 0 && dataGridViewPerson.Columns[e.ColumnIndex].Name == CheckBoxColumnName)
            {
                // A checkbox state has changed and been committed.
                // Now, update the UI state based on all checkbox selections.
                UpdateButtonStatesAndTextBoxes();
            }
        }

        private void frmPerson_Shown(object sender, EventArgs e)
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
                var result = await _personService.GetAllPersonsAsync();

                // Detach event handlers that might fire during programmatic row changes
                this.dataGridViewPerson.CurrentCellDirtyStateChanged -= dataGridViewPerson_CurrentCellDirtyStateChanged;
                this.dataGridViewPerson.CellValueChanged -= dataGridViewPerson_CellValueChanged;

                dataGridViewPerson.Rows.Clear();

                if (result.IsSuccess && result.Data != null && result.Data.Any())
                {
                    foreach (var personDto in result.Data)
                    {
                        // Adding row: Checkbox, ID, FirstName, LastName, FullName
                        // The order of cells here is by index, but we access them by name later.
                        // Ensure your AddRange in Designer or manual column add matches this conceptual order
                        // if you were to rely on indices (but using names is better).
                        dataGridViewPerson.Rows.Add(false, personDto.Id, personDto.FirstName, personDto.LastName, personDto.FullName);
                    }
                }
                else if (!result.IsSuccess)
                {
                    MessageBox.Show(result.Message, "Error Loading Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred while loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-attach event handlers
                this.dataGridViewPerson.CurrentCellDirtyStateChanged += dataGridViewPerson_CurrentCellDirtyStateChanged;
                this.dataGridViewPerson.CellValueChanged += dataGridViewPerson_CellValueChanged;

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
            txtFirstName.Enabled = enabled;
            txtLastName.Enabled = enabled;
            dataGridViewPerson.Enabled = enabled;
            btnBack.Enabled = enabled;

            if (enabled) UpdateButtonStatesAndTextBoxes();
            else { btnEdit.Enabled = false; btnDelete.Enabled = false; }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputFields()) return;
            var createDto = new CreatePersonDto
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim()
            };
            this.Cursor = Cursors.WaitCursor;
            ToggleGeneralControls(false);
            var result = await _personService.AddPersonAsync(createDto);
            this.Cursor = Cursors.Default;
            if (result.IsSuccess) { await LoadDataAsync(); }
            else { ToggleGeneralControls(true); MessageBox.Show(result.Message, "Error Adding", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            if (_selectedPersonIdForEdit == 0)
            {
                MessageBox.Show("Please select exactly one person using the checkbox to edit.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ValidateInputFields()) return;
            var updateDto = new UpdatePersonDto
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim()
            };
            this.Cursor = Cursors.WaitCursor;
            ToggleGeneralControls(false);
            var result = await _personService.UpdatePersonAsync(_selectedPersonIdForEdit, updateDto);
            this.Cursor = Cursors.Default;
            if (result.IsSuccess) { await LoadDataAsync(); }
            else { ToggleGeneralControls(true); MessageBox.Show(result.Message, "Error Editing", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var idsToDelete = GetSelectedPersonIdsFromGridCheckboxes();
            if (!idsToDelete.Any())
            {
                MessageBox.Show("Please select at least one person using the checkboxes to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show($"Are you sure you want to delete {idsToDelete.Count} selected person(s)?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;
                ToggleGeneralControls(false);
                int successCount = 0; int failCount = 0; string errorDetails = "";
                foreach (var idInList in idsToDelete)
                {
                    var result = await _personService.DeletePersonAsync(idInList);
                    if (result.IsSuccess) successCount++;
                    else { failCount++; errorDetails += $"ID {idInList}: {result.Message}\n"; }
                }
                this.Cursor = Cursors.Default;
                string finalMessage = "";
                if (successCount > 0) finalMessage += $"{successCount} person(s) deleted successfully.\n";
                if (failCount > 0) finalMessage += $"{failCount} person(s) failed to delete.\nDetails:\n{errorDetails}";
                MessageBox.Show(finalMessage.Trim(), "Delete Result", MessageBoxButtons.OK, (failCount > 0) ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
                await LoadDataAsync();
            }
        }

        private void UpdateButtonStatesAndTextBoxes()
        {
            var checkedIds = GetSelectedPersonIdsFromGridCheckboxes();
            int checkedCount = checkedIds.Count;

            DataGridViewRow firstSelectedRowForEditVisuals = null;
            _selectedPersonIdForEdit = 0;

            if (checkedCount == 1)
            {
                btnEdit.Enabled = true;
                btnDelete.Enabled = true; // Also enable delete if one is checked

                int singleId = checkedIds.First();
                _selectedPersonIdForEdit = singleId;

                foreach (DataGridViewRow row in dataGridViewPerson.Rows)
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
            else // checkedCount == 0
            {
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }

            if (firstSelectedRowForEditVisuals != null)
            {
                txtFirstName.Text = firstSelectedRowForEditVisuals.Cells[FirstNameColumnName].Value?.ToString() ?? "";
                txtLastName.Text = firstSelectedRowForEditVisuals.Cells[LastNameColumnName].Value?.ToString() ?? "";
            }
            else
            {
                txtFirstName.Clear();
                txtLastName.Clear();
            }
        }

        private List<int> GetSelectedPersonIdsFromGridCheckboxes()
        {
            var ids = new List<int>();
            foreach (DataGridViewRow row in dataGridViewPerson.Rows)
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
            txtFirstName.Clear();
            txtLastName.Clear();
            _selectedPersonIdForEdit = 0;

            if (dataGridViewPerson.Rows.Count > 0)
            {
                this.dataGridViewPerson.CurrentCellDirtyStateChanged -= dataGridViewPerson_CurrentCellDirtyStateChanged;
                this.dataGridViewPerson.CellValueChanged -= dataGridViewPerson_CellValueChanged;
                foreach (DataGridViewRow row in dataGridViewPerson.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    if (checkBoxCell != null) { checkBoxCell.Value = false; }
                }
                this.dataGridViewPerson.CurrentCellDirtyStateChanged += dataGridViewPerson_CurrentCellDirtyStateChanged;
                this.dataGridViewPerson.CellValueChanged += dataGridViewPerson_CellValueChanged;
            }
            // UpdateButtonStatesAndTextBoxes(); // Will be called by the method that invoked this clear (e.g., LoadDataAsync, Shown)
        }

        private bool ValidateInputFields()
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("First name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus(); return false;
            }
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Last name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLastName.Focus(); return false;
            }
            return true;
        }

        private async void btnRefresh_Click(object sender, EventArgs e) { await LoadDataAsync(); }
        private void btnBack_Click(object sender, EventArgs e) { this.Close(); }
        private void frmPerson_Load(object sender, EventArgs e) { /* Initial one-time setup */ }

       
    }
}