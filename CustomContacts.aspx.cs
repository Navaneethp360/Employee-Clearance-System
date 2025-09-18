using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PhoneDir
{
    public partial class CustomContacts : System.Web.UI.Page
    {
        private string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCompanies();
                BindCustomContacts();
            }
        }

        #region Load AD Companies / Departments
        private void LoadCompanies()
        {
            ddlCompany.Items.Clear();
            ddlCompany.Items.Add(new ListItem("-- Select Company --", ""));
            string adJson = "";

            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 BlobContent FROM AD_JSON_Blob ORDER BY LastSynced DESC", con))
            {
                con.Open();
                var obj = cmd.ExecuteScalar();
                if (obj != null) adJson = obj.ToString();
            }

            if (!string.IsNullOrEmpty(adJson))
            {
                var js = new JavaScriptSerializer();
                var adData = js.Deserialize<DirectoryResponse>(adJson);
                foreach (var company in adData.Companies.OrderBy(c => c.OrgName))
                {
                    ddlCompany.Items.Add(new ListItem(company.OrgName, company.OrgName));
                }
            }

            ddlDepartment.Items.Clear();
            ddlDepartment.Items.Add(new ListItem("-- Select Department --", ""));
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlDepartment.Items.Clear();
            ddlDepartment.Items.Add(new ListItem("-- Select Department --", ""));
            string selectedCompany = ddlCompany.SelectedValue;
            if (string.IsNullOrEmpty(selectedCompany)) return;

            string adJson = "";
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 BlobContent FROM AD_JSON_Blob ORDER BY LastSynced DESC", con))
            {
                con.Open();
                var obj = cmd.ExecuteScalar();
                if (obj != null) adJson = obj.ToString();
            }

            if (!string.IsNullOrEmpty(adJson))
            {
                var js = new JavaScriptSerializer();
                var adData = js.Deserialize<DirectoryResponse>(adJson);
                var company = adData.Companies.FirstOrDefault(c => c.OrgName.Equals(selectedCompany, StringComparison.OrdinalIgnoreCase));
                if (company != null)
                {
                    foreach (var dept in company.Departments.OrderBy(d => d.DeptName))
                    {
                        ddlDepartment.Items.Add(new ListItem(dept.DeptName, dept.DeptName));
                    }
                }
            }
        }
        #endregion

        #region Save Contacts
        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Determine company and department
            string company = string.IsNullOrWhiteSpace(txtCompanyNew.Text) ? ddlCompany.SelectedValue : txtCompanyNew.Text.Trim();
            string department = string.IsNullOrWhiteSpace(txtDeptNew.Text) ? ddlDepartment.SelectedValue : txtDeptNew.Text.Trim();

            // Validate company/department
            if (string.IsNullOrEmpty(company) || string.IsNullOrEmpty(department))
            {
                lblMessage.Text = "Company and Department are required.";
                lblMessage.CssClass = "status-message error";
                lblMessage.Visible = true;
                return;
            }

            // Read employee input
            string empID = txtEmpID.Text.Trim();
            string name = txtName.Text.Trim();
            string designation = txtDesignation.Text.Trim();
            string extension = txtExtension.Text.Trim();
            string mobile = txtMobile.Text.Trim();
            string location = txtLocation.Text.Trim();
            string subDept = txtSubDept.Text.Trim();

            // Validate employee name
            if (string.IsNullOrWhiteSpace(name))
            {
                lblMessage.Text = "Please enter employee name.";
                lblMessage.CssClass = "status-message error";
                lblMessage.Visible = true;
                return;
            }

            // Create contact object
            CustomContact contact = new CustomContact
            {
                Company = company,
                Department = department,
                EmpID = empID,
                Name = name,
                Designation = designation,
                Extension = extension,
                Mobile = mobile,
                Location = location,
                SubDept = subDept
            };

            // Save to database
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO CustomContacts
            (Company, Department, EmpID, Name, Designation, Extension, Mobile, Location, SubDept)
            VALUES (@Company,@Department,@EmpID,@Name,@Designation,@Extension,@Mobile,@Location,@SubDept)", con))
                {
                    cmd.Parameters.AddWithValue("@Company", contact.Company);
                    cmd.Parameters.AddWithValue("@Department", contact.Department);
                    cmd.Parameters.AddWithValue("@EmpID", contact.EmpID);
                    cmd.Parameters.AddWithValue("@Name", contact.Name);
                    cmd.Parameters.AddWithValue("@Designation", contact.Designation);
                    cmd.Parameters.AddWithValue("@Extension", contact.Extension);
                    cmd.Parameters.AddWithValue("@Mobile", contact.Mobile);
                    cmd.Parameters.AddWithValue("@Location", contact.Location);
                    cmd.Parameters.AddWithValue("@SubDept", contact.SubDept);
                    cmd.ExecuteNonQuery();
                }
            }

            lblMessage.Text = "Contact saved successfully!";
            lblMessage.CssClass = "status-message";
            lblMessage.Visible = true;

            // Refresh GridView
            BindCustomContacts();
        }


        #endregion

        #region Bind & Delete Grid
        private void BindCustomContacts()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM CustomContacts ORDER BY CreatedOn DESC", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvCustomContacts.DataSource = dt;
                gvCustomContacts.DataBind();
            }
        }

        protected void gvCustomContacts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteContact")
            {
                // CommandArgument already contains the ID
                int id = Convert.ToInt32(e.CommandArgument);

                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("DELETE FROM CustomContacts WHERE ID=@ID", con))
                {
                    cmd.Parameters.AddWithValue("@ID", id);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                // Rebind GridView after deletion
                BindCustomContacts();
            }
        }

        #endregion

        #region Models
        private class CustomContact
        {
            public string Company { get; set; }
            public string Department { get; set; }
            public string EmpID { get; set; }
            public string Name { get; set; }
            public string Designation { get; set; }
            public string Extension { get; set; }
            public string Mobile { get; set; }
            public string Location { get; set; }
            public string SubDept { get; set; }
        }

        public class DirectoryResponse { public List<Company> Companies { get; set; } = new List<Company>(); }
        public class Company { public int OrgID { get; set; } public string OrgName { get; set; } = ""; public List<Department> Departments { get; set; } = new List<Department>(); }
        public class Department { public int DeptID { get; set; } public string DeptName { get; set; } = ""; public List<Employee> Employees { get; set; } = new List<Employee>(); }
        public class Employee { public int EmployeePK { get; set; } public string EmpID { get; set; } = ""; public string Name { get; set; } = ""; public string Designation { get; set; } = ""; public string Extension { get; set; } = ""; public string Mobile { get; set; } = ""; public string Location { get; set; } = ""; public string SubDept { get; set; } = ""; }
        #endregion
    }
}
