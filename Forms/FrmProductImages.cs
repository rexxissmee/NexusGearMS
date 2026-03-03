using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmProductImages : Form
    {
        private int? productID = null;
        private int? selectedImageID = null;

        public FrmProductImages()
        {
            InitializeComponent();
        }

        public FrmProductImages(int productId) : this()
        {
            this.productID = productId;
        }

        private void FrmProductImages_Load(object sender, EventArgs e)
        {
            if (!AuthGuard.GuardForm(this, "PRODUCT_IMAGES"))
                return;

            ApplyRoleBasedAccess();
            LoadProducts();

            if (productID.HasValue)
            {
                cbProduct.SelectedValue = productID.Value;
                cbProduct.Enabled = false;
                LoadImages();
            }
        }

        private void ApplyRoleBasedAccess()
        {
            bool canManage = AuthGuard.CanManage("PRODUCT_IMAGES");

            grpImageDetails.Visible = canManage;
            btnAdd.Visible          = canManage;
            btnDelete.Visible       = canManage;
            btnSetMain.Visible      = canManage;
            btnClear.Visible        = canManage;

            if (!canManage)
                this.Text = "Product Images (View Only)";
        }

        private void LoadProducts()
        {
            try
            {
                string sql = "SELECT ProductID, ProductCode + ' - ' + ProductName AS DisplayName FROM PRODUCT WHERE IsActive = 1 ORDER BY ProductCode";
                DataTable dt = Db.ExecuteDataTable(sql);

                cbProduct.DataSource = dt;
                cbProduct.DisplayMember = "DisplayName";
                cbProduct.ValueMember = "ProductID";
                cbProduct.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbProduct.SelectedValue != null && int.TryParse(cbProduct.SelectedValue.ToString(), out int id))
            {
                productID = id;
                LoadImages();
            }
        }

        private void LoadImages()
        {
            if (!productID.HasValue)
                return;

            try
            {
                // PRODUCT_IMAGE table only has: ImageID, ProductID, ImageUrl, IsMain
                // No ImagePath or CreatedDate columns
                string sql = @"
                    SELECT ImageID, ImageUrl, IsMain
                    FROM PRODUCT_IMAGE
                    WHERE ProductID = @productId
                    ORDER BY IsMain DESC, ImageID DESC";

                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@productId", productID));

                dgvImages.DataSource = dt;

                // Format columns
                if (dgvImages.Columns.Contains("ImageID"))
                    dgvImages.Columns["ImageID"].Visible = false;

                dgvImages.Columns["ImageUrl"].HeaderText = "Image URL";
                dgvImages.Columns["IsMain"].HeaderText = "Main Image";

                lblImageCount.Text = $"Total: {dt.Rows.Count} image(s)";

                // Display first image if exists
                if (dt.Rows.Count > 0)
                {
                    DisplayImage(dt.Rows[0]);
                }
                else
                {
                    picPreview.Image = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading images: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvImages_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvImages.CurrentRow != null && dgvImages.CurrentRow.Index >= 0)
            {
                DataGridViewRow row = dgvImages.CurrentRow;
                selectedImageID = Convert.ToInt32(row.Cells["ImageID"].Value);

                txtImageUrl.Text = row.Cells["ImageUrl"].Value?.ToString() ?? "";
                // ImagePath field removed - only use ImageUrl
                txtImagePath.Clear();

                // Display image
                DisplayImage(dgvImages.CurrentRow);

                // Enable set main button if not already main
                bool isMain = Convert.ToBoolean(row.Cells["IsMain"].Value);
                btnSetMain.Enabled = !isMain;
            }
        }

        private void DisplayImage(DataGridViewRow row)
        {
            try
            {
                string imageUrl = row.Cells["ImageUrl"].Value?.ToString();

                // Try to load from URL (assuming it's a local file path or URL)
                if (!string.IsNullOrWhiteSpace(imageUrl) && File.Exists(imageUrl))
                {
                    picPreview.Image = System.Drawing.Image.FromFile(imageUrl);
                    return;
                }

                // No image available
                picPreview.Image = null;
            }
            catch
            {
                picPreview.Image = null;
            }
        }

        private void DisplayImage(DataRow row)
        {
            try
            {
                string imageUrl = row["ImageUrl"]?.ToString();

                // Try to load from URL (assuming it's a local file path or URL)
                if (!string.IsNullOrWhiteSpace(imageUrl) && File.Exists(imageUrl))
                {
                    picPreview.Image = System.Drawing.Image.FromFile(imageUrl);
                    return;
                }

                // No image available
                picPreview.Image = null;
            }
            catch
            {
                picPreview.Image = null;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                ofd.Title = "Select Product Image";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtImagePath.Text = ofd.FileName;

                    // Preview
                    try
                    {
                        picPreview.Image = System.Drawing.Image.FromFile(ofd.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!productID.HasValue)
            {
                MessageBox.Show("Please select a product.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Use ImagePath as ImageUrl (only ImageUrl column exists in DB)
            string imageUrl = string.IsNullOrWhiteSpace(txtImagePath.Text) ? txtImageUrl.Text : txtImagePath.Text;

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                MessageBox.Show("Please provide an Image URL or select a file.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // PRODUCT_IMAGE table only has: ImageID, ProductID, ImageUrl, IsMain
                string sql = @"
                    INSERT INTO PRODUCT_IMAGE (ProductID, ImageUrl, IsMain)
                    VALUES (@productId, @url, 0)";

                Db.ExecuteNonQuery(sql, null,
                    new SqlParameter("@productId", productID.Value),
                    new SqlParameter("@url", imageUrl.Trim()));

                MessageBox.Show("Image added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadImages();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedImageID == null)
            {
                MessageBox.Show("Please select an image to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this image?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    string sql = "DELETE FROM PRODUCT_IMAGE WHERE ImageID = @id";
                    Db.ExecuteNonQuery(sql, null, new SqlParameter("@id", selectedImageID));

                    MessageBox.Show("Image deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadImages();
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSetMain_Click(object sender, EventArgs e)
        {
            if (!productID.HasValue || selectedImageID == null)
            {
                MessageBox.Show("Please select an image.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    using (var tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // Set all images of this product to non-main
                            string sql1 = "UPDATE PRODUCT_IMAGE SET IsMain = 0 WHERE ProductID = @productId";
                            Db.ExecuteNonQuery(sql1, tx, new SqlParameter("@productId", productID.Value));

                            // Set selected image as main
                            string sql2 = "UPDATE PRODUCT_IMAGE SET IsMain = 1 WHERE ImageID = @id";
                            Db.ExecuteNonQuery(sql2, tx, new SqlParameter("@id", selectedImageID));

                            tx.Commit();

                            MessageBox.Show("Main image updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadImages();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting main image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            selectedImageID = null;
            txtImageUrl.Clear();
            txtImagePath.Clear();
            btnSetMain.Enabled = false;

            if (dgvImages.Rows.Count > 0)
                dgvImages.ClearSelection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
