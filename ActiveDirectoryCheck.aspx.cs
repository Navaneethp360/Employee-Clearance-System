using System;
using System.DirectoryServices;
using System.Web.UI;

namespace MedicalSystem
{
    public partial class ActiveDirectoryCheck : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblStatus.Text = string.Empty;
                connAD.CssClass = "connection-box gray"; // Default gray on load
            }
        }

        protected void btnCheckAD_Click(object sender, EventArgs e)
        {
            string employeeId = txtEmployeeID.Text.Trim();

            if (string.IsNullOrEmpty(employeeId))
            {
                ShowStatus("Please enter an Employee ID.", true);
                connAD.Text = "Active Directory: Waiting...";
                connAD.CssClass = "connection-box gray";
                return;
            }

            var result = CheckUserInAD(employeeId);

            switch (result.Status)
            {
                case ADStatus.Enabled:
                    ShowStatus($"User {result.AccountName} found and Active in Active Directory!", false);
                    connAD.Text = $"Active Directory: {result.AccountName} (Active)";
                    connAD.CssClass = "connection-box green";
                    break;

                case ADStatus.Disabled:
                    ShowStatus($"User {result.AccountName} exists but is Disabled in Active Directory.", true);
                    connAD.Text = $"Active Directory: {result.AccountName} (Disabled)";
                    connAD.CssClass = "connection-box red";
                    break;

                case ADStatus.NotFound:
                    ShowStatus("User not found in Active Directory.", true);
                    connAD.Text = "Active Directory: Not Found";
                    connAD.CssClass = "connection-box red";
                    break;

                case ADStatus.Error:
                default:
                    ShowStatus("Error connecting to Active Directory.", true);
                    connAD.Text = "Active Directory: Error";
                    connAD.CssClass = "connection-box red";
                    break;
            }
        }


        private void ShowStatus(string message, bool isError)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = isError ? "status-message error" : "status-message";
            lblStatus.Style["display"] = "block";
        }

        // Enumeration for different AD check results
        private enum ADStatus { Enabled, Disabled, NotFound, Error }
        private class ADResult
        {
            public ADStatus Status { get; set; }
            public string AccountName { get; set; }
        }

        private ADResult CheckUserInAD(string employeeId)
        {
            ADResult resultObj = new ADResult { Status = ADStatus.NotFound, AccountName = string.Empty };

            try
            {
                using (DirectoryEntry entry = new DirectoryEntry("LDAP://Corp"))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        // Search by employee ID in description or pager
                        searcher.Filter = $"(&(objectCategory=user)(|(description={employeeId})(pager={employeeId})))";
                        searcher.PropertiesToLoad.Add("userAccountControl");
                        searcher.PropertiesToLoad.Add("sAMAccountName");
                        searcher.PropertiesToLoad.Add("description");
                        searcher.PropertiesToLoad.Add("pager");

                        SearchResult result = searcher.FindOne();

                        if (result == null)
                            return resultObj; // No user found

                        // Get account name if available
                        if (result.Properties.Contains("sAMAccountName"))
                            resultObj.AccountName = result.Properties["sAMAccountName"][0].ToString();

                        // Check account status
                        if (result.Properties.Contains("userAccountControl"))
                        {
                            int uac = (int)result.Properties["userAccountControl"][0];
                            bool isDisabled = (uac & 0x2) != 0;

                            resultObj.Status = isDisabled ? ADStatus.Disabled : ADStatus.Enabled;
                        }
                        else
                        {
                            resultObj.Status = ADStatus.Enabled; // Assume enabled if not set
                        }
                    }
                }
            }
            catch
            {
                resultObj.Status = ADStatus.Error;
            }

            return resultObj;
        }


    }
}
