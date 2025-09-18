using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace MedicalSystem
{
    public partial class NONERP : Page
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
                // Reset all connection boxes
                ResetConnections();

                string[] connections = { "NONERP_Conn1", "NONERP_Conn2", "NONERP_Conn3", "NONERP_Conn4", "NONERP_Conn5" };
                string[] queries = {
                    "SELECT IsPending FROM Leaves WHERE EmployeeID=@EmpID",
                    "SELECT IsReturned FROM Assets WHERE EmployeeID=@EmpID",
                    "SELECT IsCompleted FROM Trainings WHERE EmployeeID=@EmpID",
                    "SELECT IsSettled FROM Payroll WHERE EmployeeID=@EmpID",
                    "SELECT IsPendingClearance FROM MedicalRecords WHERE EmployeeID=@EmpID"
                };

                System.Web.UI.WebControls.Label[] labels = { conn1, conn2, conn3, conn4, conn5 };

                bool hasPendingItems = false;
                bool recordFoundAnywhere = false;

                for (int i = 0; i < connections.Length; i++)
                {
                    try
                    {
                        // Add 5-second timeout
                        string connStr = ConfigurationManager.ConnectionStrings[connections[i]].ConnectionString;
                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connStr)
                        {
                            ConnectTimeout = 5
                        };

                        using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                        using (SqlCommand cmd = new SqlCommand(queries[i], conn))
                        {
                            cmd.Parameters.AddWithValue("@EmpID", empId);
                            cmd.CommandTimeout = 5; // query timeout
                            conn.Open();

                            object result = cmd.ExecuteScalar();
                            if (result == null)
                            {
                                labels[i].CssClass += " gray"; // record not found
                            }
                            else
                            {
                                recordFoundAnywhere = true;
                                int status = Convert.ToInt32(result);

                                // For each query: 0 means cleared, 1 means pending
                                if (status == 1)
                                {
                                    labels[i].CssClass += " red";
                                    hasPendingItems = true;
                                }
                                else
                                {
                                    labels[i].CssClass += " green";
                                }
                            }
                        }
                    }
                    catch
                    {
                        labels[i].CssClass += " gray"; // timeout or failure → gray
                    }
                }

                // Overall status message
                if (!recordFoundAnywhere)
                {
                    lblStatus.Text = "⚪ Employee record not found in any NONERP system.";
                    lblStatus.CssClass = "status-message error";
                }
                else if (hasPendingItems)
                {
                    lblStatus.Text = "⚠️ Employee has pending clearance items in one or more NONERP systems.";
                    lblStatus.CssClass = "status-message error";
                }
                else
                {
                    lblStatus.Text = "✅ Employee exists and has no pending clearance items.";
                    lblStatus.CssClass = "status-message success";
                }

                lblStatus.Style["display"] = "block";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Error occurred: " + ex.Message;
                lblStatus.CssClass = "status-message error";
                lblStatus.Style["display"] = "block";
            }
        }

        // Reset all boxes before checking
        private void ResetConnections()
        {
            conn1.CssClass = "connection-box";
            conn2.CssClass = "connection-box";
            conn3.CssClass = "connection-box";
            conn4.CssClass = "connection-box";
            conn5.CssClass = "connection-box";
        }
    }
}
