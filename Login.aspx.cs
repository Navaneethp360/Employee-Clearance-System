using System;
using System.Data.SqlClient;
using System.Configuration;
using System.DirectoryServices;
using System.Security.Cryptography;
using System.Text;

namespace MedicalSystem.Account
{
    public partial class Login : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {

            lblMessage.Visible = false;
        }
        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    // Step 1: Try DB login first (username + password)
                    string dbLoginQuery = "SELECT UserID, RoleID, Password, CompanyID, CostCenter FROM Users WHERE Username = @Username";
                    SqlCommand dbCmd = new SqlCommand(dbLoginQuery, con);
                    dbCmd.Parameters.AddWithValue("@Username", username);

                    con.Open();
                    SqlDataReader dbReader = dbCmd.ExecuteReader();

                    if (dbReader.Read())
                    {
                        string storedHash = dbReader["Password"].ToString();  // Get the stored hash from DB

                        // Check using bcrypt
                        if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                        {
                            // DB login success
                            Session["UserID"] = dbReader["UserID"];
                            Session["RoleID"] = dbReader["RoleID"];
                            Session["Username"] = username;
                            Session["ClinicID"] = dbReader["CompanyID"];
                            Session["UserCostCenter"] = dbReader["CostCenter"] != DBNull.Value ? dbReader["CostCenter"].ToString() : "";

                            // Log login event
                            LogAuditTrail(
                                Convert.ToInt32(dbReader["UserID"]),
                                username,
                                "Login",
                                Request.UserHostAddress,
                                Convert.ToInt32(dbReader["CompanyID"])
                            );
                            Response.Redirect("~/Home.aspx");
                            return;
                        }
                    }
                    dbReader.Close();
                }

                // Step 2: If DB login fails, check if username exists in DB
                int userId = 0;
                int roleId = 0;
                int clinicId = 0;
                string costCenter = "";
                bool userExists = false;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string checkUserQuery = "SELECT UserID, RoleID, CompanyID, CostCenter FROM Users WHERE Username = @Username";
                    SqlCommand userCmd = new SqlCommand(checkUserQuery, con);
                    userCmd.Parameters.AddWithValue("@Username", username);

                    con.Open();
                    SqlDataReader userReader = userCmd.ExecuteReader();
                    if (userReader.Read())
                    {
                        userExists = true;
                        userId = Convert.ToInt32(userReader["UserID"]);
                        roleId = Convert.ToInt32(userReader["RoleID"]);
                        clinicId = Convert.ToInt32(userReader["CompanyID"]); // treat CompanyID as ClinicID
                        costCenter = userReader["CostCenter"] != DBNull.Value ? userReader["CostCenter"].ToString() : "";
                    }
                    userReader.Close();
                }

                if (userExists)
                {
                    // Step 3: Try AD Authentication
                    bool isAdAuthenticated = AuthenticateUser(username, password);
                    if (isAdAuthenticated)
                    {
                        Session["UserID"] = userId;
                        Session["RoleID"] = roleId;
                        Session["Username"] = username;
                        Session["ClinicID"] = clinicId;
                        Session["UserCostCenter"] = costCenter;

                        // Log login
                        LogAuditTrail(userId, username, "Login", Request.UserHostAddress, clinicId);

                        Response.Redirect("~/Home.aspx");
                        return;
                    }
                }

                // Final fallback - all failed
                lblMessage.Text = "Invalid username or password.";
                lblMessage.Visible = true;
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.Visible = true;
            }
        }


        public void LogAuditTrail(int userId, string username, string action, string ip, int? clinicId = null)
        {
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                string query = @"INSERT INTO AuditTrail (UserID, Username, Action, IPAddress, ClinicID)
                         VALUES (@UserID, @Username, @Action, @IPAddress, @ClinicID)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@IPAddress", ip);
                    cmd.Parameters.AddWithValue("@ClinicID", (object)clinicId ?? DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private bool AuthenticateUser(string userCode, string userPwd)
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry("LDAP://Corp", userCode, userPwd);
                DirectorySearcher searcher = new DirectorySearcher(entry)
                {
                    Filter = "(objectCategory=user)"
                };
                SearchResult result = searcher.FindOne();
                return result != null;
            }
            catch
            {
                // AD authentication failed
                return false;
            }
        }
    }
}
