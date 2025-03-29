using Model;
using Model.DomainModels;
using Service;

namespace View
{
    public partial class frmPerson : Form
    {
        private readonly PersonService _personService;
        private int selectedPersonId = -1;
        private int selectedRowIndex = -1;

        public frmPerson(PersonService personService)
        {
            InitializeComponent();
            _personService = personService;
            btnDelete.Enabled = false;
            btnEdit.Enabled = false;
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {

            using (var db = new FinalProjectDbContext())
            {
                Person newPerson = new Person
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text
                };
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
                {
                    MessageBox.Show("Please fill out both text boxes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool isValid = true;


                if (isValid)
                {
                    db.Person.Add(newPerson); // ذخیره در دیتابیس
                    db.SaveChanges(); // ذخیره تغییرات
                    LoadData(); // رفرش لیست بعد از اضافه شدن 
                }
                ClearFields();
                UpdateTextBoxes();
                UpdateButtonStates();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {

            if (selectedRowIndex == -1 || selectedRowIndex >= dataGridViewPerson.Rows.Count) return;

            using (var db = new FinalProjectDbContext())
            {
                DataGridViewRow row = dataGridViewPerson.Rows[selectedRowIndex];
                string firstName = row.Cells[1].Value?.ToString();

                var person = db.Person.FirstOrDefault(p => p.FirstName == firstName);
                if (person != null)
                {
                    person.FirstName = txtFirstName.Text;
                    person.LastName = txtLastName.Text;

                    if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
                    {
                        MessageBox.Show("Please fill out both text boxes", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    db.SaveChanges();
                    LoadData();

                }
                ClearFields();
                UpdateTextBoxes();
                UpdateButtonStates();

            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {

            using (var db = new FinalProjectDbContext())
            {
                List<Person> personsToDelete = new List<Person>();

                foreach (DataGridViewRow row in dataGridViewPerson.Rows)
                {
                    if (Convert.ToBoolean(row.Cells[0].Value))
                    {
                        string firstName = row.Cells[1].Value?.ToString();
                        var person = db.Person.FirstOrDefault(p => p.FirstName == firstName);
                        if (person != null)
                        {
                            personsToDelete.Add(person);
                        }
                    }
                }

                if (personsToDelete.Count > 0)
                {
                    db.Person.RemoveRange(personsToDelete);
                    db.SaveChanges();
                    LoadData();
                }
                ClearFields();
                UpdateTextBoxes();
                UpdateButtonStates();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadData();
        }


        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void dataGridViewPerson_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {


            if (e.RowIndex < 0) return; // جلوگیری از کلیک روی هدر ستون

            DataGridViewRow dgvPerson = dataGridViewPerson.Rows[e.RowIndex];

            txtFirstName.Text = dgvPerson.Cells[1].Value?.ToString();
            txtLastName.Text = dgvPerson.Cells[2].Value?.ToString();


            selectedRowIndex = e.RowIndex;

            // چک کردن اینکه روی چک‌باکس کلیک شده یا نه
            if (e.ColumnIndex == 0)
            {
                DataGridViewCheckBoxCell checkBoxCell = (DataGridViewCheckBoxCell)dgvPerson.Cells[0];
                checkBoxCell.Value = !(checkBoxCell.Value != null && (bool)checkBoxCell.Value);
                dataGridViewPerson.EndEdit();
                UpdateButtonStates();
                UpdateTextBoxes();
            }
        }


        private void frmPerson_Load(object sender, EventArgs e)
        {

        }

        private void LoadData()
        {
            using (var db = new FinalProjectDbContext())
            {
                var persons = db.Person.ToList(); // دریافت کاربر از دیتابیس

                dataGridViewPerson.Rows.Clear(); // پاک کردن داده‌های قبلی
                foreach (var person in persons)
                {
                    dataGridViewPerson.Rows.Add(false, person.FirstName, person.LastName, person.FullName);
                }
            }
        }

        private void UpdateButtonStates()
        {
            int checkedCount = 0;

            // شمارش تعداد چک باکس‌های تیک خورده
            foreach (DataGridViewRow row in dataGridViewPerson.Rows)
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

        private void UpdateTextBoxes()
        {
            int selectedCount = 0;
            DataGridViewRow selectedRow = null;

            foreach (DataGridViewRow row in dataGridViewPerson.Rows)
            {
                DataGridViewCheckBoxCell checkBox = row.Cells[0] as DataGridViewCheckBoxCell;
                if (checkBox != null && checkBox.Value != null && (bool)checkBox.Value)
                {
                    selectedCount++;
                    if (selectedCount == 1)
                    {
                        selectedRow = row; // ذخیره اولین ردیف انتخاب‌شده
                    }
                }
            }

            if (selectedCount == 1)
            {
                txtFirstName.Text = selectedRow.Cells[1].Value?.ToString() ?? "";
                txtLastName.Text = selectedRow.Cells[2].Value?.ToString() ?? "";

            }
            else
            {
                txtFirstName.Clear();
                txtLastName.Clear();

            }
        }

        private void ClearFields()
        {
            txtFirstName.Clear();
            txtLastName.Clear();
            selectedPersonId = -1;
        }


    }
}
