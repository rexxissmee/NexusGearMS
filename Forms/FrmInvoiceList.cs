using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmInvoiceList : Form
    {
        private int? selectedInvoiceID = null;

        public FrmInvoiceList()
        {
            InitializeComponent();
        }

        private void FrmInvoiceList_Load(object sender, EventArgs e)
        {
            if (!AuthGuard.GuardForm(this, "INVOICES"))
                return;

            dtFrom.Value = DateTime.Now.AddMonths(-1);
            dtTo.Value = DateTime.Now;
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                string sql = @"
                    SELECT
                        si.InvoiceID,
                        si.InvoiceCode        AS [Invoice Code],
                        CONVERT(varchar, si.InvoiceDate, 103) AS [Date],
                        ISNULL(c.FullName, 'Walk-in') AS [Customer],
                        e.FullName            AS [Employee],
                        si.Status,
                        ISNULL(SUM(sd.Qty * sd.UnitPrice), 0) AS [Total Amount]
                    FROM SALES_INVOICE si
                    LEFT JOIN CUSTOMER c    ON c.CustomerID = si.CustomerID
                    LEFT JOIN EMPLOYEE e    ON e.EmpID      = si.EmpID
                    LEFT JOIN SALES_DETAIL sd ON sd.InvoiceID = si.InvoiceID
                    WHERE si.InvoiceDate BETWEEN @from AND @to
                      AND (@keyword = '' OR si.InvoiceCode LIKE @keyword OR ISNULL(c.FullName,'') LIKE @keyword)
                    GROUP BY si.InvoiceID, si.InvoiceCode, si.InvoiceDate,
                             c.FullName, e.FullName, si.Status
                    ORDER BY si.InvoiceDate DESC, si.InvoiceID DESC";

                DataTable dt = Db.ExecuteDataTable(sql,
                    new SqlParameter("@from",    dtFrom.Value.Date),
                    new SqlParameter("@to",      dtTo.Value.Date.AddDays(1).AddSeconds(-1)),
                    new SqlParameter("@keyword", string.IsNullOrEmpty(keyword) ? "" : "%" + keyword + "%"));

                gvInvoices.DataSource = dt;

                if (gvInvoices.Columns["InvoiceID"] != null)
                    gvInvoices.Columns["InvoiceID"].Visible = false;

                ClearDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading invoices: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadInvoiceDetails()
        {
            if (!selectedInvoiceID.HasValue)
            {
                ClearDetails();
                return;
            }

            try
            {
                string sql = @"
                    SELECT
                        sd.ProductID,
                        p.ProductCode  AS [Code],
                        p.ProductName  AS [Product Name],
                        sd.Qty         AS [Quantity],
                        sd.UnitPrice   AS [Unit Price],
                        sd.Qty * sd.UnitPrice AS [Amount]
                    FROM SALES_DETAIL sd
                    JOIN PRODUCT p ON p.ProductID = sd.ProductID
                    WHERE sd.InvoiceID = @id
                    ORDER BY p.ProductCode";

                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@id", selectedInvoiceID.Value));
                gvDetails.DataSource = dt;

                if (gvDetails.Columns["ProductID"] != null)
                    gvDetails.Columns["ProductID"].Visible = false;

                decimal total = 0;
                foreach (DataRow row in dt.Rows)
                    total += Convert.ToDecimal(row["Amount"]);
                lblTotal.Text = $"Total: ${total:N2}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading invoice details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDetails()
        {
            gvDetails.DataSource = null;
            lblTotal.Text = "Total: $0.00";
            btnDelete.Enabled = false;
            selectedInvoiceID = null;
        }

        private void gvInvoices_SelectionChanged(object sender, EventArgs e)
        {
            if (gvInvoices.SelectedRows.Count > 0)
            {
                var cell = gvInvoices.SelectedRows[0].Cells["InvoiceID"];
                if (cell.Value == null || cell.Value == DBNull.Value)
                    return;

                selectedInvoiceID = Convert.ToInt32(cell.Value);
                LoadInvoiceDetails();
                btnDelete.Enabled = true;
            }
            else
            {
                ClearDetails();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadInvoices();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadInvoices();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadInvoices();
                e.SuppressKeyPress = true;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Panel parentPanel = this.Parent as Panel;
            if (parentPanel == null) return;

            FrmInvoiceCreate frm = new FrmInvoiceCreate();
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            parentPanel.Controls.Clear();
            parentPanel.Controls.Add(frm);
            frm.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!selectedInvoiceID.HasValue)
            {
                MessageBox.Show("Please select an invoice to delete.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this invoice?\nProduct inventory will be restored.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = Db.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            // Restore inventory
                            string restoreSql = @"
                                UPDATE p
                                SET p.InventoryQty = p.InventoryQty + sd.Qty
                                FROM PRODUCT p
                                JOIN SALES_DETAIL sd ON sd.ProductID = p.ProductID
                                WHERE sd.InvoiceID = @id";
                            Db.ExecuteNonQuery(restoreSql, trans, new SqlParameter("@id", selectedInvoiceID.Value));

                            Db.ExecuteNonQuery("DELETE FROM SALES_DETAIL  WHERE InvoiceID = @id", trans, new SqlParameter("@id", selectedInvoiceID.Value));
                            Db.ExecuteNonQuery("DELETE FROM SALES_INVOICE WHERE InvoiceID = @id", trans, new SqlParameter("@id", selectedInvoiceID.Value));

                            trans.Commit();
                            MessageBox.Show("Invoice deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadInvoices();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
