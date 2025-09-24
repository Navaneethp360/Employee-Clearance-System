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
            string emp_num = txtEmployeeID.Text.Trim();

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

                // ActiveStatus from Users
                string userCheckQuery = "SELECT ActiveStatus FROM [Users] WHERE EmployeeID=@EmpID";

                // EmployeeName from TAS_EMP_INFO
                string empNameQuery = "SELECT EMPLOYEENAME FROM [TAS_EMP_INFO] WHERE EMPLOYEEID=@EmpID";

                string[] connections = { "PMMS_Conn1", "PMMS_Conn2", "PMMS_Conn3", "PMMS_Conn4", "PMMS_Conn5" };
                System.Web.UI.WebControls.Label[] labels = { conn1, conn2, conn3, conn4, conn5 };
                string[] companyNames = { "HEISCO", "GULF DREDGING", "HEISCO RESOURCES", "HEISCO KSA", "GULF SKY KSA" };

                bool hasActiveAccount = false;
                bool recordFoundAnywhere = false;

                // 1) Get Employee Name only from selected company connection
                string empName = "Unknown";
                try
                {
                    string selectedConnStr = ConfigurationManager.ConnectionStrings[connections[selectedCompanyIndex]].ConnectionString;
                    using (SqlConnection conn = new SqlConnection(selectedConnStr))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(empNameQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmpID", emp_num);
                            object result = cmd.ExecuteScalar();
                            if (result != null) empName = result.ToString().Trim();
                        }
                    }
                }
                catch
                {
                    empName = "Unknown";
                }

                // 2) Now check ActiveStatus in all connections using the same empId
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
                        {
                            conn.Open();

                            int? status = null;
                            using (SqlCommand cmd = new SqlCommand(userCheckQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@EmpID", empId);
                                object result = cmd.ExecuteScalar();
                                if (result != null) status = Convert.ToInt32(result);
                            }

                            if (status == null)
                            {
                                labels[i].CssClass += " gray";
                                labels[i].Text = $"{companyNames[i]}: No record found";
                            }
                            else
                            {
                                recordFoundAnywhere = true;

                                if (status == 1) // Active
                                {
                                    labels[i].CssClass += " green";
                                    hasActiveAccount = true;
                                }
                                else // Inactive
                                {
                                    labels[i].CssClass += " red";
                                }

                                labels[i].Text = $"{companyNames[i]}: {empName}";
                            }
                        }
                    }
                    catch
                    {
                        labels[i].CssClass += " gray";
                        labels[i].Text = $"{companyNames[i]}: Connection failed";
                    }
                }

                // 3) Overall status message
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
