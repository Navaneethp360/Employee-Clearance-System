using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;

namespace MedicalSystem
{
    public partial class UserManagement : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Visible = false;
            CheckUserAccess();
            if (!IsPostBack)
            {
                LoadCostCenters();
                LoadRoles();
                LoadCompanies();
                LoadUsers();
                LoadScreens();
                LoadUsersForExtraScreens(); 
                LoadExtraScreens();
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
        private void LoadRoles()
        {
            ddlNewUserRole.Items.Clear();
            ddlRoles.Items.Clear();
            ddlExistingRoles.Items.Clear();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT RoleID, RoleName FROM Roles", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string roleName = reader["RoleName"].ToString();
                    string roleId = reader["RoleID"].ToString();

                    ddlNewUserRole.Items.Add(new ListItem(roleName, roleId));
                    ddlRoles.Items.Add(new ListItem(roleName, roleId));
                    ddlExistingRoles.Items.Add(new ListItem(roleName, roleId));
                }
            }
        }

        protected void btnAddRole_Click(object sender, EventArgs e)
        {
            string roleName = txtNewRoleName.Text.Trim();

            if (string.IsNullOrEmpty(roleName))
            {
                ShowMessage("Role name cannot be empty.", false);
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "INSERT INTO Roles (RoleName) VALUES (@RoleName)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@RoleName", roleName);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                txtNewRoleName.Text = "";
                LoadRoles();
                ShowMessage("Role added successfully.");
            }
            catch (Exception ex)
            {
                ShowMessage("Error adding role: " + ex.Message, false);
            }
        }
        protected void btnDeleteRole_Click(object sender, EventArgs e)
        {
            if (ddlExistingRoles.SelectedValue == "")
            {
                ShowMessage("Please select a role to delete.", false);
                return;
            }

            int roleId = Convert.ToInt32(ddlExistingRoles.SelectedValue);

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // 1. Check if any user has this role
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE RoleID = @RoleID";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@RoleID", roleId);
                    int userCount = (int)checkCmd.ExecuteScalar();

                    if (userCount > 0)
                    {
                        ShowMessage("Cannot delete this role because it is currently assigned to one or more users.", false);
                        return;
                    }

                    // 2. Safe to delete the role
                    string deleteQuery = "DELETE FROM Roles WHERE RoleID = @RoleID";
                    SqlCommand deleteCmd = new SqlCommand(deleteQuery, con);
                    deleteCmd.Parameters.AddWithValue("@RoleID", roleId);
                    deleteCmd.ExecuteNonQuery();
                }

                LoadRoles();
                ShowMessage("Role deleted successfully.");
            }
            catch (Exception ex)
            {
                ShowMessage("Error deleting role: " + ex.Message, false);
            }
        }

        private void LoadCostCenters()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                string query = @"SELECT CST_NUM, CST_E_DESC 
                         FROM TAS_COSTCENTERS 
                         WHERE CST_STATUS = '1'
                         ORDER BY CST_NUM";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                ddlNewUserCostCenter.Items.Clear();
                ddlNewUserCostCenter.Items.Add(new ListItem("Select Cost Center", ""));

                while (reader.Read())
                {
                    string num = reader["CST_NUM"].ToString();
                    string desc = reader["CST_E_DESC"].ToString();
                    ddlNewUserCostCenter.Items.Add(new ListItem($"{num} - {desc}", num));
                }
            }
        }

        private void LoadCompanies()
        {
            ddlCompany.Items.Clear();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT CompanyID, CompanyName FROM Companies", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ddlCompany.Items.Add(new ListItem(reader["CompanyName"].ToString(), reader["CompanyID"].ToString()));
                }
            }
        }

        private void LoadUsers()
        {
            ddlUsers.Items.Clear();
            ddlUsers.Items.Add(new ListItem("-- Select User --", ""));
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT UserID, Username FROM Users", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ddlUsers.Items.Add(new ListItem(reader["Username"].ToString(), reader["UserID"].ToString()));
                }
            }
        }

        private void LoadScreens()
        {
            chkRolePermissions.Items.Clear();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT ScreenID, ScreenName FROM Screens", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    chkRolePermissions.Items.Add(new ListItem(reader["ScreenName"].ToString(), reader["ScreenID"].ToString()));
                }
            }
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate Cost Center selection
                if (string.IsNullOrEmpty(ddlNewUserCostCenter.SelectedValue))
                {
                    ShowMessage("Please select a Cost Center.", isSuccess: false);
                    return;
                }

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    // Updated query to include CostCenter column
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Users (Username, Password, RoleID, CompanyID, CostCenter) " +
                        "VALUES (@Username, @Password, @RoleID, @CompanyID, @CostCenter)", con);

                    string plainPassword = txtNewPassword.Text;
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

                    cmd.Parameters.AddWithValue("@Username", txtNewUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.Parameters.AddWithValue("@RoleID", ddlNewUserRole.SelectedValue);
                    cmd.Parameters.AddWithValue("@CompanyID", ddlCompany.SelectedValue);
                    cmd.Parameters.AddWithValue("@CostCenter", ddlNewUserCostCenter.SelectedValue);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadUsers();
                ClearFormFields();
                ShowMessage("User added successfully!");
            }
            catch (Exception ex)
            {
                ShowMessage("Failed to add user: " + ex.Message, isSuccess: false);
            }
        }


        protected void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (ddlUsers.SelectedValue != "")
            {
                int userId = int.Parse(ddlUsers.SelectedValue);
                try
                {
                    using (SqlConnection con = new SqlConnection(connStr))
                    {
                        con.Open();

                        // 1. Delete extra screens
                        SqlCommand delExtraCmd = new SqlCommand("DELETE FROM UserExtraScreens WHERE UserID=@UserID", con);
                        delExtraCmd.Parameters.AddWithValue("@UserID", userId);
                        delExtraCmd.ExecuteNonQuery();

                        // 2. Delete user
                        SqlCommand delUserCmd = new SqlCommand("DELETE FROM Users WHERE UserID=@UserID", con);
                        delUserCmd.Parameters.AddWithValue("@UserID", userId);
                        delUserCmd.ExecuteNonQuery();
                    }

                    LoadUsers();
                    LoadUsersForExtraScreens();
                    ShowMessage("User deleted successfully.");
                }
                catch (Exception ex)
                {
                    ShowMessage("Error deleting user: " + ex.Message, isSuccess: false);
                }
            }
            else
            {
                ShowMessage("Please select a user to delete.", isSuccess: false);
            }
        }


        protected void ddlRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlRoles.SelectedValue != "")
            {
                LoadPermissionsForRole(int.Parse(ddlRoles.SelectedValue));
            }
        }

        private void LoadPermissionsForRole(int roleId)
        {
            foreach (ListItem item in chkRolePermissions.Items)
                item.Selected = false;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"
                    SELECT s.ScreenID 
                    FROM RolePermissions rp
                    JOIN Screens s ON rp.ScreenID = s.ScreenID
                    WHERE rp.RoleID = @RoleID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@RoleID", roleId);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string screenId = reader["ScreenID"].ToString();
                    ListItem item = chkRolePermissions.Items.FindByValue(screenId);
                    if (item != null)
                        item.Selected = true;
                }
            }
        }

        protected void btnSaveRolePermissions_Click(object sender, EventArgs e)
        {
            int roleId = int.Parse(ddlRoles.SelectedValue);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction txn = con.BeginTransaction();

                try
                {
                    SqlCommand deleteCmd = new SqlCommand("DELETE FROM RolePermissions WHERE RoleID = @RoleID", con, txn);
                    deleteCmd.Parameters.AddWithValue("@RoleID", roleId);
                    deleteCmd.ExecuteNonQuery();

                    foreach (ListItem item in chkRolePermissions.Items)
                    {
                        if (item.Selected)
                        {
                            SqlCommand insertCmd = new SqlCommand("INSERT INTO RolePermissions (RoleID, ScreenID) VALUES (@RoleID, @ScreenID)", con, txn);
                            insertCmd.Parameters.AddWithValue("@RoleID", roleId);
                            insertCmd.Parameters.AddWithValue("@ScreenID", item.Value);
                            insertCmd.ExecuteNonQuery();
                        }
                    }

                    txn.Commit();
                    ShowMessage("Permissions updated successfully.");
                }
                catch (Exception ex)
                {
                    txn.Rollback();
                    ShowMessage("Error updating permissions: " + ex.Message, isSuccess: false);
                }
            }
        }

        private void ShowMessage(string message, bool isSuccess = true)
        {
            lblMessage.Visible = true;
            lblMessage.CssClass = isSuccess ? "alert-message alert-success" : "alert-message alert-error";
            lblMessage.Text = message;
        }
        private void ClearFormFields()
        {
            txtNewUsername.Text = "";
            txtNewPassword.Text = "";
            ddlNewUserRole.ClearSelection();
            ddlCompany.ClearSelection();
            ddlUsers.ClearSelection();
            chkRolePermissions.ClearSelection();
        }
        private void LoadUsersForExtraScreens()
        {
            ddlUsersExtra.Items.Clear();
            ddlUsersExtra.Items.Add(new ListItem("-- Select User --", ""));
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT UserID, Username FROM Users", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ddlUsersExtra.Items.Add(new ListItem(reader["Username"].ToString(), reader["UserID"].ToString()));
                }
            }
        }

        private void LoadExtraScreens()
        {
            chkExtraScreens.Items.Clear();
            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT ScreenID, ScreenName FROM Screens", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    chkExtraScreens.Items.Add(new ListItem(reader["ScreenName"].ToString(), reader["ScreenID"].ToString()));
                }
            }
        }

        protected void ddlUsersExtra_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkExtraScreens.Items.Clear();

            if (string.IsNullOrEmpty(ddlUsersExtra.SelectedValue))
                return;

            int userId = int.Parse(ddlUsersExtra.SelectedValue);
            int roleId = 0;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 1. Get the user's role
                SqlCommand roleCmd = new SqlCommand("SELECT RoleID FROM Users WHERE UserID=@UserID", con);
                roleCmd.Parameters.AddWithValue("@UserID", userId);
                object roleObj = roleCmd.ExecuteScalar();
                if (roleObj != null)
                    roleId = Convert.ToInt32(roleObj);

                // 2. Get all screens the role already has
                HashSet<int> roleScreens = new HashSet<int>();
                if (roleId != 0)
                {
                    SqlCommand rsCmd = new SqlCommand("SELECT ScreenID FROM RolePermissions WHERE RoleID=@RoleID", con);
                    rsCmd.Parameters.AddWithValue("@RoleID", roleId);
                    SqlDataReader rsReader = rsCmd.ExecuteReader();
                    while (rsReader.Read())
                        roleScreens.Add(Convert.ToInt32(rsReader["ScreenID"]));
                    rsReader.Close();
                }

                // 3. Get all screens that the user currently has as extra screens
                HashSet<int> userExtraScreens = new HashSet<int>();
                SqlCommand ueCmd = new SqlCommand("SELECT ScreenID FROM UserExtraScreens WHERE UserID=@UserID", con);
                ueCmd.Parameters.AddWithValue("@UserID", userId);
                SqlDataReader ueReader = ueCmd.ExecuteReader();
                while (ueReader.Read())
                    userExtraScreens.Add(Convert.ToInt32(ueReader["ScreenID"]));
                ueReader.Close();

                // 4. Load all screens that are NOT part of the role
                SqlCommand screensCmd = new SqlCommand("SELECT ScreenID, ScreenName FROM Screens", con);
                SqlDataReader screensReader = screensCmd.ExecuteReader();
                while (screensReader.Read())
                {
                    int screenId = Convert.ToInt32(screensReader["ScreenID"]);
                    if (!roleScreens.Contains(screenId))
                    {
                        ListItem li = new ListItem(screensReader["ScreenName"].ToString(), screenId.ToString());
                        li.Selected = userExtraScreens.Contains(screenId); // tick if user already has it
                        chkExtraScreens.Items.Add(li);
                    }
                }
                screensReader.Close();
            }
        }


        protected void btnSaveExtraScreens_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlUsersExtra.SelectedValue))
                return;

            int userId = int.Parse(ddlUsersExtra.SelectedValue);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                // delete old extra screens
                SqlCommand delCmd = new SqlCommand("DELETE FROM UserExtraScreens WHERE UserID=@UserID", con);
                delCmd.Parameters.AddWithValue("@UserID", userId);
                delCmd.ExecuteNonQuery();

                // insert new selections
                foreach (ListItem li in chkExtraScreens.Items)
                {
                    if (li.Selected)
                    {
                        SqlCommand insertCmd = new SqlCommand("INSERT INTO UserExtraScreens(UserID, ScreenID) VALUES(@UserID, @ScreenID)", con);
                        insertCmd.Parameters.AddWithValue("@UserID", userId);
                        insertCmd.Parameters.AddWithValue("@ScreenID", li.Value);
                        insertCmd.ExecuteNonQuery();
                    }
                }
            }

            lblMessage.Text = "Extra screens updated successfully!";
            lblMessage.CssClass = "alert-message alert-success";
            lblMessage.Visible = true;
        }


    }
}
