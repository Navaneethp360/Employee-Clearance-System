using System;
using System.Web.UI;

namespace GenericSystem
{
    public partial class Home : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Optional: set a default username for testing
            if (Session["Username"] == null)
                Session["Username"] = "Guest";
        }
    }
}
