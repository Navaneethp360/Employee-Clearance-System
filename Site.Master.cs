using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace MedicalSystem
{
    public partial class SiteMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["RoleID"] == null || Session["UserID"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            int roleId = Convert.ToInt32(Session["RoleID"]);
            int userId = Convert.ToInt32(Session["UserID"]);

            // Always rebuild navigation links on every page load
            var allowedScreens = GetAllowedScreensForUser(userId, roleId);
            PopulateNavigationLinks(allowedScreens);
        }

        private void PopulateNavigationLinks(List<Screen> allowedScreens)
        {
            DynamicNavPanel.Controls.Clear(); // clear previous links

            var groupedScreens = allowedScreens
                .GroupBy(s => s.GroupName)
                .OrderBy(g => g.Key);

            foreach (var group in groupedScreens)
            {
                Literal groupHeader = new Literal
                {
                    Text = $"<h4 class='nav-header'>{group.Key}</h4>"
                };
                DynamicNavPanel.Controls.Add(groupHeader);

                foreach (var screen in group.OrderBy(s => s.DisplayOrder))
                {
                    AddNavigationLink(screen);
                }
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            if (Session["UserID"] != null && Session["Username"] != null)
            {
                LogTrail(
                    Convert.ToInt32(Session["UserID"]),
                    Session["Username"].ToString(),
                    "Logout",
                    Request.UserHostAddress,
                    Session["ClinicID"] != null ? Convert.ToInt32(Session["ClinicID"]) : 0
                );
            }

            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/Login.aspx");
        }

        public void LogTrail(int uid, string uname, string act, string ip, int? cid = null)
        {
            string cstr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(cstr))
            {
                string query = @"INSERT INTO AuditTrail (UserID, Username, Action, IPAddress, ClinicID)
                                 VALUES (@UserID, @Username, @Action, @IPAddress, @ClinicID)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", uid);
                    cmd.Parameters.AddWithValue("@Username", uname);
                    cmd.Parameters.AddWithValue("@Action", act);
                    cmd.Parameters.AddWithValue("@IPAddress", ip);
                    cmd.Parameters.AddWithValue("@ClinicID", (object)cid ?? DBNull.Value);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private List<Screen> GetAllowedScreensForUser(int userId, int roleId)
        {
            List<Screen> screens = new List<Screen>();
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // 1. Screens from RolePermissions
                string roleQuery = @"
                    SELECT s.ScreenID, s.ScreenName, s.ScreenPath, s.GroupName, s.DisplayOrder
                    FROM RolePermissions rp
                    INNER JOIN Screens s ON rp.ScreenID = s.ScreenID
                    WHERE rp.RoleID = @RoleID";

                SqlCommand roleCmd = new SqlCommand(roleQuery, con);
                roleCmd.Parameters.AddWithValue("@RoleID", roleId);
                SqlDataReader reader = roleCmd.ExecuteReader();

                while (reader.Read())
                {
                    screens.Add(new Screen
                    {
                        ScreenID = (int)reader["ScreenID"],
                        ScreenName = reader["ScreenName"].ToString(),
                        ScreenPath = reader["ScreenPath"].ToString(),
                        GroupName = reader["GroupName"] != DBNull.Value ? reader["GroupName"].ToString() : "General",
                        DisplayOrder = reader["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(reader["DisplayOrder"]) : 0
                    });
                }
                reader.Close();

                // 2. Screens from UserExtraScreens that are NOT already in the list
                string extraQuery = @"
                    SELECT s.ScreenID, s.ScreenName, s.ScreenPath, s.GroupName, s.DisplayOrder
                    FROM UserExtraScreens ue
                    INNER JOIN Screens s ON ue.ScreenID = s.ScreenID
                    WHERE ue.UserID = @UserID";

                SqlCommand extraCmd = new SqlCommand(extraQuery, con);
                extraCmd.Parameters.AddWithValue("@UserID", userId);
                SqlDataReader extraReader = extraCmd.ExecuteReader();

                HashSet<int> existingScreenIds = screens.Select(s => s.ScreenID).ToHashSet();

                while (extraReader.Read())
                {
                    int screenId = (int)extraReader["ScreenID"];
                    if (!existingScreenIds.Contains(screenId))
                    {
                        screens.Add(new Screen
                        {
                            ScreenID = screenId,
                            ScreenName = extraReader["ScreenName"].ToString(),
                            ScreenPath = extraReader["ScreenPath"].ToString(),
                            GroupName = extraReader["GroupName"] != DBNull.Value ? extraReader["GroupName"].ToString() : "General",
                            DisplayOrder = extraReader["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(extraReader["DisplayOrder"]) : 0
                        });
                    }
                }
                extraReader.Close();
            }

            return screens;
        }

        private void AddNavigationLink(Screen screen)
        {
            if (string.IsNullOrWhiteSpace(screen.ScreenPath)) return;

            string formattedName = System.Text.RegularExpressions.Regex.Replace(screen.ScreenName, "([a-z])([A-Z])", "$1 $2");

            Literal navLink = new Literal
            {
                Text = $"<a href='{screen.ScreenPath}' class='nav-link d-block mb-1'>{formattedName}</a>"
            };

            DynamicNavPanel.Controls.Add(navLink);
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
