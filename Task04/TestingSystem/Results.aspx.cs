using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Linq;

namespace TestingSystem
{
    public partial class Results : System.Web.UI.Page
    {
        private readonly string testPath;
        private readonly string resultsPath;
        private XElement tests;
        private XElement results;

        public Results() : base()
        {
            testPath = Server.MapPath("~/DB/Tests.xml");
            resultsPath = Server.MapPath("~/DB/Results.xml");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            tests = XElement.Load(testPath);
            results = XElement.Load(resultsPath);
            
            if (results.HasElements)
            {
                ShowResults();
            }
            else
            {
                Label noResults = new Label();
                noResults.Text = "No data to display.";
                Controls.Add(noResults);
            }           

        }

        private void ShowResults()
        {
            //holy godness...
            var query =
                from test in tests.Elements("Test")
                from result in results.Elements("Result")
                where test.Attribute("id").Value == result.Element("TestId").Value
                select new
                {
                    TestId = test.Attribute("id").Value,
                    TestTitle = test.Element("Title").Value,
                    QuestionId = result.Element("QuestionId").Value,
                    User = result.Element("User").Value,
                    Passed = result.Attribute("passed").Value == "yes"
                } into testData
                group testData by testData.TestId into testDataByTestId
                select new
                {
                    TestTitle = tests.Elements("Test")
                        .Where(el => el.Attribute("id").Value == testDataByTestId.Key)
                        .First().Element("Title").Value,
                    UserResults =
                        from user_count_passed in (   
                            from res in testDataByTestId
                            group res by res.User into g
                            select new
                            {
                                User = g.Key,
                                Count = g.Where(el => el.Passed).Count()
                            }
                        )
                        group user_count_passed by user_count_passed.Count into g
                        select new
                        {
                            PassedValue = g.Key,
                            Count = g.Count()
                        }

                };
            
            foreach (var test in query)
            {
                var data = new List<Tuple<double, string>>();

                HtmlGenericControl paragraph = new HtmlGenericControl("p");

                var testTitleHeader = new HtmlGenericControl("h3");
                testTitleHeader.InnerText = test.TestTitle;
                paragraph.Controls.Add(testTitleHeader);

                foreach (var userResult in test.UserResults)
                {
                    data.Add(new Tuple<double, string>(userResult.Count
                        , String.Format("People gave {0} right answers {1} times."
                            , userResult.PassedValue.ToString()
                            , userResult.Count.ToString()
                            )
                        )
                    );
                }
                byte[] buffer = ChartGenerator.CreateChart(500, 250, data).ToArray();

                Image resultDiagram = new Image();
                resultDiagram.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(buffer);
                paragraph.Controls.Add(resultDiagram);

                Form.Controls.Add(paragraph);
            }
        }
    }
}