using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Common;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmEmployees : Form
    {
        private int? selectedEmpID = null;

        public FrmEmployees()
        {
            InitializeComponent();
        }

        private void FrmEmployees_Load(object sender, EventArgs e)
        {
            // Guard: Check permission (Admin only)
            if (!AuthGuard.GuardForm(this, "EMPLOYEES"))
                return;

            LoadRoles();
            LoadEmployees();
        }

        private void LoadRoles()
        {
            try
            {
                // ROLE table doesn't have IsActive column
                string sql = "SELECT RoleID, RoleName FROM ROLE ORDER BY RoleName";
                DataTable dt = Db.ExecuteDataTable(sql);

                cbRole.DataSource = dt;
                cbRole.DisplayMember = "RoleName";
                cbRole.ValueMember = "RoleID";
                cbRole.SelectedIndex = -1;

                cbFilterRole.DataSource = dt.Copy();
                cbFilterRole.DisplayMember = "RoleName";
                cbFilterRole.ValueMember = "RoleID";
                cbFilterRole.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading roles: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEmployees()
        {
            try
            {
                string sql = @"
                    SELECT e.EmpID, e.EmpCode, e.FullName, e.Phone, 
                           e.Position, r.RoleName, e.IsActive,
                           a.Username
                    FROM EMPLOYEE e
                    JOIN ROLE r ON r.RoleID = e.RoleID
                    LEFT JOIN ACCOUNT a ON a.EmpID = e.EmpID
                    WHERE (@keyword IS NULL OR e.EmpCode LIKE @keyword OR e.FullName LIKE @keyword OR e.Phone LIKE @keyword)
                      AND (@roleId IS NULL OR e.RoleID = @roleId)
                    ORDER BY e.EmpCode";

                string keyword = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : "%" + txtSearch.Text.Trim() + "%";
                int? roleId = cbFilterRole.SelectedValue == null ? null : (int?)cbFilterRole.SelectedValue;

                DataTable dt = Db.ExecuteDataTable(sql,
                    new SqlParameter("@keyword", (object)keyword ?? DBNull.Value),
                    new SqlParameter("@roleId", (object)roleId ?? DBNull.Value));

                dgvEmployees.DataSource = dt;

                // Format columns
                if (dgvEmployees.Columns.Contains("EmpID"))
                    dgvEmployees.Columns["EmpID"].Visible = false;

                dgvEmployees.Columns["EmpCode"].HeaderText = "Employee Code";
                dgvEmployees.Columns["FullName"].HeaderText = "Full Name";
                dgvEmployees.Columns["Phone"].HeaderText = "Phone";
                dgvEmployees.Columns["Position"].HeaderText = "Position";
                dgvEmployees.Columns["RoleName"].HeaderText = "Role";
                dgvEmployees.Columns["IsActive"].HeaderText = "Active";
                dgvEmployees.Columns["Username"].HeaderText = "Username";
                dgvEmployees.Columns["Username"].ReadOnly = false;

                lblRecordCount.Text = $"Total: {dt.Rows.Count} employee(s)";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employees: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cbFilterRole.SelectedIndex = -1;
            LoadEmployees();
            ClearForm();
        }

        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow != null && dgvEmployees.CurrentRow.Index >= 0)
            {
                DataGridViewRow row = dgvEmployees.CurrentRow;
                selectedEmpID = Convert.ToInt32(row.Cells["EmpID"].Value);

                txtEmpCode.Text = row.Cells["EmpCode"].Value.ToString();
                txtFullName.Text = row.Cells["FullName"].Value.ToString();
                txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? "";
                txtPosition.Text = row.Cells["Position"].Value?.ToString() ?? "";

                string roleName = row.Cells["RoleName"].Value.ToString();
                foreach (DataRowView item in cbRole.Items)
                {
                    if (item["RoleName"].ToString() == roleName)
                    {
                        cbRole.SelectedValue = item["RoleID"];
                        break;
                    }
                }

                chkActive.Checked = Convert.ToBoolean(row.Cells["IsActive"].Value);

                // Enable/disable buttons based on whether account exists
                bool hasAccount = row.Cells["Username"].Value != null && row.Cells["Username"].Value != DBNull.Value && !string.IsNullOrEmpty(row.Cells["Username"].Value.ToString());
                btnCreateAccount.Enabled = !hasAccount;
                btnResetPassword.Enabled = hasAccount;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                // Check duplicate EmpCode
                string checkSql = "SELECT COUNT(*) FROM EMPLOYEE WHERE EmpCode = @code";
                int count = Convert.ToInt32(Db.ExecuteScalar(checkSql, null, new SqlParameter("@code", txtEmpCode.Text.Trim())));

                if (count > 0)
                {
                    MessageBox.Show("Employee code already exists!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // EMPLOYEE table doesn't have Email column - removed from INSERT
                string sql = @"
                    INSERT INTO EMPLOYEE (EmpCode, FullName, Phone, Position, RoleID, IsActive)
                    VALUES (@code, @name, @phone, @position, @roleId, @active)";

                Db.ExecuteNonQuery(sql, null,
                    new SqlParameter("@code", txtEmpCode.Text.Trim().ToUpper()),
                    new SqlParameter("@name", txtFullName.Text.Trim()),
                    new SqlParameter("@phone", string.IsNullOrWhiteSpace(txtPhone.Text) ? (object)DBNull.Value : txtPhone.Text.Trim()),
                    new SqlParameter("@position", string.IsNullOrWhiteSpace(txtPosition.Text) ? (object)DBNull.Value : txtPosition.Text.Trim()),
                    new SqlParameter("@roleId", cbRole.SelectedValue),
                    new SqlParameter("@active", chkActive.Checked));

                MessageBox.Show("Employee added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEmployees();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding employee: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedEmpID == null)
            {
                MessageBox.Show("Please select an employee to update.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateInputs())
                return;

            try
            {
                // Check duplicate EmpCode (except current)
                string checkSql = "SELECT COUNT(*) FROM EMPLOYEE WHERE EmpCode = @code AND EmpID <> @id";
                int count = Convert.ToInt32(Db.ExecuteScalar(checkSql, null,
                    new SqlParameter("@code", txtEmpCode.Text.Trim()),
                    new SqlParameter("@id", selectedEmpID)));

                if (count > 0)
                {
                    MessageBox.Show("Employee code already exists!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // EMPLOYEE table doesn't have Email column - removed from UPDATE
                string sql = @"
                    UPDATE EMPLOYEE
                    SET EmpCode = @code, FullName = @name, Phone = @phone,
                        Position = @position, RoleID = @roleId, IsActive = @active
                    WHERE EmpID = @id";

                Db.ExecuteNonQuery(sql, null,
                    new SqlParameter("@code", txtEmpCode.Text.Trim().ToUpper()),
                    new SqlParameter("@name", txtFullName.Text.Trim()),
                    new SqlParameter("@phone", string.IsNullOrWhiteSpace(txtPhone.Text) ? (object)DBNull.Value : txtPhone.Text.Trim()),
                    new SqlParameter("@position", string.IsNullOrWhiteSpace(txtPosition.Text) ? (object)DBNull.Value : txtPosition.Text.Trim()),
                    new SqlParameter("@roleId", cbRole.SelectedValue),
                    new SqlParameter("@active", chkActive.Checked),
                    new SqlParameter("@id", selectedEmpID));

                MessageBox.Show("Employee updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating employee: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedEmpID == null)
            {
                MessageBox.Show("Please select an employee to delete.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to disable this employee? This will also disable their account.",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (var conn = Db.GetConnection())
                    {
                        conn.Open();
                        using (var tx = conn.BeginTransaction())
                        {
                            try
                            {
                                // Disable employee
                                string sql1 = "UPDATE EMPLOYEE SET IsActive = 0 WHERE EmpID = @id";
                                Db.ExecuteNonQuery(sql1, tx, new SqlParameter("@id", selectedEmpID));

                                // Disable account if exists
                                string sql2 = "UPDATE ACCOUNT SET IsActive = 0 WHERE EmpID = @id";
                                Db.ExecuteNonQuery(sql2, tx, new SqlParameter("@id", selectedEmpID));

                                tx.Commit();
                                MessageBox.Show("Employee disabled successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                LoadEmployees();
                                ClearForm();
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
                    MessageBox.Show("Error disabling employee: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if (selectedEmpID == null)
            {
                MessageBox.Show("Please select an employee first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if employee already has account
            string checkSql = "SELECT COUNT(*) FROM ACCOUNT WHERE EmpID = @id";
            int count = Convert.ToInt32(Db.ExecuteScalar(checkSql, null, new SqlParameter("@id", selectedEmpID)));

            if (count > 0)
            {
                MessageBox.Show("This employee already has an account!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Prompt for username
            string username = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter username for this employee:",
                "Create Account",
                txtEmpCode.Text.Trim().ToLower(),
                -1, -1);

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Username cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check duplicate username
            string checkUserSql = "SELECT COUNT(*) FROM ACCOUNT WHERE Username = @user";
            int userCount = Convert.ToInt32(Db.ExecuteScalar(checkUserSql, null, new SqlParameter("@user", username)));

            if (userCount > 0)
            {
                MessageBox.Show("Username already exists! Please choose another.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Generate default password: "Pass@1234"
                string defaultPassword = "Pass@1234";
                byte[] salt = Security.GenerateSalt();
                byte[] hash = Security.ComputeHash(defaultPassword, salt);

                string sql = @"
                    INSERT INTO ACCOUNT (Username, PasswordHash, PasswordSalt, EmpID, MustChangePwd, IsActive)
                    VALUES (@user, @hash, @salt, @empId, 1, 1)";

                Db.ExecuteNonQuery(sql, null,
                    new SqlParameter("@user", username.Trim()),
                    new SqlParameter("@hash", hash),
                    new SqlParameter("@salt", salt),
                    new SqlParameter("@empId", selectedEmpID));

                MessageBox.Show(
                    $"Account created successfully!\n\nUsername: {username}\nDefault Password: {defaultPassword}\n\nUser must change password on first login.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            if (selectedEmpID == null)
            {
                MessageBox.Show("Please select an employee first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check if employee has account
            string checkSql = "SELECT COUNT(*) FROM ACCOUNT WHERE EmpID = @id";
            int count = Convert.ToInt32(Db.ExecuteScalar(checkSql, null, new SqlParameter("@id", selectedEmpID)));

            if (count == 0)
            {
                MessageBox.Show("This employee doesn't have an account yet!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to reset password for {txtFullName.Text}?\n\n" +
                "The password will be reset to: Pass@1234\n" +
                "User must change password on next login.",
                "Confirm Reset Password",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // Generate new password
                    string defaultPassword = "Pass@1234";
                    byte[] salt = Security.GenerateSalt();
                    byte[] hash = Security.ComputeHash(defaultPassword, salt);

                    string sql = @"
                        UPDATE ACCOUNT
                        SET PasswordHash = @hash, PasswordSalt = @salt, MustChangePwd = 1
                        WHERE EmpID = @empId";

                    Db.ExecuteNonQuery(sql, null,
                        new SqlParameter("@hash", hash),
                        new SqlParameter("@salt", salt),
                        new SqlParameter("@empId", selectedEmpID));

                    MessageBox.Show(
                        $"Password reset successfully!\n\nNew Password: {defaultPassword}\n\nUser must change password on next login.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error resetting password: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            selectedEmpID = null;
            txtEmpCode.Clear();
            txtFullName.Clear();
            txtPhone.Clear();
            txtPosition.Clear();
            cbRole.SelectedIndex = -1;
            chkActive.Checked = true;
            btnCreateAccount.Enabled = false;
            btnResetPassword.Enabled = false;

            if (dgvEmployees.Rows.Count > 0)
                dgvEmployees.ClearSelection();
        }

        private bool ValidateInputs()
        {
            if (!Validator.IsRequired(txtEmpCode.Text))
            {
                MessageBox.Show("Employee code is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmpCode.Focus();
                return false;
            }

            if (!Validator.IsCode(txtEmpCode.Text.Trim()))
            {
                MessageBox.Show("Employee code must be 3-20 characters (A-Z, 0-9, dash only).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmpCode.Focus();
                return false;
            }

            if (!Validator.IsRequired(txtFullName.Text))
            {
                MessageBox.Show("Full name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(txtPhone.Text) && !Validator.IsPhone(txtPhone.Text))
            {
                MessageBox.Show("Invalid phone number format (9-15 digits).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus();
                return false;
            }

            // Email validation removed - EMPLOYEE table doesn't have Email column

            if (cbRole.SelectedValue == null)
            {
                MessageBox.Show("Please select a role.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbRole.Focus();
                return false;
            }

            return true;
        }

        private void dgvEmployees_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvEmployees.Columns[e.ColumnIndex].Name != "Username")
                return;

            try
            {
                DataGridViewRow row = dgvEmployees.Rows[e.RowIndex];
                int empId = Convert.ToInt32(row.Cells["EmpID"].Value);
                string newUsername = row.Cells["Username"].Value?.ToString()?.Trim();

                // Check if employee has an account
                string checkSql = "SELECT AccountID FROM ACCOUNT WHERE EmpID = @id";
                object accountIdObj = Db.ExecuteScalar(checkSql, null, new SqlParameter("@id", empId));

                if (accountIdObj == null || accountIdObj == DBNull.Value)
                {
                    MessageBox.Show("This employee doesn't have an account yet. Username cannot be set.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadEmployees();
                    return;
                }

                if (string.IsNullOrWhiteSpace(newUsername))
                {
                    MessageBox.Show("Username cannot be empty.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadEmployees();
                    return;
                }

                // Check duplicate username (excluding current account)
                int accountId = Convert.ToInt32(accountIdObj);
                string checkUserSql = "SELECT COUNT(*) FROM ACCOUNT WHERE Username = @user AND AccountID != @accId";
                int userCount = Convert.ToInt32(Db.ExecuteScalar(checkUserSql, null,
                    new SqlParameter("@user", newUsername),
                    new SqlParameter("@accId", accountId)));

                if (userCount > 0)
                {
                    MessageBox.Show("This username is already taken.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadEmployees();
                    return;
                }

                // Update username
                string updateSql = "UPDATE ACCOUNT SET Username = @user WHERE AccountID = @accId";
                Db.ExecuteNonQuery(updateSql, null,
                    new SqlParameter("@user", newUsername),
                    new SqlParameter("@accId", accountId));

                MessageBox.Show("Username updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating username: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LoadEmployees();
            }
        }
    }
}
