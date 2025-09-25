using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
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
            string prefix = ddlCompany.SelectedValue.Trim();
            string empId = prefix + "-" + txtEmployeeID.Text.Trim(); // For UserMaster checks (with prefix)
            string emp_num = txtEmployeeID.Text.Trim(); // For TAS_EMP_INFO name check (no prefix)
            int selectedCompanyIndex = ddlCompany.SelectedIndex - 1;

            if (string.IsNullOrEmpty(empId) || selectedCompanyIndex < 0)
            {
                lblStatus.Text = "❌ Please select a company and enter an Employee ID.";
                lblStatus.CssClass = "status-message error";
                lblStatus.Style["display"] = "block";
                return;
            }

            // Map dropdown to company codes for TAS_EMP_INFO
            var companyCodes = new Dictionary<string, string>
    {
        { "H", "HEISCO" },
        { "G", "GDCO" },
        { "T", "HTSCO" },
        { "K", "HSA" },
        { "S", "GULFSKY" }
    };

            string companyCode = companyCodes.ContainsKey(prefix) ? companyCodes[prefix] : prefix;

            try
            {
                ResetConnections();

                string[] connections = { "NONERP_Conn1", "NONERP_Conn2", "NONERP_Conn3" };
                string[] userQueries = {
            "SELECT ActiveStatus, UserCode FROM [SAL].[dbo].[UserMaster] WHERE Remark=@EmpID",
            "SELECT ActiveStatus, UserCode FROM [OSR].[dbo].[UserMaster] WHERE Remark=@EmpID",
            "SELECT ActiveStatus, UserCode FROM [TRF_NEW].[dbo].[UserMaster] WHERE Remark=@EmpID"
        };
                string[] empQueries = {
            "SELECT EMPLOYEENAME FROM [SAL].[dbo].[TAS_EMP_INFO] WHERE EMPLOYEEID=@EmpNum AND Company=@Company",
            "SELECT EMPLOYEENAME FROM [OSR].[dbo].[TAS_EMP_INFO] WHERE EMPLOYEEID=@EmpNum AND Company=@Company",
            "SELECT EMPLOYEENAME FROM [TRF_NEW].[dbo].[TAS_EMP_INFO] WHERE EMPLOYEEID=@EmpNum AND Company=@Company"
        };

                string[] systemNames = { "Salary Certificate", "Facilities Management", "Employee Training (TRF)" };
                System.Web.UI.WebControls.Label[] labels = { conn1, conn2, conn3 };

                bool hasPendingItems = false;
                bool recordFoundAnywhere = false;

                for (int i = 0; i < connections.Length; i++)
                {
                    try
                    {
                        string connStr = ConfigurationManager.ConnectionStrings[connections[i]].ConnectionString;
                        using (SqlConnection conn = new SqlConnection(connStr))
                        {
                            conn.Open();

                            // --- UserMaster: get ActiveStatus and UserCode ---
                            int? status = null;
                            string userCode = null;
                            using (SqlCommand cmdUser = new SqlCommand(userQueries[i], conn))
                            {
                                cmdUser.Parameters.AddWithValue("@EmpID", empId);
                                using (SqlDataReader reader = cmdUser.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        status = reader["ActiveStatus"] != DBNull.Value ? Convert.ToInt32(reader["ActiveStatus"]) : (int?)null;
                                        userCode = reader["UserCode"] != DBNull.Value ? reader["UserCode"].ToString().Trim() : null;
                                    }
                                }
                            }

                            // --- TAS_EMP_INFO: get Employee Name ---
                            string empName = "No Name Found";
                            using (SqlCommand cmdEmp = new SqlCommand(empQueries[i], conn))
                            {
                                cmdEmp.Parameters.AddWithValue("@EmpNum", emp_num);
                                cmdEmp.Parameters.AddWithValue("@Company", companyCode);
                                object nameResult = cmdEmp.ExecuteScalar();
                                if (nameResult != null) empName = nameResult.ToString().Trim();
                            }

                            // --- Update label ---
                            if (status == null)
                            {
                                labels[i].CssClass += " gray";
                                labels[i].Text = $"{systemNames[i]}: No record found";
                            }
                            else
                            {
                                recordFoundAnywhere = true;
                                if (status == 1)
                                {
                                    labels[i].CssClass += " green"; // Pending
                                    hasPendingItems = true;
                                }
                                else
                                {
                                    labels[i].CssClass += " red"; // Cleared
                                }

                                // Show Employee Name + Username
                                labels[i].Text = $"{systemNames[i]}: {empName}";
                                if (!string.IsNullOrEmpty(userCode))
                                {
                                    labels[i].Text += $" | Username: {userCode}";
                                }
                            }
                        }
                    }
                    catch
                    {
                        labels[i].CssClass += " gray";
                        labels[i].Text = $"{systemNames[i]}: Connection failed";
                    }
                }

                // --- Final status message ---
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
