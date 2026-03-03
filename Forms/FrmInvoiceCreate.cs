using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmInvoiceCreate : Form
    {
        private DataTable detailsTable;

        public FrmInvoiceCreate()
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
            detailsTable.Columns.Add("Unit Price", typeof(decimal));
            detailsTable.Columns.Add("Unit Cost", typeof(decimal));
            detailsTable.Columns.Add("Amount", typeof(decimal));
        }

        private void FrmInvoiceCreate_Load(object sender, EventArgs e)
        {
            // Guard: Check permission
            if (!AuthGuard.GuardForm(this, "INVOICES"))
                return;

            LoadCustomers();
            LoadProducts();
            dtInvoiceDate.Value = DateTime.Now;
            txtInvoiceCode.Text = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
            gvLines.DataSource = detailsTable;
            if (gvLines.Columns["ProductID"] != null)
                gvLines.Columns["ProductID"].Visible = false;
        }

        private void LoadCustomers()
        {
            try
            {
                string sql = "SELECT CustomerID, CustomerCode + ' - ' + FullName AS DisplayName FROM CUSTOMER ORDER BY CustomerCode";
                DataTable dt = Db.ExecuteDataTable(sql);

                DataRow walkIn = dt.NewRow();
                walkIn["CustomerID"] = DBNull.Value;
                walkIn["DisplayName"] = "-- Walk-in Customer --";
                dt.Rows.InsertAt(walkIn, 0);

                cboCustomer.DataSource = dt;
                cboCustomer.DisplayMember = "DisplayName";
                cboCustomer.ValueMember = "CustomerID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customers: " + ex.Message);
            }
        }

        private void LoadProducts()
        {
            try
            {
                string sql = "SELECT ProductID, ProductCode, ProductName, SellPrice, InventoryQty, ProductCode + ' - ' + ProductName AS DisplayName FROM PRODUCT WHERE IsActive = 1 AND InventoryQty > 0 ORDER BY ProductCode";
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

            DataRowView drv = (DataRowView)cboProduct.SelectedItem;
            int productId = Convert.ToInt32(drv["ProductID"]);
            string productCode = drv["ProductCode"].ToString();
            string productName = drv["ProductName"].ToString();
            decimal sellPrice = Convert.ToDecimal(drv["SellPrice"]);
            int inventory = Convert.ToInt32(drv["InventoryQty"]);

            if (qty > inventory)
            {
                MessageBox.Show($"Only {inventory} units available in stock!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get unit cost from the most recent import receipt for display
            string getUnitCostSql = @"
                SELECT TOP 1 UnitCost 
                FROM IMPORT_DETAIL id
                JOIN IMPORT_RECEIPT ir ON ir.ImportID = id.ImportID
                WHERE id.ProductID = @productId
                ORDER BY ir.ImportDate DESC";

            object costResult = Db.ExecuteScalar(getUnitCostSql, null, new SqlParameter("@productId", productId));
            decimal unitCost = (costResult != null && costResult != DBNull.Value) 
                ? Convert.ToDecimal(costResult) 
                : 0;

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

                // Check total quantity against inventory
                if (newQty > inventory)
                {
                    MessageBox.Show($"Total quantity ({newQty}) would exceed available stock ({inventory})!", 
                        "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                existingRow["Quantity"] = newQty;
                existingRow["Amount"] = newQty * sellPrice;
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
                newRow["Unit Price"] = sellPrice;
                newRow["Unit Cost"] = unitCost;
                newRow["Amount"] = qty * sellPrice;
                detailsTable.Rows.Add(newRow);
            }

            txtQty.Text = "1";
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
                            string insertHeader = @"
                                INSERT INTO SALES_INVOICE(InvoiceCode, InvoiceDate, CustomerID, EmpID, Status)
                                VALUES (@code, @date, @customerId, @empId, 'COMPLETED');
                                SELECT SCOPE_IDENTITY();";

                            object customerIdValue = DBNull.Value;
                            if (cboCustomer.SelectedItem != null)
                            {
                                DataRowView customerRow = (DataRowView)cboCustomer.SelectedItem;
                                if (customerRow["CustomerID"] != DBNull.Value)
                                {
                                    customerIdValue = Convert.ToInt32(customerRow["CustomerID"]);
                                }
                            }

                            int invoiceId = Convert.ToInt32(Db.ExecuteScalar(insertHeader, trans,
                                new SqlParameter("@code", txtInvoiceCode.Text),
                                new SqlParameter("@date", dtInvoiceDate.Value),
                                new SqlParameter("@customerId", customerIdValue),
                                new SqlParameter("@empId", SessionManager.EmpID)));

                            foreach (DataRow row in detailsTable.Rows)
                            {
                                int productId = Convert.ToInt32(row["ProductID"]);
                                int qty = Convert.ToInt32(row["Quantity"]);
                                decimal unitPrice = Convert.ToDecimal(row["Unit Price"]);

                                // Get average unit cost from latest import receipt
                                string getUnitCostSql = @"
                                    SELECT TOP 1 UnitCost 
                                    FROM IMPORT_DETAIL id
                                    JOIN IMPORT_RECEIPT ir ON ir.ImportID = id.ImportID
                                    WHERE id.ProductID = @productId
                                    ORDER BY ir.ImportDate DESC";

                                object costResult = Db.ExecuteScalar(getUnitCostSql, trans,
                                    new SqlParameter("@productId", productId));

                                decimal unitCostAtSale = (costResult != null && costResult != DBNull.Value) 
                                    ? Convert.ToDecimal(costResult) 
                                    : 0;

                                string insertDetail = @"
                                    INSERT INTO SALES_DETAIL(InvoiceID, ProductID, Qty, UnitPrice, UnitCostAtSale)
                                    VALUES (@invoiceId, @productId, @qty, @unitPrice, @unitCostAtSale)";

                                Db.ExecuteNonQuery(insertDetail, trans,
                                    new SqlParameter("@invoiceId", invoiceId),
                                    new SqlParameter("@productId", productId),
                                    new SqlParameter("@qty", qty),
                                    new SqlParameter("@unitPrice", unitPrice),
                                    new SqlParameter("@unitCostAtSale", unitCostAtSale));

                                // UPDATE INVENTORY: Decrease quantity sold
                                string updateInventory = @"
                                    UPDATE PRODUCT 
                                    SET InventoryQty = InventoryQty - @qty 
                                    WHERE ProductID = @productId";

                                Db.ExecuteNonQuery(updateInventory, trans,
                                    new SqlParameter("@qty", qty),
                                    new SqlParameter("@productId", productId));
                            }

                            trans.Commit();
                            MessageBox.Show("Invoice saved successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            txtInvoiceCode.Text = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss");
            cboCustomer.SelectedIndex = 0;
            detailsTable.Clear();
            CalculateTotal();
        }
    }
}
