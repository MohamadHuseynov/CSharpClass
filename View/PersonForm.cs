using Service;         // For PersonService
using Service.DTOs;    // For DTOs and ServiceResult
using System.Text;


namespace View
{
    public partial class frmPerson : Form
    {
        private readonly PersonService _personService;
        private int _selectedPersonIdForEdit = 0;

        private const string CheckBoxColumnName = "CheckBox";
        private const string IdColumnName = "colId";
        private const string FirstNameColumnName = "personFirstName";
        private const string LastNameColumnName = "personLastName";
        private const string FullNameColumnName = "personFullName";

        public frmPerson()
        {
            InitializeComponent();
            _personService = new PersonService();
            InitializeForm();
        }

        private void InitializeForm()
        {
            SetupDataGridViewColumns();
            this.Shown += (s, e) =>
            {
                ClearAllFieldsAndSelections();
                ToggleControls(true);
                dataGridViewPerson.DataSource = null; // Ensure grid is empty initially
                txtFirstName.Clear();
                txtLastName.Clear();
                UpdateButtonStatesAndTextBoxes();
            };

            // dataGridViewPerson.CellContentClick is wired in your Designer.cs
            // We'll rely more on CellValueChanged for checkboxes.
            dataGridViewPerson.SelectionChanged -= dataGridViewPerson_SelectionChanged; // Decouple row selection from populating textboxes
            dataGridViewPerson.SelectionChanged += UpdateButtonStatesOnlyOnSelectionChanged; // Optional: For button states if needed

            dataGridViewPerson.CurrentCellDirtyStateChanged += dataGridViewPerson_CurrentCellDirtyStateChanged;
            dataGridViewPerson.CellValueChanged += dataGridViewPerson_CellValueChanged;
        }

        // In frmPerson.cs

        private void SetupDataGridViewColumns()
        {
            dataGridViewPerson.AutoGenerateColumns = false;

            // If you clear and re-add columns here, set ReadOnly on text columns:
            // dataGridViewPerson.Columns.Clear(); 

            // CheckBox Column - Should NOT be ReadOnly for interaction
            if (dataGridViewPerson.Columns.Contains(CheckBoxColumnName))
            {
                dataGridViewPerson.Columns[CheckBoxColumnName].ReadOnly = false; // Explicitly ensure it's not read-only
            }
            else
            {
                var checkBoxCol = new DataGridViewCheckBoxColumn
                {
                    Name = CheckBoxColumnName,
                    HeaderText = "",
                    Width = 50,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                    ReadOnly = false // Checkboxes need to be interactive
                };
                dataGridViewPerson.Columns.Insert(0, checkBoxCol); // Insert at the beginning if not already there
            }

            // ID Column (Hidden and can be ReadOnly)
            if (dataGridViewPerson.Columns.Contains(IdColumnName))
            {
                dataGridViewPerson.Columns[IdColumnName].DataPropertyName = "Id";
                dataGridViewPerson.Columns[IdColumnName].Visible = false;
                dataGridViewPerson.Columns[IdColumnName].ReadOnly = true; // ID column should be read-only
            }
            // ... (similar for other columns if adding them programmatically) ...

            // For columns already added by the designer, set their ReadOnly property:
            if (dataGridViewPerson.Columns.Contains(FirstNameColumnName))
            {
                dataGridViewPerson.Columns[FirstNameColumnName].DataPropertyName = "FirstName";
                dataGridViewPerson.Columns[FirstNameColumnName].ReadOnly = true; // << SET TO TRUE
            }
            if (dataGridViewPerson.Columns.Contains(LastNameColumnName))
            {
                dataGridViewPerson.Columns[LastNameColumnName].DataPropertyName = "LastName";
                dataGridViewPerson.Columns[LastNameColumnName].ReadOnly = true; // << SET TO TRUE
            }
            if (dataGridViewPerson.Columns.Contains(FullNameColumnName))
            {
                dataGridViewPerson.Columns[FullNameColumnName].DataPropertyName = "FullName";
                dataGridViewPerson.Columns[FullNameColumnName].ReadOnly = true; // Already read-only as it's derived
            }

            // Ensure the DataGridView itself is not ReadOnly
            dataGridViewPerson.ReadOnly = false;

            dataGridViewPerson.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewPerson.AllowUserToAddRows = false;
            dataGridViewPerson.RowHeadersVisible = false;
        }
        private void UpdateButtonStatesOnlyOnSelectionChanged(object sender, EventArgs e)
        {
            // This handler now ONLY updates button states if absolutely necessary based on CurrentRow,
            // but primarily, checkbox changes will drive the state.
            UpdateButtonStatesAndTextBoxes();
        }


        private void ToggleControls(bool enable)
        {
            txtFirstName.Enabled = enable;
            txtLastName.Enabled = enable;
            btnAdd.Enabled = enable;
            btnRefresh.Enabled = enable;
            btnBack.Enabled = enable;
            dataGridViewPerson.Enabled = enable;

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

        private void LoadData()
        {
            ToggleControls(false);
            var result = _personService.GetAllPersons();
            if (result.IsSuccess && result.Data != null)
            {
                dataGridViewPerson.DataSource = null;
                dataGridViewPerson.DataSource = result.Data;
            }
            else
            {
                MessageBox.Show(result.Message ?? "An unknown error occurred while loading data.", "Error Loading Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridViewPerson.DataSource = null;
            }
            ClearAllFieldsAndSelections();
            ToggleControls(true);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please enter both First Name and Last Name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return;
            }

            var postDto = new PostPersonDto()
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim()
            };

            ToggleControls(false);
            var result = _personService.AddPerson(postDto);
            ToggleControls(true);

            if (result.IsSuccess)
            {
                MessageBox.Show(result.Message, "Operation Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            else
            {
                MessageBox.Show(result.Message, "Operation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
            // ClearAllFieldsAndSelections(); // LoadData now calls this
            MessageBox.Show("Data has been refreshed.", "Refresh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // _selectedPersonIdForEdit is now set by UpdateButtonStatesAndTextBoxes when one checkbox is checked
            if (_selectedPersonIdForEdit <= 0)
            {
                MessageBox.Show("Please check the box next to the person you want to edit.", "No Person Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("First Name and Last Name cannot be empty for editing.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFirstName.Focus();
                return;
            }

            var updateDto = new UpdatePersonDto
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim()
            };

            ToggleControls(false);
            var result = _personService.UpdatePerson(_selectedPersonIdForEdit, updateDto);
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
                MessageBox.Show("Please check the box(es) for the person(s) you want to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirmResult = MessageBox.Show($"Are you sure you want to delete {selectedIds.Count} selected person(s)?",
                                     "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                ToggleControls(false);
                int successCount = 0;
                int failCount = 0;
                StringBuilder errors = new StringBuilder();

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
                        errors.AppendLine($"- ID {id}: {result.Message}");
                    }
                }
                ToggleControls(true);

                string summaryMessage = $"{successCount} person(s) deleted successfully.";
                if (failCount > 0)
                {
                    summaryMessage += $"\n{failCount} person(s) could not be deleted:\n{errors.ToString()}";
                }
                MessageBox.Show(summaryMessage, "Delete Operation Complete", MessageBoxButtons.OK,
                                failCount > 0 ? MessageBoxIcon.Error : MessageBoxIcon.Information);
                LoadData();
            }
        }

        private List<int> GetSelectedIdsFromCheckboxes()
        {
            var ids = new List<int>();
            if (dataGridViewPerson.Columns.Contains(CheckBoxColumnName) && dataGridViewPerson.Columns.Contains(IdColumnName))
            {
                foreach (DataGridViewRow row in dataGridViewPerson.Rows)
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

        // This event is wired in your Designer.cs
        private void dataGridViewPerson_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // Ignore header clicks

            // If the click was specifically on the checkbox column.
            if (dataGridViewPerson.Columns.Contains(CheckBoxColumnName) && e.ColumnIndex == dataGridViewPerson.Columns[CheckBoxColumnName].Index)
            {
                // The actual logic of toggling the checkbox and then handling the state change
                // (like populating textboxes) will be driven by CellValueChanged,
                // after the checkbox value is committed.
                // We ensure the change is committed so CellValueChanged fires promptly.
                dataGridViewPerson.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
            // Clicking other cells will NOT populate textboxes directly from here anymore.
            // UpdateButtonStatesAndTextBoxes(); // Call this to reflect any potential change in button states immediately
        }

        private void dataGridViewPerson_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridViewPerson.IsCurrentCellDirty &&
                dataGridViewPerson.Columns.Contains(CheckBoxColumnName) &&
                dataGridViewPerson.CurrentCell.OwningColumn.Name == CheckBoxColumnName)
            {
                dataGridViewPerson.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dataGridViewPerson_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // This event fires AFTER the cell value (e.g., checkbox state) has changed and been committed.
            if (e.RowIndex >= 0 && dataGridViewPerson.Columns.Contains(CheckBoxColumnName) && e.ColumnIndex == dataGridViewPerson.Columns[CheckBoxColumnName].Index)
            {
                UpdateButtonStatesAndTextBoxes(); // This will now handle populating textboxes based on checkbox state
            }
        }

        private void dataGridViewPerson_SelectionChanged(object sender, EventArgs e)
        {
            // This event is now mainly for ensuring UI consistency if needed,
            // but not for directly populating textboxes.
            // Textbox population is driven by checkbox state changes via UpdateButtonStatesAndTextBoxes.
            // We can call it here to ensure button states are always fresh,
            // but it won't populate textboxes unless specific checkbox conditions are met.
            UpdateButtonStatesAndTextBoxes();
        }

        private void PopulateFieldsFromRow(DataGridViewRow row)
        {
            if (row == null || row.IsNewRow ||
                !dataGridViewPerson.Columns.Contains(IdColumnName) ||
                !dataGridViewPerson.Columns.Contains(FirstNameColumnName) ||
                !dataGridViewPerson.Columns.Contains(LastNameColumnName) ||
                row.Cells[IdColumnName].Value == null)
            {
                _selectedPersonIdForEdit = 0;
                txtFirstName.Clear();
                txtLastName.Clear();
                return;
            }

            if (int.TryParse(row.Cells[IdColumnName].Value.ToString(), out int id))
            {
                _selectedPersonIdForEdit = id; // Set the ID for potential edit operation
                txtFirstName.Text = row.Cells[FirstNameColumnName].Value?.ToString() ?? "";
                txtLastName.Text = row.Cells[LastNameColumnName].Value?.ToString() ?? "";
            }
            else
            {
                _selectedPersonIdForEdit = 0;
                txtFirstName.Clear();
                txtLastName.Clear();
            }
        }

        private void ClearCheckboxes()
        {
            if (dataGridViewPerson.Columns.Contains(CheckBoxColumnName))
            {
                foreach (DataGridViewRow row in dataGridViewPerson.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    if (checkBoxCell != null) checkBoxCell.Value = false;
                }
            }
        }

        private void ClearAllFieldsAndSelections()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            _selectedPersonIdForEdit = 0;
            ClearCheckboxes();
            if (dataGridViewPerson.Rows.Count > 0)
            {
                dataGridViewPerson.ClearSelection();
                if (dataGridViewPerson.CurrentCell != null)
                    dataGridViewPerson.CurrentCell = null;
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
                // Find the *single* checked row and populate text boxes from it
                foreach (DataGridViewRow row in dataGridViewPerson.Rows)
                {
                    if (row.IsNewRow) continue;
                    DataGridViewCheckBoxCell checkBoxCell = row.Cells[CheckBoxColumnName] as DataGridViewCheckBoxCell;
                    if (checkBoxCell?.Value != null && Convert.ToBoolean(checkBoxCell.Value))
                    {
                        PopulateFieldsFromRow(row); // This sets _selectedPersonIdForEdit and textboxes
                        break;
                    }
                }
            }
            else
            {
                btnEdit.Enabled = false;
                txtFirstName.Clear();
                txtLastName.Clear();
                _selectedPersonIdForEdit = 0; // No single item selected via checkbox for edit
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPerson_Load(object sender, EventArgs e)
        {

        }
    }
}