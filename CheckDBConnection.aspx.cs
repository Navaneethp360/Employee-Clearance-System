using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace MedicalSystem
{
    public partial class CheckDBConnection : System.Web.UI.Page
    {
        private const string MASTER_PASSWORD = "HEISCO";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["Authenticated"] != null && (bool)Session["Authenticated"])
                {
                    pnlLogin.Visible = false;
                    pnlDashboard.Visible = true;
                    CheckDatabaseConnection();
                }
                else
                {
                    pnlLogin.Visible = true;
                    pnlDashboard.Visible = false;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text == MASTER_PASSWORD)
            {
                Session["Authenticated"] = true;
                pnlLogin.Visible = false;
                pnlDashboard.Visible = true;
                CheckDatabaseConnection();
            }
            else
            {
                lblLoginStatus.Text = "❌ Incorrect password.";
                pnlLogin.Visible = true;
                pnlDashboard.Visible = false;
            }
        }

        private void CheckDatabaseConnection()
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string getTablesQuery = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";
                    SqlCommand cmdTables = new SqlCommand(getTablesQuery, con);

                    DataTable dt = new DataTable();
                    dt.Columns.Add("TableName", typeof(string));
                    dt.Columns.Add("RowCount", typeof(long));

                    List<string> tables = new List<string>();
                    using (SqlDataReader rdr = cmdTables.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            tables.Add(rdr.GetString(0));
                        }
                    }

                    foreach (string tableName in tables)
                    {
                        long rowCount = 0;
                        try
                        {
                            string countQuery = $"SELECT COUNT(*) FROM [{tableName}]";
                            SqlCommand cmdCount = new SqlCommand(countQuery, con);
                            object result = cmdCount.ExecuteScalar();
                            if (result != DBNull.Value)
                                rowCount = Convert.ToInt64(result);
                        }
                        catch
                        {
                            rowCount = 0;
                        }

                        dt.Rows.Add(tableName, rowCount);
                    }

                    gvTables.DataSource = dt;
                    gvTables.DataBind();

                    int totalTables = dt.Rows.Count;
                    long totalRows = 0;
                    int emptyTables = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        long rc = Convert.ToInt64(row["RowCount"]);
                        totalRows += rc;
                        if (rc == 0)
                            emptyTables++;
                    }

                    lblTotalTables.Text = totalTables.ToString();
                    lblTotalRows.Text = totalRows.ToString("N0");
                    lblEmptyTables.Text = emptyTables.ToString();
                    lblLastRefreshed.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    lblStatus.CssClass = "status-label success";
                    lblStatus.Text = "✅ Connection successful! Available tables and their row counts are listed below.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.CssClass = "status-label error";
                lblStatus.Text = "❌ Error: " + ex.Message;
            }
        }

        protected void btnSyncData_Click(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sync", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 600;
                        cmd.ExecuteNonQuery();
                    }
                }

                CheckDatabaseConnection();

                lblStatus.CssClass = "status-label success";
                lblStatus.Text = "✅ Sync completed successfully.";
            }
            catch (Exception ex)
            {
                lblStatus.CssClass = "status-label error";
                lblStatus.Text = "❌ Sync failed: " + ex.Message;
            }
        }

        protected void btnFlushData_Click(object sender, EventArgs e)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    string[] tablesToFlush = { "Vitals", "MedicalInfo", "ExternalVisitRecords", "EmployeeRecords", "AuditTrail", "FitnessReportData", "FitnessReportHeader" };

                    foreach (string table in tablesToFlush)
                    {
                        using (SqlCommand cmd = new SqlCommand($"DELETE FROM {table}", con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    CheckDatabaseConnection();
                    lblStatus.CssClass = "status-label success";
                    lblStatus.Text = "✅ Data in EmployeeRecords, MedicalInfo, ExternalVisitRecords, AuditTrail, Fitness Reports and Vitals has been successfully flushed.";
                }
            }
            catch (Exception ex)
            {
                lblStatus.CssClass = "status-label error";
                lblStatus.Text = "❌ Flush failed: " + ex.Message;
            }
        }
    }
}
