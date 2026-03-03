using System;
using System.Windows.Forms;

namespace NexusGearMS.Helpers
{
    public static class AuthGuard
    {
        /// <summary>
        /// Check if current user has permission to access a specific feature
        /// </summary>
        public static bool HasPermission(string feature)
        {
            string role = SessionManager.RoleName;

            if (string.IsNullOrEmpty(role))
                return false;

            switch (feature.ToUpper())
            {
                case "PRODUCTS":
                    return role == "Admin" || role == "Sales" || role == "Warehouse";

                case "CATEGORIES":
                    return role == "Admin";

                case "CUSTOMERS":
                    return role == "Admin" || role == "Sales";

                case "INVOICES":
                    return role == "Admin" || role == "Sales";

                case "IMPORTS":
                    return role == "Admin" || role == "Warehouse";

                case "SUPPLIERS":
                    return role == "Admin" || role == "Warehouse";

                case "EMPLOYEES":
                    return role == "Admin";

                case "REPORTS":
                    return role == "Admin";

                case "PRODUCT_IMAGES":
                    return role == "Admin" || role == "Sales" || role == "Warehouse";

                default:
                    return false;
            }
        }

        /// <summary>
        /// Guard a form by checking if user has permission to access it
        /// If no permission, close the form and show a message
        /// </summary>
        public static bool GuardForm(Form form, string feature)
        {
            if (!HasPermission(feature))
            {
                MessageBox.Show(
                    $"You do not have permission to access {feature}.",
                    "Access Denied",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                form.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if user is Admin
        /// </summary>
        public static bool IsAdmin()
        {
            return SessionManager.RoleName == "Admin";
        }

        /// <summary>
        /// Check if user has specific role
        /// </summary>
        public static bool HasRole(string roleName)
        {
            return SessionManager.RoleName == roleName;
        }

        /// <summary>
        /// Check if user can manage (add/edit/delete) a specific feature
        /// Warehouse role can only view, not manage
        /// </summary>
        public static bool CanManage(string feature)
        {
            string role = SessionManager.RoleName;

            if (string.IsNullOrEmpty(role))
                return false;

            switch (feature.ToUpper())
            {
                case "PRODUCTS":
                    return role == "Admin" || role == "Sales";

                case "CATEGORIES":
                    return role == "Admin";

                case "CUSTOMERS":
                    return role == "Admin" || role == "Sales";

                case "SUPPLIERS":
                    return role == "Admin" || role == "Warehouse";

                case "INVOICES":
                    return role == "Admin" || role == "Sales";

                case "IMPORTS":
                    return role == "Admin" || role == "Warehouse";

                case "EMPLOYEES":
                    return role == "Admin";

                case "PRODUCT_IMAGES":
                    return role == "Admin" || role == "Sales";

                default:
                    return false;
            }
        }

        /// <summary>
        /// Check if user can only view (read-only access)
        /// </summary>
        public static bool IsReadOnly(string feature)
        {
            return HasPermission(feature) && !CanManage(feature);
        }
    }
}
