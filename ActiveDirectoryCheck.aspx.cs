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
            string username = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ShowStatus("Please enter a username.", true);
                connAD.Text = "Active Directory: Waiting...";
                connAD.CssClass = "connection-box gray";
                return;
            }

            var status = CheckUserInAD(username);

            switch (status)
            {
                case ADStatus.Enabled:
                    ShowStatus("User found and Active in Active Directory!", false);
                    connAD.Text = "Active Directory: Active";
                    connAD.CssClass = "connection-box green";
                    break;

                case ADStatus.Disabled:
                    ShowStatus("User exists but is Disabled in Active Directory.", true);
                    connAD.Text = "Active Directory: Disabled";
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

        private ADStatus CheckUserInAD(string userCode)
        {
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry("LDAP://Corp"))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        // Search user by username
                        searcher.Filter = $"(&(objectCategory=user)(sAMAccountName={userCode}))";
                        searcher.PropertiesToLoad.Add("userAccountControl");
                        SearchResult result = searcher.FindOne();

                        if (result == null)
                            return ADStatus.NotFound; // No such user

                        if (result.Properties.Contains("userAccountControl"))
                        {
                            int uac = (int)result.Properties["userAccountControl"][0];
                            bool isDisabled = (uac & 0x2) != 0; // ACCOUNTDISABLE bit

                            return isDisabled ? ADStatus.Disabled : ADStatus.Enabled;
                        }

                        // If property not found, assume user exists but unknown status
                        return ADStatus.Enabled;
                    }
                }
            }
            catch
            {
                return ADStatus.Error; // Connection failure or AD not reachable
            }
        }
    }
}
