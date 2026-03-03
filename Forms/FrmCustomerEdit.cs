using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Common;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmCustomerEdit : Form
    {
        private int? customerId;

        public FrmCustomerEdit(int? customerId = null)
        {
            InitializeComponent();
            this.customerId = customerId;
        }

        private void FrmCustomerEdit_Load(object sender, EventArgs e)
        {
            // Guard: Check permission (Admin and Sales can add/edit customers)
            if (!AuthGuard.HasPermission("CUSTOMERS"))
            {
                MessageBox.Show(
                    "You do not have permission to add or edit customers.",
                    "Access Denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            if (customerId.HasValue)
            {
                LoadCustomer();
                txtCode.ReadOnly = true;
            }
            else
            {
            }
        }

        private void LoadCustomer()
        {
            try
            {
                string sql = "SELECT * FROM CUSTOMER WHERE CustomerID = @id";
                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@id", customerId.Value));

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    txtCode.Text = row["CustomerCode"].ToString();
                    txtName.Text = row["FullName"].ToString();
                    txtPhone.Text = row["Phone"].ToString();
                    txtAddress.Text = row["Address"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                if (customerId.HasValue)
                {
                    UpdateCustomer();
                }
                else
                {
                    InsertCustomer();
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Customer code already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Please enter customer code.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCode.Focus();
                return false;
            }

            // Validate code format
            string code = txtCode.Text.Trim().ToUpper();
            if (!Validator.IsCode(code))
            {
                MessageBox.Show("Customer code must be 3-20 characters (A-Z, 0-9, dash only).", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCode.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter full name.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }

            // Validate phone if provided
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                if (!Validator.IsPhone(txtPhone.Text.Trim()))
                {
                    MessageBox.Show("Phone number must be 9-15 digits.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPhone.Focus();
                    return false;
                }
            }

            return true;
        }

        private void InsertCustomer()
        {
            string sql = @"
                INSERT INTO CUSTOMER(CustomerCode, FullName, Phone, Address)
                VALUES (@code, @name, @phone, @address)";

            string code = txtCode.Text.Trim().ToUpper();
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            Db.ExecuteNonQuery(sql, null,
                new SqlParameter("@code", code),
                new SqlParameter("@name", name),
                new SqlParameter("@phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone),
                new SqlParameter("@address", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address));

            MessageBox.Show("Customer added successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateCustomer()
        {
            string sql = @"
                UPDATE CUSTOMER
                SET FullName = @name, Phone = @phone, Address = @address
                WHERE CustomerID = @id";

            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();

            Db.ExecuteNonQuery(sql, null,
                new SqlParameter("@name", name),
                new SqlParameter("@phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone),
                new SqlParameter("@address", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address),
                new SqlParameter("@id", customerId.Value));

            MessageBox.Show("Customer updated successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
