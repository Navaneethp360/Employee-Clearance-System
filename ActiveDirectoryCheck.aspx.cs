using System;
using System.Collections.Generic;
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

            var results = CheckUserInAD(employeeId);

            if (results.Count == 1 && results[0].Status == ADStatus.NotFound)
            {
                ShowStatus("User not found in Active Directory.", true);
                connAD.Text = "Active Directory: Not Found";
                connAD.CssClass = "connection-box red";
                return;
            }

            string allUsers = "";
            bool hasDisabled = false;

            foreach (var r in results)
            {
                string statusText = r.Status == ADStatus.Enabled ? "Active" :
                                    r.Status == ADStatus.Disabled ? "Disabled" :
                                    "Error";

                if (r.Status == ADStatus.Disabled) hasDisabled = true;

                allUsers += $"Username: {r.AccountName} ({statusText})<br/>";
            }

            connAD.Text = allUsers;
            connAD.CssClass = hasDisabled ? "connection-box red" : "connection-box green";
            ShowStatus($"Found {results.Count} matching AD accounts.", false);
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

        private List<ADResult> CheckUserInAD(string employeeId)
        {
            var resultsList = new List<ADResult>();

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

                        SearchResultCollection results = searcher.FindAll();

                        if (results.Count == 0)
                        {
                            // No matches found
                            resultsList.Add(new ADResult { Status = ADStatus.NotFound, AccountName = "" });
                            return resultsList;
                        }

                        foreach (SearchResult res in results)
                        {
                            string accountName = res.Properties.Contains("sAMAccountName")
                                ? res.Properties["sAMAccountName"][0].ToString()
                                : "(Unknown)";

                            ADStatus status = ADStatus.Enabled;
                            if (res.Properties.Contains("userAccountControl"))
                            {
                                int uac = (int)res.Properties["userAccountControl"][0];
                                bool isDisabled = (uac & 0x2) != 0;
                                status = isDisabled ? ADStatus.Disabled : ADStatus.Enabled;
                            }

                            resultsList.Add(new ADResult { Status = status, AccountName = accountName });
                        }
                    }
                }
            }
            catch
            {
                resultsList.Clear();
                resultsList.Add(new ADResult { Status = ADStatus.Error, AccountName = "" });
            }

            return resultsList;
        }


    }
}
