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
                connAD.CssClass = "connection-box gray";
            }
        }

        protected void btnCheckAD_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                ShowStatus("Please enter a username.", true);
                connAD.CssClass = "connection-box gray";
                return;
            }

            bool userExists = CheckUserInAD(username);

            if (userExists)
            {
                ShowStatus("User found in Active Directory!", false);
                connAD.Text = "Active Directory: Connected";
                connAD.CssClass = "connection-box green";
            }
            else
            {
                ShowStatus("User not found in Active Directory.", true);
                connAD.Text = "Active Directory: Not Found / Inactive";
                connAD.CssClass = "connection-box red";
            }
        }

        private void ShowStatus(string message, bool isError)
        {
            lblStatus.Text = message;
            lblStatus.CssClass = isError ? "status-message error" : "status-message";
            lblStatus.Style["display"] = "block";
        }

        private bool CheckUserInAD(string userCode)
        {
            try
            {
                using (DirectoryEntry entry = new DirectoryEntry("LDAP://Corp"))
                {
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        // Search by sAMAccountName (username)
                        searcher.Filter = $"(&(objectCategory=user)(sAMAccountName={userCode}))";
                        searcher.PropertiesToLoad.Add("sAMAccountName");
                        SearchResult result = searcher.FindOne();
                        return result != null;
                    }
                }
            }
            catch
            {
                // AD check failed (server not reachable or user not found)
                return false;
            }
        }
    }
}
