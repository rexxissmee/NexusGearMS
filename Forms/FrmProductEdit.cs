using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmProductEdit : Form
    {
        private int? productId;

        public FrmProductEdit(int? productId = null)
        {
            InitializeComponent();
            this.productId = productId;
        }

        private void FrmProductEdit_Load(object sender, EventArgs e)
        {
            if (!AuthGuard.CanManage("PRODUCTS"))
            {
                MessageBox.Show(
                    "You do not have permission to add or edit products.",
                    "Access Denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            LoadCategories();

            if (productId.HasValue)
            {
                LoadProduct();
                txtCode.ReadOnly = true;
            }
            else
            {
                txtInventory.Text = "0";
                txtReorder.Text = "10";
            }
        }

        private void LoadCategories()
        {
            try
            {
                string sql = "SELECT CategoryID, CategoryName FROM CATEGORY ORDER BY CategoryName";
                DataTable dt = Db.ExecuteDataTable(sql);

                cboCategory.DataSource = dt;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProduct()
        {
            try
            {
                string sql = "SELECT * FROM PRODUCT WHERE ProductID = @id";
                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@id", productId.Value));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtCode.Text = row["ProductCode"].ToString();
                    txtName.Text = row["ProductName"].ToString();
                    cboCategory.SelectedValue = row["CategoryID"];
                    txtPrice.Text = row["SellPrice"].ToString();
                    txtInventory.Text = row["InventoryQty"].ToString();
                    txtReorder.Text = row["ReorderLevel"].ToString();
                    chkActive.Checked = Convert.ToBoolean(row["IsActive"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (productId.HasValue)
                {
                    UpdateProduct();
                }
                else
                {
                    InsertProduct();
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Product code already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Please enter product code.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter product name.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Please select a category.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            decimal price;
            if (!decimal.TryParse(txtPrice.Text, out price) || price <= 0)
            {
                MessageBox.Show("Price must be greater than 0.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus();
                return false;
            }

            int inventory;
            if (!int.TryParse(txtInventory.Text, out inventory) || inventory < 0)
            {
                MessageBox.Show("Inventory quantity cannot be negative.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtInventory.Focus();
                return false;
            }

            int reorder;
            if (!int.TryParse(txtReorder.Text, out reorder) || reorder < 0)
            {
                MessageBox.Show("Reorder level cannot be negative.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtReorder.Focus();
                return false;
            }

            return true;
        }

        private void InsertProduct()
        {
            string sql = @"
                INSERT INTO PRODUCT(ProductCode, ProductName, CategoryID, SellPrice, InventoryQty, ReorderLevel, IsActive)
                VALUES (@code, @name, @catId, @price, @inv, @reorder, @active)";

            Db.ExecuteNonQuery(sql, null,
                new SqlParameter("@code", txtCode.Text.Trim()),
                new SqlParameter("@name", txtName.Text.Trim()),
                new SqlParameter("@catId", cboCategory.SelectedValue),
                new SqlParameter("@price", decimal.Parse(txtPrice.Text)),
                new SqlParameter("@inv", int.Parse(txtInventory.Text)),
                new SqlParameter("@reorder", int.Parse(txtReorder.Text)),
                new SqlParameter("@active", chkActive.Checked));

            MessageBox.Show("Product added successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateProduct()
        {
            string sql = @"
                UPDATE PRODUCT
                SET ProductName = @name, CategoryID = @catId, SellPrice = @price,
                    InventoryQty = @inv, ReorderLevel = @reorder, IsActive = @active
                WHERE ProductID = @id";

            Db.ExecuteNonQuery(sql, null,
                new SqlParameter("@name", txtName.Text.Trim()),
                new SqlParameter("@catId", cboCategory.SelectedValue),
                new SqlParameter("@price", decimal.Parse(txtPrice.Text)),
                new SqlParameter("@inv", int.Parse(txtInventory.Text)),
                new SqlParameter("@reorder", int.Parse(txtReorder.Text)),
                new SqlParameter("@active", chkActive.Checked),
                new SqlParameter("@id", productId.Value));

            MessageBox.Show("Product updated successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
