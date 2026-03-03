using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmProducts : Form
    {
        public FrmProducts()
        {
            InitializeComponent();
        }

        private void FrmProducts_Load(object sender, EventArgs e)
        {
            // Guard: Check permission
            if (!AuthGuard.GuardForm(this, "PRODUCTS"))
                return;

            ApplyRoleBasedAccess();
            LoadCategories();
            LoadProducts();
        }

        private void ApplyRoleBasedAccess()
        {
            bool canManage = AuthGuard.CanManage("PRODUCTS");

            btnAdd.Visible    = canManage;
            btnEdit.Visible   = canManage;
            btnDelete.Visible = canManage;

            if (AuthGuard.IsReadOnly("PRODUCTS"))
            {
                this.Text = "Products (View Only)";
            }
        }

        private void LoadCategories()
        {
            try
            {
                string sql = "SELECT CategoryID, CategoryName FROM CATEGORY ORDER BY CategoryName";
                DataTable dt = Db.ExecuteDataTable(sql);

                DataRow allRow = dt.NewRow();
                allRow["CategoryID"] = 0;
                allRow["CategoryName"] = "-- All --";
                dt.Rows.InsertAt(allRow, 0);

                cboCategory.DataSource = dt;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                string keyword = txtSearch.Text == "Search by code or name..." ? "" : txtSearch.Text.Trim();

                int categoryId = 0;
                if (cboCategory.SelectedValue != null && cboCategory.SelectedValue != DBNull.Value)
                {
                    if (cboCategory.SelectedValue is int)
                    {
                        categoryId = (int)cboCategory.SelectedValue;
                    }
                    else if (cboCategory.SelectedValue is DataRowView)
                    {
                        DataRowView drv = (DataRowView)cboCategory.SelectedValue;
                        categoryId = Convert.ToInt32(drv["CategoryID"]);
                    }
                    else
                    {
                        categoryId = Convert.ToInt32(cboCategory.SelectedValue);
                    }
                }

                string sql = @"
                    SELECT p.ProductID, p.ProductCode AS [Code], p.ProductName AS [Product Name], 
                           c.CategoryName AS [Category], p.SellPrice AS [Price], 
                           p.InventoryQty AS [Stock], p.ReorderLevel AS [Reorder Level], 
                           CASE WHEN p.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS [Status]
                    FROM PRODUCT p
                    JOIN CATEGORY c ON c.CategoryID = p.CategoryID
                    WHERE (@kw IS NULL OR @kw = '' OR p.ProductCode LIKE '%' + @kw + '%' OR p.ProductName LIKE '%' + @kw + '%')
                      AND (@catId = 0 OR p.CategoryID = @catId)
                    ORDER BY p.ProductCode";

                DataTable dt = Db.ExecuteDataTable(sql,
                    new SqlParameter("@kw", string.IsNullOrWhiteSpace(keyword) ? (object)DBNull.Value : keyword),
                    new SqlParameter("@catId", categoryId));

                gvProducts.DataSource = dt;

                if (gvProducts.Columns["ProductID"] != null)
                    gvProducts.Columns["ProductID"].Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Search by code or name...")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = System.Drawing.Color.Black;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "Search by code or name...";
                txtSearch.ForeColor = System.Drawing.Color.Gray;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!AuthGuard.CanManage("PRODUCTS"))
            {
                MessageBox.Show("You do not have permission to add products.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FrmProductEdit frm = new FrmProductEdit();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!AuthGuard.CanManage("PRODUCTS"))
            {
                MessageBox.Show("You do not have permission to edit products.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (gvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to edit.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int productId = Convert.ToInt32(gvProducts.SelectedRows[0].Cells["ProductID"].Value);
            FrmProductEdit frm = new FrmProductEdit(productId);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!AuthGuard.CanManage("PRODUCTS"))
            {
                MessageBox.Show("You do not have permission to delete products.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (gvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this product?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    int productId = Convert.ToInt32(gvProducts.SelectedRows[0].Cells["ProductID"].Value);
                    string sql = "UPDATE PRODUCT SET IsActive = 0 WHERE ProductID = @id";
                    Db.ExecuteNonQuery(sql, null, new SqlParameter("@id", productId));

                    MessageBox.Show("Product deleted successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadProducts();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "Search by code or name...";
            txtSearch.ForeColor = System.Drawing.Color.Gray;
            cboCategory.SelectedIndex = 0;
            LoadProducts();
        }

        private void btnManageImages_Click(object sender, EventArgs e)
        {
            if (gvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to manage images.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int productId = Convert.ToInt32(gvProducts.SelectedRows[0].Cells["ProductID"].Value);
            FrmProductImages frm = new FrmProductImages(productId);
            frm.ShowDialog();
        }

        private void gvProducts_DoubleClick(object sender, EventArgs e)
        {
            // Double-click to manage images
            if (gvProducts.SelectedRows.Count > 0)
            {
                btnManageImages_Click(sender, e);
            }
        }
    }
}
