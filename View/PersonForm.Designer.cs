namespace View
{
    partial class frmPerson
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnAdd = new Button();
            btnEdit = new Button();
            btnDelete = new Button();
            btnRefresh = new Button();
            btnBack = new Button();
            dataGridViewPerson = new DataGridView();
            CheckBox = new DataGridViewCheckBoxColumn();
            personFirstName = new DataGridViewTextBoxColumn();
            personLastName = new DataGridViewTextBoxColumn();
            personFullName = new DataGridViewTextBoxColumn();
            lblFirstName = new Label();
            lblLastName = new Label();
            txtFirstName = new TextBox();
            txtLastName = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewPerson).BeginInit();
            SuspendLayout();
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(25, 29);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(245, 66);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(303, 29);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(245, 66);
            btnEdit.TabIndex = 2;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(581, 29);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(245, 66);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(853, 29);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(245, 66);
            btnRefresh.TabIndex = 4;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(1162, 29);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(245, 66);
            btnBack.TabIndex = 5;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // dataGridViewPerson
            // 
            dataGridViewPerson.AllowDrop = true;
            dataGridViewPerson.AllowUserToAddRows = false;
            dataGridViewPerson.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewPerson.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewPerson.Columns.AddRange(new DataGridViewColumn[] { CheckBox, personFirstName, personLastName, personFullName });
            dataGridViewPerson.Location = new Point(25, 319);
            dataGridViewPerson.Name = "dataGridViewPerson";
            dataGridViewPerson.RowHeadersVisible = false;
            dataGridViewPerson.RowHeadersWidth = 82;
            dataGridViewPerson.Size = new Size(1382, 300);
            dataGridViewPerson.TabIndex = 6;
            dataGridViewPerson.CellContentClick += dataGridViewPerson_CellContentClick;
            // 
            // CheckBox
            // 
            CheckBox.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            CheckBox.FillWeight = 73.71795F;
            CheckBox.HeaderText = "";
            CheckBox.MinimumWidth = 10;
            CheckBox.Name = "CheckBox";
            CheckBox.Width = 50;
            // 
            // personFirstName
            // 
            personFirstName.FillWeight = 106.570511F;
            personFirstName.HeaderText = "First Name";
            personFirstName.MinimumWidth = 10;
            personFirstName.Name = "personFirstName";
            // 
            // personLastName
            // 
            personLastName.FillWeight = 106.570511F;
            personLastName.HeaderText = "Last Name";
            personLastName.MinimumWidth = 10;
            personLastName.Name = "personLastName";
            // 
            // personFullName
            // 
            personFullName.FillWeight = 106.570511F;
            personFullName.HeaderText = "Full Name";
            personFullName.MinimumWidth = 10;
            personFullName.Name = "personFullName";
            // 
            // lblFirstName
            // 
            lblFirstName.AutoSize = true;
            lblFirstName.Location = new Point(25, 148);
            lblFirstName.Name = "lblFirstName";
            lblFirstName.Size = new Size(134, 32);
            lblFirstName.TabIndex = 7;
            lblFirstName.Text = "First Name:";
            // 
            // lblLastName
            // 
            lblLastName.AutoSize = true;
            lblLastName.Location = new Point(695, 148);
            lblLastName.Name = "lblLastName";
            lblLastName.Size = new Size(131, 32);
            lblLastName.TabIndex = 8;
            lblLastName.Text = "Last Name:";
            // 
            // txtFirstName
            // 
            txtFirstName.Location = new Point(178, 141);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.Size = new Size(200, 39);
            txtFirstName.TabIndex = 0;
            // 
            // txtLastName
            // 
            txtLastName.Location = new Point(853, 141);
            txtLastName.Name = "txtLastName";
            txtLastName.Size = new Size(200, 39);
            txtLastName.TabIndex = 1;
            // 
            // frmPerson
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1537, 663);
            Controls.Add(txtLastName);
            Controls.Add(txtFirstName);
            Controls.Add(lblLastName);
            Controls.Add(lblFirstName);
            Controls.Add(dataGridViewPerson);
            Controls.Add(btnBack);
            Controls.Add(btnRefresh);
            Controls.Add(btnDelete);
            Controls.Add(btnEdit);
            Controls.Add(btnAdd);
            Name = "frmPerson";
            Text = "PersonForm";
            Load += frmPerson_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewPerson).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnBack;
        private DataGridView dataGridViewPerson;
        private Label lblFirstName;
        private Label lblLastName;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private DataGridViewCheckBoxColumn CheckBox;
        private DataGridViewTextBoxColumn personFirstName;
        private DataGridViewTextBoxColumn personLastName;
        private DataGridViewTextBoxColumn personFullName;
    }
}