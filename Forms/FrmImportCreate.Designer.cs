namespace NexusGearMS.Forms
{
    partial class FrmImportCreate
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblSupplier = new System.Windows.Forms.Label();
            this.cboSupplier = new System.Windows.Forms.ComboBox();
            this.lblImportCode = new System.Windows.Forms.Label();
            this.txtImportCode = new System.Windows.Forms.TextBox();
            this.lblImportDate = new System.Windows.Forms.Label();
            this.dtImportDate = new System.Windows.Forms.DateTimePicker();
            this.lblNote = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cboProduct = new System.Windows.Forms.ComboBox();
            this.lblQty = new System.Windows.Forms.Label();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.lblUnitCost = new System.Windows.Forms.Label();
            this.txtUnitCost = new System.Windows.Forms.TextBox();
            this.btnAddLine = new System.Windows.Forms.Button();
            this.btnRemoveLine = new System.Windows.Forms.Button();
            this.gvLines = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvLines)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSupplier
            //
            this.lblSupplier.AutoSize = true;
            this.lblSupplier.Location = new System.Drawing.Point(20, 20);
            this.lblSupplier.Name = "lblSupplier";
            this.lblSupplier.Size = new System.Drawing.Size(78, 13);
            this.lblSupplier.TabIndex = 1;
            this.lblSupplier.Text = "Supplier:";
            // 
            // cboSupplier
            // 
            this.cboSupplier.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSupplier.FormattingEnabled = true;
            this.cboSupplier.Location = new System.Drawing.Point(110, 17);
            this.cboSupplier.Name = "cboSupplier";
            this.cboSupplier.Size = new System.Drawing.Size(250, 21);
            this.cboSupplier.TabIndex = 2;
            // 
            // lblImportCode
            // 
            this.lblImportCode.AutoSize = true;
            this.lblImportCode.Location = new System.Drawing.Point(390, 20);
            this.lblImportCode.Name = "lblImportCode";
            this.lblImportCode.Size = new System.Drawing.Size(43, 13);
            this.lblImportCode.TabIndex = 3;
            this.lblImportCode.Text = "Receipt ID:";
            // 
            // txtImportCode
            // 
            this.txtImportCode.Location = new System.Drawing.Point(445, 17);
            this.txtImportCode.Name = "txtImportCode";
            this.txtImportCode.Size = new System.Drawing.Size(130, 20);
            this.txtImportCode.TabIndex = 4;
            // 
            // lblImportDate
            // 
            this.lblImportDate.AutoSize = true;
            this.lblImportDate.Location = new System.Drawing.Point(600, 20);
            this.lblImportDate.Name = "lblImportDate";
            this.lblImportDate.Size = new System.Drawing.Size(35, 13);
            this.lblImportDate.TabIndex = 5;
            this.lblImportDate.Text = "Date:";
            // 
            // dtImportDate
            // 
            this.dtImportDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtImportDate.Location = new System.Drawing.Point(645, 17);
            this.dtImportDate.Name = "dtImportDate";
            this.dtImportDate.Size = new System.Drawing.Size(130, 20);
            this.dtImportDate.TabIndex = 6;
            // 
            // lblNote
            // 
            this.lblNote.AutoSize = true;
            this.lblNote.Location = new System.Drawing.Point(20, 55);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(47, 13);
            this.lblNote.TabIndex = 7;
            this.lblNote.Text = "Note:";
            // 
            // txtNote
            // 
            this.txtNote.Location = new System.Drawing.Point(110, 52);
            this.txtNote.Name = "txtNote";
            this.txtNote.Size = new System.Drawing.Size(665, 20);
            this.txtNote.TabIndex = 8;
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(20, 95);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(58, 13);
            this.lblProduct.TabIndex = 9;
            this.lblProduct.Text = "Product:";
            // 
            // cboProduct
            // 
            this.cboProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProduct.FormattingEnabled = true;
            this.cboProduct.Location = new System.Drawing.Point(110, 92);
            this.cboProduct.Name = "cboProduct";
            this.cboProduct.Size = new System.Drawing.Size(200, 21);
            this.cboProduct.TabIndex = 10;
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Location = new System.Drawing.Point(325, 95);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(52, 13);
            this.lblQty.TabIndex = 11;
            this.lblQty.Text = "Quantity:";
            // 
            // txtQty
            // 
            this.txtQty.Location = new System.Drawing.Point(385, 92);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(70, 20);
            this.txtQty.TabIndex = 12;
            this.txtQty.Text = "1";
            // 
            // lblUnitCost
            // 
            this.lblUnitCost.AutoSize = true;
            this.lblUnitCost.Location = new System.Drawing.Point(470, 95);
            this.lblUnitCost.Name = "lblUnitCost";
            this.lblUnitCost.Size = new System.Drawing.Size(56, 13);
            this.lblUnitCost.TabIndex = 13;
            this.lblUnitCost.Text = "Unit Cost:";
            // 
            // txtUnitCost
            // 
            this.txtUnitCost.Location = new System.Drawing.Point(535, 92);
            this.txtUnitCost.Name = "txtUnitCost";
            this.txtUnitCost.Size = new System.Drawing.Size(100, 20);
            this.txtUnitCost.TabIndex = 14;
            this.txtUnitCost.Text = "0";
            // 
            // btnAddLine
            // 
            this.btnAddLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAddLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLine.ForeColor = System.Drawing.Color.White;
            this.btnAddLine.Location = new System.Drawing.Point(645, 87);
            this.btnAddLine.Name = "btnAddLine";
            this.btnAddLine.Size = new System.Drawing.Size(65, 30);
            this.btnAddLine.TabIndex = 15;
            this.btnAddLine.Text = "Add";
            this.btnAddLine.UseVisualStyleBackColor = false;
            this.btnAddLine.Click += new System.EventHandler(this.btnAddLine_Click);
            // 
            // btnRemoveLine
            // 
            this.btnRemoveLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnRemoveLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveLine.ForeColor = System.Drawing.Color.White;
            this.btnRemoveLine.Location = new System.Drawing.Point(715, 87);
            this.btnRemoveLine.Name = "btnRemoveLine";
            this.btnRemoveLine.Size = new System.Drawing.Size(60, 30);
            this.btnRemoveLine.TabIndex = 16;
            this.btnRemoveLine.Text = "Remove";
            this.btnRemoveLine.UseVisualStyleBackColor = false;
            this.btnRemoveLine.Click += new System.EventHandler(this.btnRemoveLine_Click);
            // 
            // gvLines
            // 
            this.gvLines.AllowUserToAddRows = false;
            this.gvLines.AllowUserToDeleteRows = false;
            this.gvLines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvLines.BackgroundColor = System.Drawing.Color.White;
            this.gvLines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvLines.Location = new System.Drawing.Point(20, 135);
            this.gvLines.Name = "gvLines";
            this.gvLines.ReadOnly = true;
            this.gvLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvLines.Size = new System.Drawing.Size(760, 280);
            this.gvLines.TabIndex = 17;
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(400, 430);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(380, 30);
            this.lblTotal.TabIndex = 18;
            this.lblTotal.Text = "Total: $0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(300, 465);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 40);
            this.btnSave.TabIndex = 19;
            this.btnSave.Text = "Save Receipt";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmImportCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 520);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.gvLines);
            this.Controls.Add(this.btnRemoveLine);
            this.Controls.Add(this.btnAddLine);
            this.Controls.Add(this.txtUnitCost);
            this.Controls.Add(this.lblUnitCost);
            this.Controls.Add(this.txtQty);
            this.Controls.Add(this.lblQty);
            this.Controls.Add(this.cboProduct);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(this.dtImportDate);
            this.Controls.Add(this.lblImportDate);
            this.Controls.Add(this.txtImportCode);
            this.Controls.Add(this.lblImportCode);
            this.Controls.Add(this.cboSupplier);
            this.Controls.Add(this.lblSupplier);
            this.Name = "FrmImportCreate";
            this.Text = "Create Import Receipt";
            this.Load += new System.EventHandler(this.FrmImportCreate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvLines)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblSupplier;
        private System.Windows.Forms.ComboBox cboSupplier;
        private System.Windows.Forms.Label lblImportCode;
        private System.Windows.Forms.TextBox txtImportCode;
        private System.Windows.Forms.Label lblImportDate;
        private System.Windows.Forms.DateTimePicker dtImportDate;
        private System.Windows.Forms.Label lblNote;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.ComboBox cboProduct;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Label lblUnitCost;
        private System.Windows.Forms.TextBox txtUnitCost;
        private System.Windows.Forms.Button btnAddLine;
        private System.Windows.Forms.Button btnRemoveLine;
        private System.Windows.Forms.DataGridView gvLines;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnSave;
    }
}
