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
        private readonly Dictionary<int, string> testUrls;
        private string testsFile;

        public Default()
        {
            testsFile = Server.MapPath("~/DB/Tests.xml");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!File.Exists(testsFile))
                return;

            XElement xmlTests = XElement.Load(testsFile);

            var query = xmlTests.Descendants("Test").Select(test =>
            {
                int testId = Int32.Parse(test.Attribute("id").Value);
                return new
                {
                    Title = test.Element("Title").Value,
                    Id = testId
                };
            });

            TestList.Controls.Clear();

            foreach(var test in query)
            {
                WebControl li = new WebControl(HtmlTextWriterTag.Li);
                HyperLink a = new HyperLink();
                a.Text = test.Title;
                a.NavigateUrl = "/Test.aspx?id=" + test.Id.ToString();
                li.Controls.Add(a);

                TestList.Controls.Add(li);
            }
        }
    }
}