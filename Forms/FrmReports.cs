using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using NexusGearMS.Helpers;

namespace NexusGearMS.Forms
{
    public partial class FrmReports : Form
    {
        public FrmReports()
        {
            InitializeComponent();
        }

        private void FrmReports_Load(object sender, EventArgs e)
        {
            // Guard: Check permission (Admin only)
            if (!AuthGuard.GuardForm(this, "REPORTS"))
                return;

            dtFrom.Value = DateTime.Now.AddMonths(-1);
            dtTo.Value = DateTime.Now;
            cboReportType.SelectedIndex = 0;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            string reportType = cboReportType.SelectedItem.ToString();

            try
            {
                DataTable dt = null;

                switch (reportType)
                {
                    case "Total Imported Quantity by Product":
                        dt = GetImportedQtyReport();
                        break;
                    case "Revenue by Day":
                        dt = GetRevenueByDayReport();
                        break;
                    case "Revenue by Month":
                        dt = GetRevenueByMonthReport();
                        break;
                    case "Revenue by Year":
                        dt = GetRevenueByYearReport();
                        break;
                    case "Profit by Product":
                        dt = GetProfitByProductReport();
                        break;
                    case "Profit by Employee":
                        dt = GetProfitByEmployeeReport();
                        break;
                }

                gvReport.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetImportedQtyReport()
        {
            string sql = @"
                SELECT p.ProductCode AS [Code], p.ProductName AS [Product Name], 
                       SUM(d.Qty) AS [Total Imported Qty]
                FROM IMPORT_DETAIL d
                JOIN PRODUCT p ON p.ProductID = d.ProductID
                GROUP BY p.ProductCode, p.ProductName
                ORDER BY [Total Imported Qty] DESC";

            return Db.ExecuteDataTable(sql);
        }

        private DataTable GetRevenueByDayReport()
        {
            string sql = @"
                SELECT CONVERT(date, i.InvoiceDate) AS [Date],
                       SUM(d.Qty * d.UnitPrice) AS [Revenue]
                FROM SALES_INVOICE i
                JOIN SALES_DETAIL d ON d.InvoiceID = i.InvoiceID
                WHERE i.Status = 'COMPLETED'
                  AND i.InvoiceDate >= @fromDate AND i.InvoiceDate < DATEADD(day,1,@toDate)
                GROUP BY CONVERT(date, i.InvoiceDate)
                ORDER BY [Date]";

            return Db.ExecuteDataTable(sql,
                new SqlParameter("@fromDate", dtFrom.Value.Date),
                new SqlParameter("@toDate", dtTo.Value.Date));
        }

        private DataTable GetRevenueByMonthReport()
        {
            string sql = @"
                SELECT CAST(YEAR(i.InvoiceDate) AS VARCHAR(4)) + '-' + 
                       RIGHT('0'+CAST(MONTH(i.InvoiceDate) AS VARCHAR(2)),2) AS [Month],
                       SUM(d.Qty * d.UnitPrice) AS [Revenue]
                FROM SALES_INVOICE i
                JOIN SALES_DETAIL d ON d.InvoiceID=i.InvoiceID
                WHERE i.Status='COMPLETED'
                  AND i.InvoiceDate >= @fromDate AND i.InvoiceDate < DATEADD(day,1,@toDate)
                GROUP BY YEAR(i.InvoiceDate), MONTH(i.InvoiceDate)
                ORDER BY YEAR(i.InvoiceDate), MONTH(i.InvoiceDate)";

            return Db.ExecuteDataTable(sql,
                new SqlParameter("@fromDate", dtFrom.Value.Date),
                new SqlParameter("@toDate", dtTo.Value.Date));
        }

        private DataTable GetRevenueByYearReport()
        {
            string sql = @"
                SELECT YEAR(i.InvoiceDate) AS [Year],
                       SUM(d.Qty * d.UnitPrice) AS [Revenue],
                       COUNT(DISTINCT i.InvoiceID) AS [Total Invoices]
                FROM SALES_INVOICE i
                JOIN SALES_DETAIL d ON d.InvoiceID=i.InvoiceID
                WHERE i.Status='COMPLETED'
                GROUP BY YEAR(i.InvoiceDate)
                ORDER BY [Year] DESC";

            return Db.ExecuteDataTable(sql);
        }

        private DataTable GetProfitByProductReport()
        {
            string sql = @"
                SELECT p.ProductCode AS [Code], p.ProductName AS [Product Name],
                       SUM((d.UnitPrice - d.UnitCostAtSale) * d.Qty) AS [Profit]
                FROM SALES_INVOICE i
                JOIN SALES_DETAIL d ON d.InvoiceID=i.InvoiceID
                JOIN PRODUCT p ON p.ProductID=d.ProductID
                WHERE i.Status='COMPLETED'
                  AND i.InvoiceDate >= @fromDate AND i.InvoiceDate < DATEADD(day,1,@toDate)
                GROUP BY p.ProductCode, p.ProductName
                ORDER BY [Profit] DESC";

            return Db.ExecuteDataTable(sql,
                new SqlParameter("@fromDate", dtFrom.Value.Date),
                new SqlParameter("@toDate", dtTo.Value.Date));
        }

        private DataTable GetProfitByEmployeeReport()
        {
            string sql = @"
                SELECT e.EmpCode AS [Emp Code], e.FullName AS [Full Name],
                       SUM((d.UnitPrice - d.UnitCostAtSale) * d.Qty) AS [Profit]
                FROM SALES_INVOICE i
                JOIN SALES_DETAIL d ON d.InvoiceID=i.InvoiceID
                JOIN EMPLOYEE e ON e.EmpID=i.EmpID
                WHERE i.Status='COMPLETED'
                  AND i.InvoiceDate >= @fromDate AND i.InvoiceDate < DATEADD(day,1,@toDate)
                GROUP BY e.EmpCode, e.FullName
                ORDER BY [Profit] DESC";

            return Db.ExecuteDataTable(sql,
                new SqlParameter("@fromDate", dtFrom.Value.Date),
                new SqlParameter("@toDate", dtTo.Value.Date));
        }
    }
}
