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

                // Connection strings and labels
                string[] connections = { "MIS_Conn1", "MIS_Conn2", "MIS_Conn3", "MIS_Conn4", "MIS_Conn5" };
                System.Web.UI.WebControls.Label[] labels = { conn1, conn2, conn3, conn4, conn5 };
                string[] companyNames = { "HEISCO", "GULF DREDGING", "HEISCO RESOURCES", "HEISCO KSA", "GULF SKY KSA" };

                // Views for each company
                string[] views = {
            "HEISCO.V_HEISCO_EMPLOYEE_ACCESS",    // HEISCO Kuwait
            "GDCO.V_GDCO_EMPLOYEE_ACCESS",        // Gulf Dredging
            "HTSCO.V_HTSCO_EMPLOYEE_ACCESS",      // HEISCO Resource Kuwait
            "HSA.V_HSA_EMPLOYEE_ACCESS",          // HEISCO KSA
            "GULFSKY.V_GULFSKY_EMPLOYEE_ACCESS"   // GulfSky KSA
        };

                // Query templates
                string queryPrimaryTemplate = "SELECT IS_ACTIVE, EMPNAME, USER_ID FROM OracleLinkedServer..{0} WHERE EMPFNO=@EmpID";
                string queryOthersTemplate = "SELECT IS_ACTIVE, EMPNAME, USER_ID FROM OracleLinkedServer..{0} WHERE EMPFID=@EmpID OR EMPFBP=@EmpID";

                bool inactiveFound = true;
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

                        string query = (i == selectedCompanyIndex)
                            ? string.Format(queryPrimaryTemplate, views[i])
                            : string.Format(queryOthersTemplate, views[i]);

                        using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                        using (SqlCommand cmd = new SqlCommand(query, conn))
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

                                    string status = reader["IS_ACTIVE"].ToString().Trim();
                                    string empName = reader["EMPNAME"].ToString().Trim();
                                    string userId = reader["USER_ID"].ToString().Trim();

                                    // Color logic
                                    if (status == "0") // Inactive
                                    {
                                        labels[i].CssClass += " red";
                                    }
                                    else // Active
                                    {
                                        labels[i].CssClass += " green";
                                        inactiveFound = false;
                                    }

                                    // Show Company, Employee Name and User ID
                                    labels[i].Text = $"{companyNames[i]}: {empName} | User ID: {userId}";
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

                // Overall status
                if (!recordFoundAnywhere)
                {
                    lblStatus.Text = "⚪ Employee record not found in any MIS system.";
                    lblStatus.CssClass = "status-message error";
                }
                else if (!inactiveFound)
                {
                    lblStatus.Text = "⚠️ Employee account is still active in one or more systems.";
                    lblStatus.CssClass = "status-message error";
                }
                else
                {
                    lblStatus.Text = "✅ Employee exists and all accounts are inactive.";
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
