namespace NexusGearMS.Forms
{
    partial class FrmInvoiceCreate
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
            this.lblCustomer = new System.Windows.Forms.Label();
            this.cboCustomer = new System.Windows.Forms.ComboBox();
            this.lblInvoiceCode = new System.Windows.Forms.Label();
            this.txtInvoiceCode = new System.Windows.Forms.TextBox();
            this.lblInvoiceDate = new System.Windows.Forms.Label();
            this.dtInvoiceDate = new System.Windows.Forms.DateTimePicker();
            this.lblProduct = new System.Windows.Forms.Label();
            this.cboProduct = new System.Windows.Forms.ComboBox();
            this.lblQty = new System.Windows.Forms.Label();
            this.txtQty = new System.Windows.Forms.TextBox();
            this.btnAddLine = new System.Windows.Forms.Button();
            this.btnRemoveLine = new System.Windows.Forms.Button();
            this.gvLines = new System.Windows.Forms.DataGridView();
            this.lblTotal = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvLines)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCustomer
            //
            this.lblCustomer.AutoSize = true;
            this.lblCustomer.Location = new System.Drawing.Point(20, 20);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(68, 13);
            this.lblCustomer.TabIndex = 1;
            this.lblCustomer.Text = "Customer:";
            // 
            // cboCustomer
            // 
            this.cboCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCustomer.FormattingEnabled = true;
            this.cboCustomer.Location = new System.Drawing.Point(100, 17);
            this.cboCustomer.Name = "cboCustomer";
            this.cboCustomer.Size = new System.Drawing.Size(250, 21);
            this.cboCustomer.TabIndex = 2;
            // 
            // lblInvoiceCode
            // 
            this.lblInvoiceCode.AutoSize = true;
            this.lblInvoiceCode.Location = new System.Drawing.Point(380, 20);
            this.lblInvoiceCode.Name = "lblInvoiceCode";
            this.lblInvoiceCode.Size = new System.Drawing.Size(44, 13);
            this.lblInvoiceCode.TabIndex = 3;
            this.lblInvoiceCode.Text = "Invoice ID:";
            // 
            // txtInvoiceCode
            // 
            this.txtInvoiceCode.Location = new System.Drawing.Point(430, 17);
            this.txtInvoiceCode.Name = "txtInvoiceCode";
            this.txtInvoiceCode.Size = new System.Drawing.Size(150, 20);
            this.txtInvoiceCode.TabIndex = 4;
            // 
            // lblInvoiceDate
            // 
            this.lblInvoiceDate.AutoSize = true;
            this.lblInvoiceDate.Location = new System.Drawing.Point(600, 20);
            this.lblInvoiceDate.Name = "lblInvoiceDate";
            this.lblInvoiceDate.Size = new System.Drawing.Size(35, 13);
            this.lblInvoiceDate.TabIndex = 5;
            this.lblInvoiceDate.Text = "Date:";
            // 
            // dtInvoiceDate
            // 
            this.dtInvoiceDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtInvoiceDate.Location = new System.Drawing.Point(645, 17);
            this.dtInvoiceDate.Name = "dtInvoiceDate";
            this.dtInvoiceDate.Size = new System.Drawing.Size(130, 20);
            this.dtInvoiceDate.TabIndex = 6;
            // 
            // lblProduct
            // 
            this.lblProduct.AutoSize = true;
            this.lblProduct.Location = new System.Drawing.Point(20, 60);
            this.lblProduct.Name = "lblProduct";
            this.lblProduct.Size = new System.Drawing.Size(58, 13);
            this.lblProduct.TabIndex = 7;
            this.lblProduct.Text = "Product:";
            // 
            // cboProduct
            // 
            this.cboProduct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProduct.FormattingEnabled = true;
            this.cboProduct.Location = new System.Drawing.Point(100, 57);
            this.cboProduct.Name = "cboProduct";
            this.cboProduct.Size = new System.Drawing.Size(250, 21);
            this.cboProduct.TabIndex = 8;
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Location = new System.Drawing.Point(370, 60);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(52, 13);
            this.lblQty.TabIndex = 9;
            this.lblQty.Text = "Quantity:";
            // 
            // txtQty
            // 
            this.txtQty.Location = new System.Drawing.Point(430, 57);
            this.txtQty.Name = "txtQty";
            this.txtQty.Size = new System.Drawing.Size(80, 20);
            this.txtQty.TabIndex = 10;
            this.txtQty.Text = "1";
            // 
            // btnAddLine
            // 
            this.btnAddLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnAddLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLine.ForeColor = System.Drawing.Color.White;
            this.btnAddLine.Location = new System.Drawing.Point(530, 52);
            this.btnAddLine.Name = "btnAddLine";
            this.btnAddLine.Size = new System.Drawing.Size(100, 30);
            this.btnAddLine.TabIndex = 11;
            this.btnAddLine.Text = "Add Line";
            this.btnAddLine.UseVisualStyleBackColor = false;
            this.btnAddLine.Click += new System.EventHandler(this.btnAddLine_Click);
            // 
            // btnRemoveLine
            // 
            this.btnRemoveLine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.btnRemoveLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveLine.ForeColor = System.Drawing.Color.White;
            this.btnRemoveLine.Location = new System.Drawing.Point(645, 52);
            this.btnRemoveLine.Name = "btnRemoveLine";
            this.btnRemoveLine.Size = new System.Drawing.Size(100, 30);
            this.btnRemoveLine.TabIndex = 12;
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
            this.gvLines.Location = new System.Drawing.Point(20, 100);
            this.gvLines.Name = "gvLines";
            this.gvLines.ReadOnly = true;
            this.gvLines.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvLines.Size = new System.Drawing.Size(760, 300);
            this.gvLines.TabIndex = 13;
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(400, 415);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(380, 30);
            this.lblTotal.TabIndex = 14;
            this.lblTotal.Text = "Total: $0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(300, 450);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 40);
            this.btnSave.TabIndex = 15;
            this.btnSave.Text = "Save Invoice";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmInvoiceCreate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 510);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.gvLines);
            this.Controls.Add(this.btnRemoveLine);
            this.Controls.Add(this.btnAddLine);
            this.Controls.Add(this.txtQty);
            this.Controls.Add(this.lblQty);
            this.Controls.Add(this.cboProduct);
            this.Controls.Add(this.lblProduct);
            this.Controls.Add(this.dtInvoiceDate);
            this.Controls.Add(this.lblInvoiceDate);
            this.Controls.Add(this.txtInvoiceCode);
            this.Controls.Add(this.lblInvoiceCode);
            this.Controls.Add(this.cboCustomer);
            this.Controls.Add(this.lblCustomer);
            this.Name = "FrmInvoiceCreate";
            this.Text = "Create Invoice";
            this.Load += new System.EventHandler(this.FrmInvoiceCreate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvLines)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblCustomer;
        private System.Windows.Forms.ComboBox cboCustomer;
        private System.Windows.Forms.Label lblInvoiceCode;
        private System.Windows.Forms.TextBox txtInvoiceCode;
        private System.Windows.Forms.Label lblInvoiceDate;
        private System.Windows.Forms.DateTimePicker dtInvoiceDate;
        private System.Windows.Forms.Label lblProduct;
        private System.Windows.Forms.ComboBox cboProduct;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.TextBox txtQty;
        private System.Windows.Forms.Button btnAddLine;
        private System.Windows.Forms.Button btnRemoveLine;
        private System.Windows.Forms.DataGridView gvLines;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Button btnSave;
    }
}
