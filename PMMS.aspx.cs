using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace MedicalSystem
{
    public partial class PMMS : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCheckClearance_Click(object sender, EventArgs e)
        {
            // Prepend dropdown value before Employee ID
            string prefix = ddlCompany.SelectedValue.Trim();
            string empId = prefix + txtEmployeeID.Text.Trim();
            int selectedCompanyIndex = ddlCompany.SelectedIndex - 1; // Because first item = "Select Company"

            if (string.IsNullOrEmpty(empId) || selectedCompanyIndex < 0)
            {
                lblStatus.Text = "❌ Please select a company and enter an Employee ID.";
                lblStatus.CssClass = "status-message error";
                lblStatus.Style["display"] = "block";
                return;
            }

            try
            {
                // Reset all connection boxes
                ResetConnections();

                string userCheckQuery = "SELECT ActiveStatus FROM [TAS].[dbo].[Users] WHERE EmployeeID=@EmpID";

                string[] connections = { "PMMS_Conn1", "PMMS_Conn2", "PMMS_Conn3", "PMMS_Conn4", "PMMS_Conn5" };
                System.Web.UI.WebControls.Label[] labels = { conn1, conn2, conn3, conn4, conn5 };

                bool hasActiveAccount = false;
                bool recordFoundAnywhere = false;

                for (int i = 0; i < connections.Length; i++)
                {
                    try
                    {
                        // Get connection string and add 5-second timeout
                        string connStr = ConfigurationManager.ConnectionStrings[connections[i]].ConnectionString;
                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connStr)
                        {
                            ConnectTimeout = 5 // 5 seconds
                        };

                        using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                        using (SqlCommand cmd = new SqlCommand(userCheckQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmpID", empId);
                            cmd.CommandTimeout = 5; // optional, command timeout
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
                                if (status == 1)
                                {
                                    labels[i].CssClass += " green"; // active
                                    hasActiveAccount = true;
                                }
                                else
                                {
                                    labels[i].CssClass += " red"; // inactive
                                }
                            }
                        }
                    }
                    catch
                    {
                        labels[i].CssClass += " gray"; // treat connection failure as record not found
                    }
                }

                // Set overall status message
                if (!recordFoundAnywhere)
                {
                    lblStatus.Text = "⚪ Employee record not found in any PMMS system.";
                    lblStatus.CssClass = "status-message error";
                }
                else if (hasActiveAccount)
                {
                    lblStatus.Text = "⚠️ Employee has pending clearance items in one or more PMMS systems.";
                    lblStatus.CssClass = "status-message error";
                }
                else
                {
                    lblStatus.Text = "✅ Employee exists and has no pending clearance items. All accounts are inactive.";
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

        // Reset connection boxes to default
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
