using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace TestingSystem
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            var conSettings = config.ConnectionStrings;
            if (conSettings.ConnectionStrings.Count < 0)
            {
                ShowErrorPage();
                return;
            }
            string connectionString = conSettings.ConnectionStrings["TestingSystem"].ConnectionString;

            TestList.Controls.Clear();

            var tests = new List<DataAccess.Test>();
            using(var context = new DataAccess.TestingSystemDataContext(connectionString))
            {
                tests = context.GetTest();
            }

            foreach(var test in tests)
            {
                WebControl li = new WebControl(HtmlTextWriterTag.Li);
                HyperLink a = new HyperLink();
                a.Text = test.Title;
                a.NavigateUrl = "/Test.aspx?id=" + test.TestID.ToString();
                li.Controls.Add(a);

                TestList.Controls.Add(li);
            }
        }
        private void ShowErrorPage()
        {
            Response.StatusCode = 404;
            Response.End();
        }
    }
}