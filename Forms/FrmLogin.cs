using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                lblError.Text = "Please enter username.";
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                lblError.Text = "Please enter password.";
                txtPassword.Focus();
                return;
            }

            try
            {
                string sql = @"
                    SELECT a.AccountID, a.Username, a.PasswordHash, a.PasswordSalt, a.MustChangePwd, a.IsActive,
                           e.EmpID, e.EmpCode, e.FullName, e.RoleID, r.RoleName
                    FROM ACCOUNT a
                    JOIN EMPLOYEE e ON e.EmpID = a.EmpID
                    JOIN ROLE r ON r.RoleID = e.RoleID
                    WHERE a.Username = @username AND a.IsActive = 1 AND e.IsActive = 1";

                DataTable dt = Db.ExecuteDataTable(sql, new SqlParameter("@username", txtUsername.Text.Trim()));

                if (dt.Rows.Count == 0)
                {
                    lblError.Text = "Account does not exist or has been disabled.";
                    return;
                }

                DataRow row = dt.Rows[0];

                byte[] storedHash = (byte[])row["PasswordHash"];
                byte[] storedSalt = (byte[])row["PasswordSalt"];

                if (!Security.VerifyPassword(txtPassword.Text, storedSalt, storedHash))
                {
                    lblError.Text = "Incorrect password.";
                    return;
                }

                SessionManager.AccountID = Convert.ToInt32(row["AccountID"]);
                SessionManager.Username = row["Username"].ToString();
                SessionManager.EmpID = Convert.ToInt32(row["EmpID"]);
                SessionManager.EmpCode = row["EmpCode"].ToString();
                SessionManager.FullName = row["FullName"].ToString();
                SessionManager.RoleID = Convert.ToInt32(row["RoleID"]);
                SessionManager.RoleName = row["RoleName"].ToString();

                bool mustChangePwd = Convert.ToBoolean(row["MustChangePwd"]);

                if (mustChangePwd)
                {
                    MessageBox.Show("You must change your password before continuing.", "Notice",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    FrmChangePassword frmChangePwd = new FrmChangePassword();
                    this.Hide();
                    if (frmChangePwd.ShowDialog() == DialogResult.OK)
                    {
                        OpenMainForm();
                    }
                    else
                    {
                        this.Show();
                        SessionManager.Clear();
                    }
                }
                else
                {
                    OpenMainForm();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
        }

        private void OpenMainForm()
        {
            this.Hide();
            FrmMain frmMain = new FrmMain();
            frmMain.FormClosed += (s, args) =>
            {
                // When main form closes, show login again
                this.Show();
                txtPassword.Text = "";
                lblError.Text = "";
            };
            frmMain.Show();
        }
    }
}
