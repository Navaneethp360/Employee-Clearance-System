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
                ResetConnections();

                // Connection names and labels
                string[] connections = { "MIS_Conn1", "MIS_Conn2", "MIS_Conn3", "MIS_Conn4", "MIS_Conn5" };
                System.Web.UI.WebControls.Label[] labels = { conn1, conn2, conn3, conn4, conn5 };

                // Query templates
                string queryParent = "SELECT IS_ACTIVE FROM OracleLinkedServer..HEISCO.V_HEISCO_EMPLOYEE_ACCESS WHERE EMPFNO=@EmpID";
                string queryOthers = "SELECT IS_ACTIVE FROM OracleLinkedServer..HEISCO.V_HEISCO_EMPLOYEE_ACCESS WHERE EMPFID=@EmpID OR EMPFBP=@EmpID";

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

                        using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
                        using (SqlCommand cmd = new SqlCommand((i == selectedCompanyIndex) ? queryParent : queryOthers, conn))
                        {
                            cmd.Parameters.AddWithValue("@EmpID", empId);
                            cmd.CommandTimeout = 5;
                            conn.Open();

                            object result = cmd.ExecuteScalar();

                            if (result == null || result == DBNull.Value)
                            {
                                labels[i].CssClass += " gray"; // No record found → Gray
                            }
                            else
                            {
                                recordFoundAnywhere = true;
                                string status = result.ToString().Trim();

                                if (status == "0")  // Assuming 0 = inactive
                                {
                                    labels[i].CssClass += " red"; // inactive → Red
                                }
                                else
                                {
                                    labels[i].CssClass += " green"; // active → Green
                                    inactiveFound = false;
                                }
                            }
                        }
                    }
                    catch
                    {
                        labels[i].CssClass += " gray"; // Connection failure → Gray
                    }
                }

                // Final status message
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
