using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Xml.Schema;

namespace TestingSystem
{
    public partial class Results : System.Web.UI.Page
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
            List<DataAccess.TesttingStats> stats;
            using (var context = new DataAccess.TestingSystemDataContext(connectionString))
            {
                stats = context.GetTestingStats();
            }
            if (stats.Count == 0)
            {
                ShowNoDataMessage();
            }
            else
            {
                ShowResults(stats);
            }
        }

        private void ShowNoDataMessage()
        {
            Label noResults = new Label();
            noResults.Text = "No data to display.";
            ResultsContent.Controls.Add(noResults);
        }

        private void ShowResults(List<DataAccess.TesttingStats> stats)
        {            
            foreach (var stat in stats)
            {
                var data = new List<Tuple<double, string>>();

                HtmlGenericControl paragraph = new HtmlGenericControl("p");

                var testTitleHeader = new HtmlGenericControl("h3");
                testTitleHeader.InnerText = stat.TestTitle;
                paragraph.Controls.Add(testTitleHeader);

                foreach (var el in stat.AnsweringStat)
                {
                    data.Add(new Tuple<double, string>(el.Second
                        , String.Format("{0}% right answers."
                            , el.First.ToString()
                            , el.Second.ToString()
                            )
                        )
                    );
                }
                byte[] buffer = ChartGenerator.CreateChart(500, 250, data).ToArray();

                Image resultDiagram = new Image();
                resultDiagram.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(buffer);
                paragraph.Controls.Add(resultDiagram);
                ResultsContent.Controls.Add(paragraph);
            }
        }
        private void ShowErrorPage()
        {
            Response.StatusCode = 404;
            Response.End();
        }
    }
}