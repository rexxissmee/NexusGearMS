namespace NexusGearMS.Forms
{
    partial class FrmInvoiceList
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlSearch      = new System.Windows.Forms.Panel();
            this.lblKeyword     = new System.Windows.Forms.Label();
            this.txtSearch      = new System.Windows.Forms.TextBox();
            this.lblFrom        = new System.Windows.Forms.Label();
            this.dtFrom         = new System.Windows.Forms.DateTimePicker();
            this.lblTo          = new System.Windows.Forms.Label();
            this.dtTo           = new System.Windows.Forms.DateTimePicker();
            this.btnSearch      = new System.Windows.Forms.Button();
            this.btnRefresh     = new System.Windows.Forms.Button();
            this.lblInvoicesHdr = new System.Windows.Forms.Label();
            this.gvInvoices     = new System.Windows.Forms.DataGridView();
            this.lblDetailsHdr  = new System.Windows.Forms.Label();
            this.gvDetails      = new System.Windows.Forms.DataGridView();
            this.pnlBottom      = new System.Windows.Forms.Panel();
            this.btnNew         = new System.Windows.Forms.Button();
            this.btnDelete      = new System.Windows.Forms.Button();
            this.lblTotal       = new System.Windows.Forms.Label();
            this.pnlSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvInvoices)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetails)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSearch
            //
            this.pnlSearch.BackColor = System.Drawing.Color.FromArgb(236, 239, 241);
            this.pnlSearch.Controls.Add(this.lblKeyword);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.lblFrom);
            this.pnlSearch.Controls.Add(this.dtFrom);
            this.pnlSearch.Controls.Add(this.lblTo);
            this.pnlSearch.Controls.Add(this.dtTo);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.btnRefresh);
            this.pnlSearch.Location = new System.Drawing.Point(0, 0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(800, 55);
            this.pnlSearch.TabIndex = 1;
            // 
            // lblKeyword
            // 
            this.lblKeyword.AutoSize = true;
            this.lblKeyword.Location = new System.Drawing.Point(10, 18);
            this.lblKeyword.Name = "lblKeyword";
            this.lblKeyword.Text = "Search:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(65, 15);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(175, 20);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(250, 18);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Text = "From:";
            // 
            // dtFrom
            // 
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(295, 15);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(110, 20);
            this.dtFrom.TabIndex = 1;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(415, 18);
            this.lblTo.Name = "lblTo";
            this.lblTo.Text = "To:";
            // 
            // dtTo
            // 
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(440, 15);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(110, 20);
            this.dtTo.TabIndex = 2;
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Location = new System.Drawing.Point(562, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 28);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = false;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(652, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 28);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblInvoicesHdr
            // 
            this.lblInvoicesHdr.AutoSize = true;
            this.lblInvoicesHdr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblInvoicesHdr.Location = new System.Drawing.Point(10, 62);
            this.lblInvoicesHdr.Name = "lblInvoicesHdr";
            this.lblInvoicesHdr.Text = "Invoice List";
            // 
            // gvInvoices
            // 
            this.gvInvoices.AllowUserToAddRows = false;
            this.gvInvoices.AllowUserToDeleteRows = false;
            this.gvInvoices.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvInvoices.BackgroundColor = System.Drawing.Color.White;
            this.gvInvoices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvInvoices.Location = new System.Drawing.Point(10, 80);
            this.gvInvoices.MultiSelect = false;
            this.gvInvoices.Name = "gvInvoices";
            this.gvInvoices.ReadOnly = true;
            this.gvInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvInvoices.Size = new System.Drawing.Size(780, 190);
            this.gvInvoices.TabIndex = 5;
            this.gvInvoices.SelectionChanged += new System.EventHandler(this.gvInvoices_SelectionChanged);
            // 
            // lblDetailsHdr
            // 
            this.lblDetailsHdr.AutoSize = true;
            this.lblDetailsHdr.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.lblDetailsHdr.Location = new System.Drawing.Point(10, 278);
            this.lblDetailsHdr.Name = "lblDetailsHdr";
            this.lblDetailsHdr.Text = "Invoice Lines";
            // 
            // gvDetails
            // 
            this.gvDetails.AllowUserToAddRows = false;
            this.gvDetails.AllowUserToDeleteRows = false;
            this.gvDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvDetails.BackgroundColor = System.Drawing.Color.White;
            this.gvDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvDetails.Location = new System.Drawing.Point(10, 296);
            this.gvDetails.Name = "gvDetails";
            this.gvDetails.ReadOnly = true;
            this.gvDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvDetails.Size = new System.Drawing.Size(780, 130);
            this.gvDetails.TabIndex = 6;
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.pnlBottom.Controls.Add(this.btnNew);
            this.pnlBottom.Controls.Add(this.btnDelete);
            this.pnlBottom.Controls.Add(this.lblTotal);
            this.pnlBottom.Location = new System.Drawing.Point(0, 437);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(800, 73);
            this.pnlBottom.TabIndex = 7;
            // 
            // btnNew
            // 
            this.btnNew.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
            this.btnNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.White;
            this.btnNew.Location = new System.Drawing.Point(10, 18);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(150, 36);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "New Invoice";
            this.btnNew.UseVisualStyleBackColor = false;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnDelete.Enabled = false;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Location = new System.Drawing.Point(170, 18);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 36);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.lblTotal.Location = new System.Drawing.Point(360, 20);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(425, 30);
            this.lblTotal.TabIndex = 2;
            this.lblTotal.Text = "Total: $0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmInvoiceList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 510);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.gvDetails);
            this.Controls.Add(this.lblDetailsHdr);
            this.Controls.Add(this.gvInvoices);
            this.Controls.Add(this.lblInvoicesHdr);
            this.Controls.Add(this.pnlSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FrmInvoiceList";
            this.Text = "Invoice Management";
            this.Load += new System.EventHandler(this.FrmInvoiceList_Load);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvInvoices)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDetails)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Label lblKeyword;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblInvoicesHdr;
        private System.Windows.Forms.DataGridView gvInvoices;
        private System.Windows.Forms.Label lblDetailsHdr;
        private System.Windows.Forms.DataGridView gvDetails;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblTotal;
    }
}
