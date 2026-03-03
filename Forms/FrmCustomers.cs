using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmCustomers : Form
    {
        public FrmCustomers()
        {
            InitializeComponent();
        }

        private void FrmCustomers_Load(object sender, EventArgs e)
        {
            // Guard: Check permission
            if (!AuthGuard.GuardForm(this, "CUSTOMERS"))
                return;

            LoadCustomers();
        }

        private void LoadCustomers()
        {
            try
            {
                string keyword = txtSearch.Text == "Search by name/phone..." ? "" : txtSearch.Text.Trim();

                string sql = @"
                    SELECT CustomerID, CustomerCode AS [Code], FullName AS [Full Name], 
                           Phone AS [Phone], Address AS [Address], 
                           FORMAT(CreatedAt, 'dd/MM/yyyy') AS [Created Date]
                    FROM CUSTOMER
                    WHERE (@kw IS NULL OR @kw = '' OR CustomerCode LIKE '%' + @kw + '%' 
                           OR FullName LIKE '%' + @kw + '%' OR Phone LIKE '%' + @kw + '%')
                    ORDER BY CustomerCode";

                DataTable dt = Db.ExecuteDataTable(sql,
                    new SqlParameter("@kw", string.IsNullOrWhiteSpace(keyword) ? (object)DBNull.Value : keyword));

                gvCustomers.DataSource = dt;

                if (gvCustomers.Columns["CustomerID"] != null)
                    gvCustomers.Columns["CustomerID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gvCustomers_SelectionChanged(object sender, EventArgs e)
        {
            if (gvCustomers.SelectedRows.Count > 0)
            {
                int customerId = Convert.ToInt32(gvCustomers.SelectedRows[0].Cells["CustomerID"].Value);
                LoadPurchaseHistory(customerId);
            }
            else
            {
                gvHistory.DataSource = null;
            }
        }

        private void LoadPurchaseHistory(int customerId)
        {
            try
            {
                string sql = @"
                    SELECT i.InvoiceCode AS [Invoice Code], 
                           FORMAT(i.InvoiceDate, 'dd/MM/yyyy') AS [Date],
                           p.ProductCode AS [Product Code], p.ProductName AS [Product Name], 
                           d.Qty AS [Quantity], d.UnitPrice AS [Unit Price],
                           (d.Qty * d.UnitPrice) AS [Amount]
                    FROM SALES_INVOICE i
                    JOIN SALES_DETAIL d ON d.InvoiceID = i.InvoiceID
                    JOIN PRODUCT p ON p.ProductID = d.ProductID
                    WHERE i.Status = 'COMPLETED' AND i.CustomerID = @customerId
                    ORDER BY i.InvoiceDate DESC";

                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@customerId", customerId));
                gvHistory.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading purchase history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCustomers();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Search by name/phone...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Search by name/phone...";
                txtSearch.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmCustomerEdit frm = new FrmCustomerEdit();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadCustomers();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (gvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to edit.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int customerId = Convert.ToInt32(gvCustomers.SelectedRows[0].Cells["CustomerID"].Value);
            FrmCustomerEdit frm = new FrmCustomerEdit(customerId);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadCustomers();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to delete.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this customer?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    int customerId = Convert.ToInt32(gvCustomers.SelectedRows[0].Cells["CustomerID"].Value);

                    // Check if customer has purchase history
                    string checkSql = "SELECT COUNT(*) FROM SALES_INVOICE WHERE CustomerID = @id";
                    int invoiceCount = Convert.ToInt32(Db.ExecuteScalar(checkSql, null, new SqlParameter("@id", customerId)));

                    if (invoiceCount > 0)
                    {
                        MessageBox.Show(
                            $"Cannot delete customer with {invoiceCount} invoice(s).\n\nThis customer has purchase history and must be kept for data integrity.",
                            "Cannot Delete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    string sql = "DELETE FROM CUSTOMER WHERE CustomerID = @id";
                    Db.ExecuteNonQuery(sql, null, new SqlParameter("@id", customerId));

                    MessageBox.Show("Customer deleted successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "Search by name/phone...";
            txtSearch.ForeColor = System.Drawing.Color.Gray;
            LoadCustomers();
        }
    }
}
