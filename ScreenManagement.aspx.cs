using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace MedicalSystem
{
    public partial class ScreenManagement : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            CheckUserAccess();
            if (!IsPostBack)
            {
                LoadScreens();
            }
        }
        private void CheckUserAccess()
        {
            // 1. Check session
            if (Session["UserID"] == null || Session["RoleID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            int roleId = Convert.ToInt32(Session["RoleID"]);
            int userId = Convert.ToInt32(Session["UserID"]);
            string currentPath = System.IO.Path.GetFileName(Request.Url.AbsolutePath);

            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // 2. Get the screen ID based on current path
                    string screenQuery = @"SELECT ScreenID FROM Screens WHERE LOWER(ScreenName) = @Path";
                    SqlCommand screenCmd = new SqlCommand(screenQuery, con);
                    screenCmd.Parameters.AddWithValue("@Path", currentPath.ToLower());
                    object screenIdObj = screenCmd.ExecuteScalar();

                    if (screenIdObj == null)
                    {
                        // Page not registered in Screens table
                        Response.Redirect("~/Unauthorized.aspx");
                        return;
                    }

                    int screenId = Convert.ToInt32(screenIdObj);

                    // 3. Check if the current role or additional user screens include this screen
                    string permissionQuery = @"
                SELECT COUNT(*) 
                FROM RolePermissions rp
                WHERE rp.RoleID = @RoleID AND rp.ScreenID = @ScreenID

                UNION ALL

                SELECT COUNT(*) 
                FROM UserExtraScreens ue
                WHERE ue.UserID = @UserID AND ue.ScreenID = @ScreenID
            ";

                    SqlCommand permCmd = new SqlCommand(permissionQuery, con);
                    permCmd.Parameters.AddWithValue("@RoleID", roleId);
                    permCmd.Parameters.AddWithValue("@UserID", userId);
                    permCmd.Parameters.AddWithValue("@ScreenID", screenId);

                    int accessCount = 0;
                    using (SqlDataReader reader = permCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            accessCount += Convert.ToInt32(reader[0]);
                        }
                    }

                    if (accessCount == 0)
                    {
                        // Neither role nor extra screens give access
                        Response.Redirect("~/Unauthorized.aspx");
                        return;
                    }
                }
            }
            catch
            {
                Response.Redirect("~/Unauthorized.aspx");
            }
        }
        private void LoadScreens()
        {
            List<Screen> screens = new List<Screen>();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand(
    "SELECT ScreenID, ScreenName, ScreenPath, GroupName, DisplayOrder FROM Screens ORDER BY GroupName, DisplayOrder", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    screens.Add(new Screen
                    {
                        ScreenID = (int)reader["ScreenID"],
                        ScreenName = reader["ScreenName"].ToString(),
                        ScreenPath = reader["ScreenPath"].ToString(),
                        GroupName = reader["GroupName"] != DBNull.Value ? reader["GroupName"].ToString() : string.Empty,
                        DisplayOrder = reader["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(reader["DisplayOrder"]) : 0
                    });
                }
            }

            gvScreens.DataSource = screens;
            gvScreens.DataBind();
        }


        protected void btnAddScreen_Click(object sender, EventArgs e)
        {
            string screenName = txtScreenName.Text.Trim();
            string screenPath = txtScreenPath.Text.Trim();
            string groupName = txtGroupName.Text.Trim();
            int displayOrder = string.IsNullOrEmpty(txtDisplayOrder.Text.Trim()) ? 0 : Convert.ToInt32(txtDisplayOrder.Text.Trim());

            if (string.IsNullOrEmpty(screenName) || string.IsNullOrEmpty(screenPath))
            {
                lblMessage.Text = "All fields are required.";
                lblMessage.Visible = true;
                return;
            }

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd;

                if (ViewState["EditScreenID"] != null)
                {
                    int screenID = (int)ViewState["EditScreenID"];
                    cmd = new SqlCommand(@"UPDATE Screens 
                                SET ScreenName = @ScreenName, ScreenPath = @ScreenPath, GroupName = @GroupName, DisplayOrder = @DisplayOrder 
                                WHERE ScreenID = @ScreenID", con);
                    cmd.Parameters.AddWithValue("@ScreenID", screenID);
                }
                else
                {
                    cmd = new SqlCommand(@"INSERT INTO Screens (ScreenName, ScreenPath, GroupName, DisplayOrder) 
                                VALUES (@ScreenName, @ScreenPath, @GroupName, @DisplayOrder)", con);
                }

                cmd.Parameters.AddWithValue("@ScreenName", screenName);
                cmd.Parameters.AddWithValue("@ScreenPath", screenPath);
                cmd.Parameters.AddWithValue("@GroupName", groupName);
                cmd.Parameters.AddWithValue("@DisplayOrder", displayOrder);

                con.Open();
                cmd.ExecuteNonQuery();
            }


            lblMessage.Text = ViewState["EditScreenID"] != null ? "Screen updated successfully!" : "Screen added successfully!";
            lblMessage.Visible = true;

            ViewState["EditScreenID"] = null;
            btnAddScreen.Text = "Add Screen";
            txtScreenName.Text = "";
            txtScreenPath.Text = "";
            txtGroupName.Text = "";
            txtDisplayOrder.Text = "";

            LoadScreens();
        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int screenID = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM Screens WHERE ScreenID = @ScreenID", con);
                cmd.Parameters.AddWithValue("@ScreenID", screenID);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            LoadScreens();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int screenID = Convert.ToInt32(btn.CommandArgument);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT ScreenName, ScreenPath FROM Screens WHERE ScreenID = @ScreenID", con);
                cmd.Parameters.AddWithValue("@ScreenID", screenID);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtScreenName.Text = reader["ScreenName"].ToString();
                    txtScreenPath.Text = reader["ScreenPath"].ToString();
                    ViewState["EditScreenID"] = screenID;

                    btnAddScreen.Text = "Update Screen";
                }
            }
        }


        public class Screen
        {
            public int ScreenID { get; set; }
            public string ScreenName { get; set; }
            public string ScreenPath { get; set; }
            public string GroupName { get; set; }
            public int DisplayOrder { get; set; }
        }

    }
}
