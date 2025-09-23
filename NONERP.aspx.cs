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
            // Prepend dropdown value with '-' before Employee ID
            string prefix = ddlCompany.SelectedValue.Trim();
            string empId = prefix + "-" + txtEmployeeID.Text.Trim();
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

                string[] connections = { "NONERP_Conn1", "NONERP_Conn2", "NONERP_Conn3" };
                string[] queries = {
                    "SELECT ActiveStatus FROM [SAL].[dbo].[UserMaster] WHERE Remark=@EmpID",
                    "SELECT ActiveStatus FROM [OSR].[dbo].[UserMaster] WHERE Remark=@EmpID",
                    "SELECT ActiveStatus FROM [TRF_NEW].[dbo].[UserMaster] WHERE Remark=@EmpID"
                };

                System.Web.UI.WebControls.Label[] labels = { conn1, conn2, conn3 };

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

                                // For each query: 0 = cleared, 1 = pending
                                if (status == 1)
                                {
                                    labels[i].CssClass += " green";
                                    hasPendingItems = true;
                                }
                                else
                                {
                                    labels[i].CssClass += " red";
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
                    lblStatus.Text = "⚪ Employee record not found in any NON-ERP system.";
                    lblStatus.CssClass = "status-message error";
                }
                else if (hasPendingItems)
                {
                    lblStatus.Text = "⚠️ Employee has pending clearance items in one or more NON-ERP systems.";
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

        // Reset all boxes before checking
        private void ResetConnections()
        {
            conn1.CssClass = "connection-box";
            conn2.CssClass = "connection-box";
            conn3.CssClass = "connection-box";
        }
    }
}
