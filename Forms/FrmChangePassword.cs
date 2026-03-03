using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Common;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmChangePassword : Form
    {
        public FrmChangePassword()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (string.IsNullOrWhiteSpace(txtOldPwd.Text))
            {
                lblError.Text = "Please enter old password.";
                txtOldPwd.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNewPwd.Text))
            {
                lblError.Text = "Please enter new password.";
                txtNewPwd.Focus();
                return;
            }

            if (!Validator.IsValidPassword(txtNewPwd.Text))
            {
                lblError.Text = "New password must be at least 8 characters.";
                txtNewPwd.Focus();
                return;
            }

            if (txtNewPwd.Text != txtConfirmPwd.Text)
            {
                lblError.Text = "Confirmation password does not match.";
                txtConfirmPwd.Focus();
                return;
            }

            try
            {
                string sql = "SELECT PasswordHash, PasswordSalt FROM ACCOUNT WHERE AccountID = @accountId";
                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@accountId", SessionManager.AccountID));

                if (dt.Rows.Count == 0)
                {
                    lblError.Text = "Account not found.";
                    return;
                }

                byte[] storedHash = (byte[])dt.Rows[0]["PasswordHash"];
                byte[] storedSalt = (byte[])dt.Rows[0]["PasswordSalt"];

                if (!Security.VerifyPassword(txtOldPwd.Text, storedSalt, storedHash))
                {
                    lblError.Text = "Old password is incorrect.";
                    return;
                }

                byte[] newSalt = Security.GenerateSalt();
                byte[] newHash = Security.ComputeHash(txtNewPwd.Text, newSalt);

                string updateSql = @"
                    UPDATE ACCOUNT
                    SET PasswordSalt = @salt,
                        PasswordHash = @hash,
                        MustChangePwd = 0
                    WHERE AccountID = @accountId";

                int result = Db.ExecuteNonQuery(updateSql,
                    null,
                    new SqlParameter("@salt", newSalt),
                    new SqlParameter("@hash", newHash),
                    new SqlParameter("@accountId", SessionManager.AccountID));

                if (result > 0)
                {
                    MessageBox.Show("Password changed successfully!", "Notice",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    lblError.Text = "An error occurred while updating password.";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
