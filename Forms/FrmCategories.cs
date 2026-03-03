using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmCategories : Form
    {
        private int? selectedCategoryID = null;

        public FrmCategories()
        {
            InitializeComponent();
        }

        private void FrmCategories_Load(object sender, EventArgs e)
        {
            // Guard: Admin only
            if (!AuthGuard.GuardForm(this, "CATEGORIES"))
                return;

            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                string sql;
                DataTable dt;

                if (string.IsNullOrEmpty(keyword))
                {
                    sql = @"
                        SELECT CategoryID, CategoryName AS [Category Name],
                               CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS [Status]
                        FROM CATEGORY
                        ORDER BY CategoryName";
                    dt = Db.ExecuteDataTable(sql);
                }
                else
                {
                    sql = @"
                        SELECT CategoryID, CategoryName AS [Category Name],
                               CASE WHEN IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS [Status]
                        FROM CATEGORY
                        WHERE CategoryName LIKE @keyword
                        ORDER BY CategoryName";
                    dt = Db.ExecuteDataTable(sql, new SqlParameter("@keyword", "%" + keyword + "%"));
                }

                gvCategories.DataSource = dt;

                if (gvCategories.Columns["CategoryID"] != null)
                    gvCategories.Columns["CategoryID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void gvCategories_SelectionChanged(object sender, EventArgs e)
        {
            if (gvCategories.SelectedRows.Count > 0)
            {
                selectedCategoryID = Convert.ToInt32(gvCategories.SelectedRows[0].Cells["CategoryID"].Value);
                LoadCategoryDetails();
                btnSave.Enabled = true;
                btnDelete.Enabled = true;
                btnCancel.Enabled = true;
            }
            else
            {
                selectedCategoryID = null;
                ClearForm();
            }
        }

        private void LoadCategoryDetails()
        {
            if (!selectedCategoryID.HasValue)
                return;

            try
            {
                string sql = "SELECT * FROM CATEGORY WHERE CategoryID = @id";
                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@id", selectedCategoryID.Value));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtName.Text = row["CategoryName"].ToString();
                    chkActive.Checked = Convert.ToBoolean(row["IsActive"]);
                    btnSave.Text = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading category: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            selectedCategoryID = null;
            ClearForm();
            btnSave.Text = "Save";
            btnSave.Enabled = true;
            btnDelete.Enabled = false;
            btnCancel.Enabled = true;
            txtName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (selectedCategoryID.HasValue)
                {
                    UpdateCategory();
                }
                else
                {
                    InsertCategory();
                }

                LoadCategories();
                ClearForm();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Category name already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter category name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            return true;
        }

        private void InsertCategory()
        {
            string sql = @"
                INSERT INTO CATEGORY(CategoryName, IsActive)
                VALUES (@name, @active)";

            string name = txtName.Text.Trim();

            Db.ExecuteNonQuery(sql, null,
                new SqlParameter("@name", name),
                new SqlParameter("@active", chkActive.Checked));

            MessageBox.Show("Category added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateCategory()
        {
            string sql = @"
                UPDATE CATEGORY
                SET CategoryName = @name, IsActive = @active
                WHERE CategoryID = @id";

            string name = txtName.Text.Trim();

            Db.ExecuteNonQuery(sql, null,
                new SqlParameter("@name", name),
                new SqlParameter("@active", chkActive.Checked),
                new SqlParameter("@id", selectedCategoryID.Value));

            MessageBox.Show("Category updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!selectedCategoryID.HasValue)
            {
                MessageBox.Show("Please select a category to delete.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this category?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Check if category has products
                    string checkSql = "SELECT COUNT(*) FROM PRODUCT WHERE CategoryID = @id";
                    int productCount = Convert.ToInt32(Db.ExecuteScalar(checkSql, null, new SqlParameter("@id", selectedCategoryID.Value)));

                    if (productCount > 0)
                    {
                        MessageBox.Show(
                            $"Cannot delete category with {productCount} product(s).\n\nPlease move or delete products in this category first.",
                            "Cannot Delete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        return;
                    }

                    string sql = "DELETE FROM CATEGORY WHERE CategoryID = @id";
                    Db.ExecuteNonQuery(sql, null, new SqlParameter("@id", selectedCategoryID.Value));

                    MessageBox.Show("Category deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCategories();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting category: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtName.Clear();
            chkActive.Checked = true;
            btnSave.Text = "Save";
            btnSave.Enabled = false;
            btnDelete.Enabled = false;
            btnCancel.Enabled = false;
            selectedCategoryID = null;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadCategories();
                e.SuppressKeyPress = true;
            }
        }
    }
}
