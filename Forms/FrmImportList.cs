using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmImportList : Form
    {
        private int? selectedImportID = null;

        public FrmImportList()
        {
            InitializeComponent();
        }

        private void FrmImportList_Load(object sender, EventArgs e)
        {
            if (!AuthGuard.GuardForm(this, "IMPORTS"))
                return;

            dtFrom.Value = DateTime.Now.AddMonths(-1);
            dtTo.Value = DateTime.Now;
            LoadImports();
        }

        private void LoadImports()
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                string sql = @"
                    SELECT
                        ir.ImportID,
                        ir.ImportCode        AS [Import Code],
                        CONVERT(varchar, ir.ImportDate, 103) AS [Date],
                        s.SupplierName       AS [Supplier],
                        e.FullName           AS [Employee],
                        ISNULL(ir.Note, '') AS [Note],
                        ISNULL(SUM(id.Qty * id.UnitCost), 0) AS [Total Amount]
                    FROM IMPORT_RECEIPT ir
                    LEFT JOIN SUPPLIER s  ON s.SupplierID = ir.SupplierID
                    LEFT JOIN EMPLOYEE e  ON e.EmpID      = ir.EmpID
                    LEFT JOIN IMPORT_DETAIL id ON id.ImportID = ir.ImportID
                    WHERE ir.ImportDate BETWEEN @from AND @to
                      AND (@keyword = '' OR ir.ImportCode LIKE @keyword OR ISNULL(s.SupplierName,'') LIKE @keyword)
                    GROUP BY ir.ImportID, ir.ImportCode, ir.ImportDate,
                             s.SupplierName, e.FullName, ir.Note
                    ORDER BY ir.ImportDate DESC, ir.ImportID DESC";

                DataTable dt = Db.ExecuteDataTable(sql,
                    new SqlParameter("@from",    dtFrom.Value.Date),
                    new SqlParameter("@to",      dtTo.Value.Date.AddDays(1).AddSeconds(-1)),
                    new SqlParameter("@keyword", string.IsNullOrEmpty(keyword) ? "" : "%" + keyword + "%"));

                gvImports.DataSource = dt;

                if (gvImports.Columns["ImportID"] != null)
                    gvImports.Columns["ImportID"].Visible = false;

                ClearDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading import receipts: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadImportDetails()
        {
            if (!selectedImportID.HasValue)
            {
                ClearDetails();
                return;
            }

            try
            {
                string sql = @"
                    SELECT
                        id.ProductID,
                        p.ProductCode AS [Code],
                        p.ProductName AS [Product Name],
                        id.Qty        AS [Quantity],
                        id.UnitCost   AS [Unit Cost],
                        id.Qty * id.UnitCost AS [Amount]
                    FROM IMPORT_DETAIL id
                    JOIN PRODUCT p ON p.ProductID = id.ProductID
                    WHERE id.ImportID = @id
                    ORDER BY p.ProductCode";

                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@id", selectedImportID.Value));
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
                MessageBox.Show("Error loading import details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearDetails()
        {
            gvDetails.DataSource = null;
            lblTotal.Text = "Total: $0.00";
            btnDelete.Enabled = false;
            selectedImportID = null;
        }

        private void gvImports_SelectionChanged(object sender, EventArgs e)
        {
            if (gvImports.SelectedRows.Count > 0)
            {
                var cell = gvImports.SelectedRows[0].Cells["ImportID"];
                if (cell.Value == null || cell.Value == DBNull.Value)
                    return;

                selectedImportID = Convert.ToInt32(cell.Value);
                LoadImportDetails();
                btnDelete.Enabled = true;
            }
            else
            {
                ClearDetails();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadImports();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadImports();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadImports();
                e.SuppressKeyPress = true;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Panel parentPanel = this.Parent as Panel;
            if (parentPanel == null) return;

            FrmImportCreate frm = new FrmImportCreate();
            frm.TopLevel = false;
            frm.FormBorderStyle = FormBorderStyle.None;
            frm.Dock = DockStyle.Fill;
            parentPanel.Controls.Clear();
            parentPanel.Controls.Add(frm);
            frm.Show();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!selectedImportID.HasValue)
            {
                MessageBox.Show("Please select an import receipt to delete.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this import receipt?\nProduct inventory will be decreased accordingly.",
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
                            // Decrease inventory
                            string restoreSql = @"
                                UPDATE p
                                SET p.InventoryQty = p.InventoryQty - id.Qty
                                FROM PRODUCT p
                                JOIN IMPORT_DETAIL id ON id.ProductID = p.ProductID
                                WHERE id.ImportID = @id";
                            Db.ExecuteNonQuery(restoreSql, trans, new SqlParameter("@id", selectedImportID.Value));

                            Db.ExecuteNonQuery("DELETE FROM IMPORT_DETAIL  WHERE ImportID = @id", trans, new SqlParameter("@id", selectedImportID.Value));
                            Db.ExecuteNonQuery("DELETE FROM IMPORT_RECEIPT WHERE ImportID = @id", trans, new SqlParameter("@id", selectedImportID.Value));

                            trans.Commit();
                            MessageBox.Show("Import receipt deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadImports();
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
                MessageBox.Show("Error deleting import receipt: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
