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
            int selectedCompanyIndex = ddlCompany.SelectedIndex - 1; // first item = "Select Company"

            if (string.IsNullOrEmpty(empId) || selectedCompanyIndex < 0)
            {
                lblStatus.Text = "❌ Please select a company and enter an Employee ID.";
                lblStatus.CssClass = "status-message error";
                lblStatus.Style["display"] = "block";
                return;
            }

            try
            {
                ResetConnections();

                // Query to get ActiveStatus and EmployeeName
                string userCheckQuery = "SELECT ActiveStatus, EmployeeName FROM [TAS].[dbo].[Users] WHERE EmployeeID=@EmpID";

                string[] connections = { "PMMS_Conn1", "PMMS_Conn2", "PMMS_Conn3", "PMMS_Conn4", "PMMS_Conn5" };
                System.Web.UI.WebControls.Label[] labels = { conn1, conn2, conn3, conn4, conn5 };
                string[] companyNames = { "HEISCO", "GULF DREDGING", "HEISCO RESOURCES", "HEISCO KSA", "GULF SKY KSA" };

                bool hasActiveAccount = false;
                bool recordFoundAnywhere = false;

                for (int i = 0; i < connections.Length; i++)
                {
                    try
                    {
                        string connStr = ConfigurationManager.ConnectionStrings[connections[i]].ConnectionString;
                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connStr)
                        {
                            ConnectTimeout = 5
                        };

                        using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                        using (SqlCommand cmd = new SqlCommand(userCheckQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmpID", empId);
                            cmd.CommandTimeout = 5;
                            conn.Open();

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (!reader.HasRows)
                                {
                                    labels[i].CssClass += " gray";
                                    labels[i].Text = $"{companyNames[i]}: No record found";
                                }
                                else
                                {
                                    recordFoundAnywhere = true;
                                    reader.Read();

                                    int status = Convert.ToInt32(reader["ActiveStatus"]);
                                    string empName = reader["EmployeeName"].ToString().Trim();

                                    // Set color based on ActiveStatus
                                    if (status == 1) // Active
                                    {
                                        labels[i].CssClass += " green";
                                        hasActiveAccount = true;
                                    }
                                    else // Inactive
                                    {
                                        labels[i].CssClass += " red";
                                    }

                                    // Show Company + Employee Name
                                    labels[i].Text = $"{companyNames[i]}: {empName}";
                                }
                            }
                        }
                    }
                    catch
                    {
                        labels[i].CssClass += " gray";
                        labels[i].Text = $"{companyNames[i]}: Connection failed";
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
