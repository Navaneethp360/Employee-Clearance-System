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
                conn1.CssClass = "connection-box";
                conn2.CssClass = "connection-box";
                conn3.CssClass = "connection-box";
                conn4.CssClass = "connection-box";
                conn5.CssClass = "connection-box";

                // Example: check each system
                conn1.CssClass += CheckSystem1(empId) ? " green" : " red";
                conn2.CssClass += CheckSystem2(empId) ? " green" : " red";
                conn3.CssClass += CheckSystem3(empId) ? " green" : " red";
                conn4.CssClass += CheckSystem4(empId) ? " green" : " red";
                conn5.CssClass += CheckSystem5(empId) ? " green" : " red";

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

        // Sample system check methods
        private bool CheckSystem1(string empId)
        {
            // Example query - replace with your actual check
            string query = "SELECT COUNT(*) FROM Leaves WHERE EmployeeID=@EmpID AND IsPending=1";
            return ExecuteQueryCheck(query, empId) == 0; // green if no pending leaves
        }

        private bool CheckSystem2(string empId)
        {
            string query = "SELECT COUNT(*) FROM Assets WHERE EmployeeID=@EmpID AND IsReturned=0";
            return ExecuteQueryCheck(query, empId) == 0;
        }

        private bool CheckSystem3(string empId)
        {
            string query = "SELECT COUNT(*) FROM Trainings WHERE EmployeeID=@EmpID AND IsCompleted=0";
            return ExecuteQueryCheck(query, empId) == 0;
        }

        private bool CheckSystem4(string empId)
        {
            string query = "SELECT COUNT(*) FROM Payroll WHERE EmployeeID=@EmpID AND IsSettled=0";
            return ExecuteQueryCheck(query, empId) == 0;
        }

        private bool CheckSystem5(string empId)
        {
            string query = "SELECT COUNT(*) FROM MedicalRecords WHERE EmployeeID=@EmpID AND IsPendingClearance=1";
            return ExecuteQueryCheck(query, empId) == 0;
        }

        // Executes a scalar query and returns the count
        private int ExecuteQueryCheck(string query, string empId)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmpID", empId);
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }
}
