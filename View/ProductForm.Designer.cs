namespace View
{
    partial class frmProduct
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
            btnBack = new Button();
            btnRefresh = new Button();
            btnDelete = new Button();
            btnEdit = new Button();
            btnAdd = new Button();
            dataGridViewProduct = new DataGridView();
            CheckBox = new DataGridViewCheckBoxColumn();
            colId = new DataGridViewTextBoxColumn();
            productTitle = new DataGridViewTextBoxColumn();
            productUnitPrice = new DataGridViewTextBoxColumn();
            productQuantity = new DataGridViewTextBoxColumn();
            lblTitle = new Label();
            lblUnitPrice = new Label();
            lblQuantity = new Label();
            txtTitle = new TextBox();
            txtUnitPrice = new TextBox();
            txtQuantity = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridViewProduct).BeginInit();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Location = new Point(1156, 23);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(245, 66);
            btnBack.TabIndex = 9;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(847, 23);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(245, 66);
            btnRefresh.TabIndex = 8;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnDelete
            // 
            btnDelete.Location = new Point(575, 23);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(245, 66);
            btnDelete.TabIndex = 7;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(297, 23);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(245, 66);
            btnEdit.TabIndex = 6;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(19, 23);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(245, 66);
            btnAdd.TabIndex = 5;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // dataGridViewProduct
            // 
            dataGridViewProduct.AllowDrop = true;
            dataGridViewProduct.AllowUserToAddRows = false;
            dataGridViewProduct.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewProduct.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewProduct.Columns.AddRange(new DataGridViewColumn[] { CheckBox, colId, productTitle, productUnitPrice, productQuantity });
            dataGridViewProduct.Location = new Point(19, 325);
            dataGridViewProduct.Name = "dataGridViewProduct";
            dataGridViewProduct.RowHeadersVisible = false;
            dataGridViewProduct.RowHeadersWidth = 82;
            dataGridViewProduct.Size = new Size(1382, 288);
            dataGridViewProduct.TabIndex = 10;
            dataGridViewProduct.CellContentClick += dataGridViewProduct_CellContentClick;
            // 
            // CheckBox
            // 
            CheckBox.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            CheckBox.HeaderText = "";
            CheckBox.MinimumWidth = 10;
            CheckBox.Name = "CheckBox";
            CheckBox.Width = 50;
            // 
            // colId
            // 
            colId.DataPropertyName = "Id";
            colId.HeaderText = "ID";
            colId.MinimumWidth = 10;
            colId.Name = "colId";
            colId.Visible = false;
            // 
            // productTitle
            // 
            productTitle.HeaderText = "Title";
            productTitle.MinimumWidth = 10;
            productTitle.Name = "productTitle";
            // 
            // productUnitPrice
            // 
            productUnitPrice.HeaderText = "UnitPrice";
            productUnitPrice.MinimumWidth = 10;
            productUnitPrice.Name = "productUnitPrice";
            // 
            // productQuantity
            // 
            productQuantity.HeaderText = "Quantity";
            productQuantity.MinimumWidth = 10;
            productQuantity.Name = "productQuantity";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(36, 134);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(65, 32);
            lblTitle.TabIndex = 12;
            lblTitle.Text = "TItle:";
            // 
            // lblUnitPrice
            // 
            lblUnitPrice.AutoSize = true;
            lblUnitPrice.Location = new Point(689, 131);
            lblUnitPrice.Name = "lblUnitPrice";
            lblUnitPrice.Size = new Size(121, 32);
            lblUnitPrice.TabIndex = 13;
            lblUnitPrice.Text = "Unit Price:";
            // 
            // lblQuantity
            // 
            lblQuantity.AutoSize = true;
            lblQuantity.Location = new Point(689, 183);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Size = new Size(111, 32);
            lblQuantity.TabIndex = 14;
            lblQuantity.Text = "Quantity:";
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(176, 128);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(200, 39);
            txtTitle.TabIndex = 1;
            // 
            // txtUnitPrice
            // 
            txtUnitPrice.Location = new Point(847, 124);
            txtUnitPrice.Name = "txtUnitPrice";
            txtUnitPrice.Size = new Size(200, 39);
            txtUnitPrice.TabIndex = 2;
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new Point(847, 183);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new Size(200, 39);
            txtQuantity.TabIndex = 3;
            // 
            // frmProduct
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1463, 645);
            Controls.Add(txtQuantity);
            Controls.Add(txtUnitPrice);
            Controls.Add(txtTitle);
            Controls.Add(lblQuantity);
            Controls.Add(lblUnitPrice);
            Controls.Add(lblTitle);
            Controls.Add(dataGridViewProduct);
            Controls.Add(btnBack);
            Controls.Add(btnRefresh);
            Controls.Add(btnDelete);
            Controls.Add(btnEdit);
            Controls.Add(btnAdd);
            Name = "frmProduct";
            Text = "ProductForm";
            Load += frmProduct_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewProduct).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnBack;
        private Button btnRefresh;
        private Button btnDelete;
        private Button btnEdit;
        private Button btnAdd;
        private DataGridView dataGridViewProduct;
        private Label lblTitle;
        private Label lblUnitPrice;
        private Label lblQuantity;
        private TextBox txtTitle;
        private TextBox txtUnitPrice;
        private TextBox txtQuantity;
        private DataGridViewCheckBoxColumn CheckBox;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn productTitle;
        private DataGridViewTextBoxColumn productUnitPrice;
        private DataGridViewTextBoxColumn productQuantity;
    }
}