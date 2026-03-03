using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmImportCreate : Form
    {
        private DataTable detailsTable;

        public FrmImportCreate()
        {
            InitializeComponent();
            InitializeDetailsTable();
        }

        private void InitializeDetailsTable()
        {
            detailsTable = new DataTable();
            detailsTable.Columns.Add("ProductID", typeof(int));
            detailsTable.Columns.Add("Code", typeof(string));
            detailsTable.Columns.Add("Product Name", typeof(string));
            detailsTable.Columns.Add("Quantity", typeof(int));
            detailsTable.Columns.Add("Unit Cost", typeof(decimal));
            detailsTable.Columns.Add("Amount", typeof(decimal));
        }

        private void FrmImportCreate_Load(object sender, EventArgs e)
        {
            // Guard: Check permission
            if (!AuthGuard.GuardForm(this, "IMPORTS"))
                return;

            LoadSuppliers();
            LoadProducts();
            dtImportDate.Value = DateTime.Now;
            txtImportCode.Text = "IMP" + DateTime.Now.ToString("yyyyMMddHHmmss");
            gvLines.DataSource = detailsTable;
            if (gvLines.Columns["ProductID"] != null)
                gvLines.Columns["ProductID"].Visible = false;
        }

        private void LoadSuppliers()
        {
            try
            {
                string sql = "SELECT SupplierID, SupplierCode + ' - ' + SupplierName AS DisplayName FROM SUPPLIER WHERE IsActive = 1 ORDER BY SupplierCode";
                DataTable dt = Db.ExecuteDataTable(sql);
                cboSupplier.DataSource = dt;
                cboSupplier.DisplayMember = "DisplayName";
                cboSupplier.ValueMember = "SupplierID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message);
            }
        }

        private void LoadProducts()
        {
            try
            {
                string sql = "SELECT ProductID, ProductCode, ProductName, ProductCode + ' - ' + ProductName AS DisplayName FROM PRODUCT WHERE IsActive = 1 ORDER BY ProductCode";
                DataTable dt = Db.ExecuteDataTable(sql);
                cboProduct.DataSource = dt;
                cboProduct.DisplayMember = "DisplayName";
                cboProduct.ValueMember = "ProductID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message);
            }
        }

        private void btnAddLine_Click(object sender, EventArgs e)
        {
            if (cboProduct.SelectedItem == null)
            {
                MessageBox.Show("Please select a product.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int qty;
            if (!int.TryParse(txtQty.Text, out qty) || qty <= 0)
            {
                MessageBox.Show("Quantity must be greater than 0.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal unitCost;
            if (!decimal.TryParse(txtUnitCost.Text, out unitCost) || unitCost < 0)
            {
                MessageBox.Show("Unit cost must be >= 0.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRowView drv = (DataRowView)cboProduct.SelectedItem;
            int productId = Convert.ToInt32(drv["ProductID"]);
            string productCode = drv["ProductCode"].ToString();
            string productName = drv["ProductName"].ToString();

            // Check if product already exists in the detail table
            DataRow existingRow = null;
            foreach (DataRow row in detailsTable.Rows)
            {
                if (Convert.ToInt32(row["ProductID"]) == productId)
                {
                    existingRow = row;
                    break;
                }
            }

            if (existingRow != null)
            {
                // Product exists, accumulate quantity
                int currentQty = Convert.ToInt32(existingRow["Quantity"]);
                int newQty = currentQty + qty;

                existingRow["Quantity"] = newQty;
                existingRow["Unit Cost"] = unitCost;  // Update unit cost with latest value
                existingRow["Amount"] = newQty * unitCost;
                MessageBox.Show($"Product quantity updated to {newQty}.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Product doesn't exist, add new row
                DataRow newRow = detailsTable.NewRow();
                newRow["ProductID"] = productId;
                newRow["Code"] = productCode;
                newRow["Product Name"] = productName;
                newRow["Quantity"] = qty;
                newRow["Unit Cost"] = unitCost;
                newRow["Amount"] = qty * unitCost;
                detailsTable.Rows.Add(newRow);
            }

            txtQty.Text = "1";
            txtUnitCost.Text = "0";
            CalculateTotal();
        }

        private void btnRemoveLine_Click(object sender, EventArgs e)
        {
            if (gvLines.SelectedRows.Count > 0)
            {
                gvLines.Rows.Remove(gvLines.SelectedRows[0]);
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            decimal total = 0;
            foreach (DataRow row in detailsTable.Rows)
            {
                total += Convert.ToDecimal(row["Amount"]);
            }
            lblTotal.Text = $"Total: ${total:N2}";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboSupplier.SelectedItem == null)
            {
                MessageBox.Show("Please select a supplier.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (detailsTable.Rows.Count == 0)
            {
                MessageBox.Show("Please add at least 1 product.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = Db.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        try
                        {
                            DataRowView supplierRow = (DataRowView)cboSupplier.SelectedItem;
                            int supplierId = Convert.ToInt32(supplierRow["SupplierID"]);

                            string insertHeader = @"
                                INSERT INTO IMPORT_RECEIPT(ImportCode, ImportDate, SupplierID, EmpID, Note)
                                VALUES (@code, @date, @supplierId, @empId, @note);
                                SELECT SCOPE_IDENTITY();";

                            int importId = Convert.ToInt32(Db.ExecuteScalar(insertHeader, trans,
                                new SqlParameter("@code", txtImportCode.Text),
                                new SqlParameter("@date", dtImportDate.Value),
                                new SqlParameter("@supplierId", supplierId),
                                new SqlParameter("@empId", SessionManager.EmpID),
                                new SqlParameter("@note", txtNote.Text)));

                            foreach (DataRow row in detailsTable.Rows)
                            {
                                int productId = Convert.ToInt32(row["ProductID"]);
                                int qty = Convert.ToInt32(row["Quantity"]);
                                decimal unitCost = Convert.ToDecimal(row["Unit Cost"]);

                                string insertDetail = @"
                                    INSERT INTO IMPORT_DETAIL(ImportID, ProductID, Qty, UnitCost)
                                    VALUES (@importId, @productId, @qty, @unitCost)";

                                Db.ExecuteNonQuery(insertDetail, trans,
                                    new SqlParameter("@importId", importId),
                                    new SqlParameter("@productId", productId),
                                    new SqlParameter("@qty", qty),
                                    new SqlParameter("@unitCost", unitCost));

                                // UPDATE INVENTORY: Add imported quantity
                                string updateInventory = @"
                                    UPDATE PRODUCT 
                                    SET InventoryQty = InventoryQty + @qty 
                                    WHERE ProductID = @productId";

                                Db.ExecuteNonQuery(updateInventory, trans,
                                    new SqlParameter("@qty", qty),
                                    new SqlParameter("@productId", productId));
                            }

                            trans.Commit();
                            MessageBox.Show("Import receipt saved successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ClearForm();
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
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtImportCode.Text = "IMP" + DateTime.Now.ToString("yyyyMMddHHmmss");
            cboSupplier.SelectedIndex = 0;
            txtNote.Clear();
            detailsTable.Clear();
            CalculateTotal();
        }
    }
}
