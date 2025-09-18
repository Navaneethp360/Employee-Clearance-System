using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI;

namespace MedicalSystem
{
    public partial class ConnectionHealthCheck : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnCheckConnections_Click(object sender, EventArgs e)
        {
            try
            {
                // MIS connections
                CheckAndSetLabel(mis1, "MIS_Conn1");
                CheckAndSetLabel(mis2, "MIS_Conn2");
                CheckAndSetLabel(mis3, "MIS_Conn3");
                CheckAndSetLabel(mis4, "MIS_Conn4");
                CheckAndSetLabel(mis5, "MIS_Conn5");

                // PMMS connections
                CheckAndSetLabel(pmms1, "PMMS_Conn1");
                CheckAndSetLabel(pmms2, "PMMS_Conn2");
                CheckAndSetLabel(pmms3, "PMMS_Conn3");
                CheckAndSetLabel(pmms4, "PMMS_Conn4");
                CheckAndSetLabel(pmms5, "PMMS_Conn5");

                // NONERP connections
                CheckAndSetLabel(nonerp1, "NONERP_Conn1");
                CheckAndSetLabel(nonerp2, "NONERP_Conn2");
                CheckAndSetLabel(nonerp3, "NONERP_Conn3");
                CheckAndSetLabel(nonerp4, "NONERP_Conn4");
                CheckAndSetLabel(nonerp5, "NONERP_Conn5");
            }
            catch (Exception ex)
            {
                // Optional: show general error
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", $"alert('Error: {ex.Message}')", true);
            }
        }

       private void CheckAndSetLabel(System.Web.UI.WebControls.Label lbl, string connStrName)
{
    try
    {
        string connStr = ConfigurationManager.ConnectionStrings[connStrName].ConnectionString;

        // Use SqlConnectionStringBuilder to safely adjust timeout
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connStr)
        {
            ConnectTimeout = 5 // 5 seconds instead of default 15
        };

        using (SqlConnection conn = new SqlConnection(builder.ConnectionString))
        {
            conn.Open(); // try to connect
            lbl.CssClass = "connection-box green";
            lbl.Text += " ✅"; // optional: add visual checkmark
        }
    }
    catch (Exception ex)
    {
        lbl.CssClass = "connection-box red";
        lbl.Text += " ❌"; // optional: add visual cross
        // Optional: log the error somewhere if needed
        // System.Diagnostics.Debug.WriteLine($"{connStrName} failed: {ex.Message}");
    }
}

    }
}
