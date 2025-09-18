using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MedicalSystem
{
    public partial class Unauthorized : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                litUsername.Text = Session["Username"] != null ? Session["Username"].ToString() : "User";
            }
        }

    }
}