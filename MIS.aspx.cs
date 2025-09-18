using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace MedicalSystem
{
    public partial class EmployeeClearance : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCheckClearance_Click(object sender, EventArgs e)
        {
            string empId = txtEmployeeID.Text.Trim();
            if (string.IsNullOrEmpty(empId))
            {
                lblStatus.Text = "❌ Please enter an Employee ID.";
                lblStatus.CssClass = "status-message error";
                lblStatus.Style["display"] = "block";
                return;
            }

            try
            {
                // Reset connection boxes
                ResetConnections();

                // Check each system with its own connection string
                conn1.CssClass += CheckSystem(empId, "MIS_Conn1", "SELECT COUNT(*) FROM Leaves WHERE EmployeeID=@EmpID AND IsPending=1") ? " green" : " red";
                conn2.CssClass += CheckSystem(empId, "MIS_Conn2", "SELECT COUNT(*) FROM Assets WHERE EmployeeID=@EmpID AND IsReturned=0") ? " green" : " red";
                conn3.CssClass += CheckSystem(empId, "MIS_Conn3", "SELECT COUNT(*) FROM Trainings WHERE EmployeeID=@EmpID AND IsCompleted=0") ? " green" : " red";
                conn4.CssClass += CheckSystem(empId, "MIS_Conn4", "SELECT COUNT(*) FROM Payroll WHERE EmployeeID=@EmpID AND IsSettled=0") ? " green" : " red";
                conn5.CssClass += CheckSystem(empId, "MIS_Conn5", "SELECT COUNT(*) FROM MedicalRecords WHERE EmployeeID=@EmpID AND IsPendingClearance=1") ? " green" : " red";

                lblStatus.Text = "✅ Clearance check completed!";
                lblStatus.CssClass = "status-message";
                lblStatus.Style["display"] = "block";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Error occurred: " + ex.Message;
                lblStatus.CssClass = "status-message error";
                lblStatus.Style["display"] = "block";
            }
        }

        // Reset connection boxes to default
        private void ResetConnections()
        {
            conn1.CssClass = "connection-box";
            conn2.CssClass = "connection-box";
            conn3.CssClass = "connection-box";
            conn4.CssClass = "connection-box";
            conn5.CssClass = "connection-box";
        }

        // Generic method to check any system with its connection string and query
        private bool CheckSystem(string empId, string connStrName, string query)
        {
            string connStr = ConfigurationManager.ConnectionStrings[connStrName].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmpID", empId);
                conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count == 0; // green if no pending items
            }
        }
    }
}
