namespace View
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnPerson = new Button();
            btnProduct = new Button();
            SuspendLayout();
            // 
            // btnPerson
            // 
            btnPerson.Location = new Point(82, 212);
            btnPerson.Name = "btnPerson";
            btnPerson.Size = new Size(550, 232);
            btnPerson.TabIndex = 0;
            btnPerson.TabStop = false;
            btnPerson.Text = "Person";
            btnPerson.UseVisualStyleBackColor = true;
            btnPerson.Click += btnOpenPersons_Click;
            // 
            // btnProduct
            // 
            btnProduct.Location = new Point(746, 212);
            btnProduct.Name = "btnProduct";
            btnProduct.Size = new Size(550, 232);
            btnProduct.TabIndex = 1;
            btnProduct.TabStop = false;
            btnProduct.Text = "Product";
            btnProduct.UseVisualStyleBackColor = true;
            btnProduct.Click += btnOpenProducts_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1497, 818);
            Controls.Add(btnProduct);
            Controls.Add(btnPerson);
            Name = "frmMain";
            Text = "MainForm";
            Load += frmMain_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnPerson;
        private Button btnProduct;
    }
}
