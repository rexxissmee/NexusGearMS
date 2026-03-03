using System;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {SessionManager.FullName}";
            lblRole.Text = $"Role: {SessionManager.RoleName}";

            ApplyRoleBasedAccess();
            SetupButtonHoverEffects();
        }

        private void SetupButtonHoverEffects()
        {
            Button[] buttons = { btnProducts, btnCategories, btnCustomers, btnSales, btnImport, btnSuppliers, btnEmployees, btnReports };

            foreach (Button btn in buttons)
            {
                btn.MouseEnter += (s, e) =>
                {
                    if (btn.Visible)
                    {
                        btn.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
                        btn.Cursor = System.Windows.Forms.Cursors.Hand;
                    }
                };

                btn.MouseLeave += (s, e) =>
                {
                    btn.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
                };
            }

            btnLogout.MouseEnter += (s, e) =>
            {
                btnLogout.BackColor = System.Drawing.Color.FromArgb(200, 35, 51);
                btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            };

            btnLogout.MouseLeave += (s, e) =>
            {
                btnLogout.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            };
        }

        private void ApplyRoleBasedAccess()
        {
            string role = SessionManager.RoleName;

            // Admin has full access
            if (role == "Admin")
            {
                btnProducts.Visible = true;
                btnCategories.Visible = true;  // Admin only
                btnCustomers.Visible = true;
                btnSales.Visible = true;
                btnImport.Visible = true;
                btnSuppliers.Visible = true;   // Admin and Warehouse
                btnEmployees.Visible = true;   // Admin only
                btnReports.Visible = true;     // Admin only
            }
            // Sales can access products, customers, and sales
            else if (role == "Sales")
            {
                btnProducts.Visible = true;
                btnCategories.Visible = false;
                btnCustomers.Visible = true;
                btnSales.Visible = true;
                btnImport.Visible = false;
                btnSuppliers.Visible = false;
                btnEmployees.Visible = false;
                btnReports.Visible = false;
            }
            // Warehouse can access products and import
            else if (role == "Warehouse")
            {
                btnProducts.Visible = true;
                btnCategories.Visible = false;
                btnCustomers.Visible = false;
                btnSales.Visible = false;
                btnImport.Visible = true;
                btnSuppliers.Visible = true;   // Warehouse can manage suppliers
                btnEmployees.Visible = false;
                btnReports.Visible = false;
            }
            // Default: hide all
            else
            {
                btnProducts.Visible = false;
                btnCategories.Visible = false;
                btnCustomers.Visible = false;
                btnSales.Visible = false;
                btnImport.Visible = false;
                btnSuppliers.Visible = false;
                btnEmployees.Visible = false;
                btnReports.Visible = false;
            }

            // Rearrange buttons to remove gaps
            RearrangeMenuButtons();
        }

        private void RearrangeMenuButtons()
        {
            int topPosition = 20;
            int buttonHeight = 50;
            int spacing = 10;

            Button[] buttons = { btnProducts, btnCategories, btnCustomers, btnSales, btnImport, btnSuppliers, btnEmployees, btnReports };

            foreach (Button btn in buttons)
            {
                if (btn.Visible)
                {
                    btn.Top = topPosition;
                    topPosition += buttonHeight + spacing;
                }
            }
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new FrmProducts());
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new FrmCategories());
        }

        private void btnCustomers_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new FrmCustomers());
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new FrmInvoiceList());
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new FrmImportList());
        }

        private void btnSuppliers_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new FrmSuppliers());
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new FrmReports());
        }

        private void btnEmployees_Click(object sender, EventArgs e)
        {
            LoadFormInPanel(new FrmEmployees());
        }

        private void LoadFormInPanel(Form form)
        {
            pnlContent.Controls.Clear();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(form);
            form.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                SessionManager.Clear();
                this.Close();
            }
        }
    }
}
