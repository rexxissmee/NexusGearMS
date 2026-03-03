using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Common;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmSuppliers : Form
    {
        private int? selectedSupplierID = null;

        public FrmSuppliers()
        {
            InitializeComponent();
        }

        private void FrmSuppliers_Load(object sender, EventArgs e)
        {
            // Guard: Check permission (Admin and Warehouse can manage suppliers)
            if (!AuthGuard.GuardForm(this, "SUPPLIERS"))
                return;

            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                if (txtSearch.Text == "Search by code/name...")
                    keyword = "";

                string sql = @"
                    SELECT SupplierID, SupplierCode AS [Code], SupplierName AS [Name], 
                           Phone AS [Phone], Address AS [Address],
                           CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS [Status],
                           FORMAT(CreatedAt, 'dd/MM/yyyy') AS [Created Date]
                    FROM SUPPLIER
                    WHERE (@kw IS NULL OR @kw = '' OR SupplierCode LIKE '%' + @kw + '%' 
                           OR SupplierName LIKE '%' + @kw + '%')
                    ORDER BY SupplierCode";

                DataTable dt = Db.ExecuteDataTable(sql,
                    new SqlParameter("@kw", string.IsNullOrWhiteSpace(keyword) ? (object)DBNull.Value : keyword));

                gvSuppliers.DataSource = dt;

                if (gvSuppliers.Columns["SupplierID"] != null)
                    gvSuppliers.Columns["SupplierID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading suppliers: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gvSuppliers_SelectionChanged(object sender, EventArgs e)
        {
            if (gvSuppliers.SelectedRows.Count > 0)
            {
                selectedSupplierID = Convert.ToInt32(gvSuppliers.SelectedRows[0].Cells["SupplierID"].Value);
                LoadSupplierDetails();
            }
            else
            {
                selectedSupplierID = null;
                ClearForm();
            }
        }

        private void LoadSupplierDetails()
        {
            if (!selectedSupplierID.HasValue)
                return;

            try
            {
                string sql = "SELECT * FROM SUPPLIER WHERE SupplierID = @id";
                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@id", selectedSupplierID.Value));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtCode.Text = row["SupplierCode"].ToString();
                    txtName.Text = row["SupplierName"].ToString();
                    txtPhone.Text = row["Phone"].ToString();
                    txtAddress.Text = row["Address"].ToString();
                    chkActive.Checked = Convert.ToBoolean(row["IsActive"]);

                    txtCode.ReadOnly = true;
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading supplier details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            selectedSupplierID = null;
            ClearForm();
            txtCode.ReadOnly = false;
            btnSave.Text = "Save";
            txtCode.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (selectedSupplierID.HasValue)
                {
                    UpdateSupplier();
                }
                else
                {
                    InsertSupplier();
                }

                LoadSuppliers();
                ClearForm();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Supplier code already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("Please enter supplier code.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCode.Focus();
                return false;
            }

            string code = txtCode.Text.Trim().ToUpper();
            if (!Validator.IsCode(code))
            {
                MessageBox.Show("Supplier code must be 3-20 characters (A-Z, 0-9, dash only).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter supplier name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                if (!Validator.IsPhone(txtPhone.Text.Trim()))
                {
                    MessageBox.Show("Phone number must be 9-15 digits.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhone.Focus();
                    return false;
                }
            }

            return true;
        }

        private void InsertSupplier()
        {
            string sql = @"
                INSERT INTO SUPPLIER(SupplierCode, SupplierName, Phone, Address, IsActive)
                VALUES (@code, @name, @phone, @address, @active)";

            string code = txtCode.Text.Trim().ToUpper();
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            Db.ExecuteNonQuery(sql, null,
                new SqlParameter("@code", code),
                new SqlParameter("@name", name),
                new SqlParameter("@phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone),
                new SqlParameter("@address", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address),
                new SqlParameter("@active", chkActive.Checked));

            MessageBox.Show("Supplier added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateSupplier()
        {
            string sql = @"
                UPDATE SUPPLIER
                SET SupplierName = @name, Phone = @phone, Address = @address, IsActive = @active
                WHERE SupplierID = @id";

            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            Db.ExecuteNonQuery(sql, null,
                new SqlParameter("@name", name),
                new SqlParameter("@phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone),
                new SqlParameter("@address", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address),
                new SqlParameter("@active", chkActive.Checked),
                new SqlParameter("@id", selectedSupplierID.Value));

            MessageBox.Show("Supplier updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!selectedSupplierID.HasValue)
            {
                MessageBox.Show("Please select a supplier to delete.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this supplier?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Check if supplier has import receipts
                    string checkSql = "SELECT COUNT(*) FROM IMPORT_RECEIPT WHERE SupplierID = @id";
                    int importCount = Convert.ToInt32(Db.ExecuteScalar(checkSql, null, new SqlParameter("@id", selectedSupplierID.Value)));

                    if (importCount > 0)
                    {
                        MessageBox.Show(
                            $"Cannot delete supplier with {importCount} import receipt(s).\n\nThis supplier has transaction history and must be kept for data integrity.",
                            "Cannot Delete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    string sql = "DELETE FROM SUPPLIER WHERE SupplierID = @id";
                    Db.ExecuteNonQuery(sql, null, new SqlParameter("@id", selectedSupplierID.Value));

                    MessageBox.Show("Supplier deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSuppliers();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting supplier: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtCode.Clear();
            txtName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            chkActive.Checked = true;
            txtCode.ReadOnly = false;
            btnSave.Text = "Save";
            selectedSupplierID = null;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Search by code/name...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Search by code/name...";
                txtSearch.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "Search by code/name...";
            txtSearch.ForeColor = System.Drawing.Color.Gray;
            LoadSuppliers();
        }
    }
}
