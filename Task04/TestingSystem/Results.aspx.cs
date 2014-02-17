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
        private readonly string testPath;
        private readonly string testSchemaPath;
        private readonly string resultsPath;
        private readonly string resultsSchemaPath;
        private XmlSchemaSet schemas;
        private XNamespace tn;
        private XDocument tests;
        private XNamespace rn;
        private XDocument results;

        public Results() : base()
        {
            tn = "http://maleficus.com/Test";
            testPath = Server.MapPath("~/DB/Tests.xml");
            testSchemaPath = Server.MapPath("~/DB/Tests.xsd");
            rn = "http://maleficus.com/Result";
            resultsPath = Server.MapPath("~/DB/Results.xml");
            resultsSchemaPath = Server.MapPath("~/DB/Results.xsd");
            schemas = new XmlSchemaSet();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!File.Exists(testPath))
            {
                ShowNoDataMessage();
                return;
            }
            if (!File.Exists(resultsPath))
            {
                ShowNoDataMessage();
                return;
            }
            tests = XDocument.Load(testPath);
            results = XDocument.Load(resultsPath);

            // Load schemas

            if (!File.Exists(testSchemaPath))
            {
                ShowNoDataMessage();
                return;
            }
            if (!File.Exists(resultsSchemaPath))
            {
                ShowNoDataMessage();
                return;
            }
            schemas.Add(tn.ToString(), testSchemaPath);
            schemas.Add(rn.ToString(), resultsSchemaPath);

            // Validate files

            bool error = false;

            tests.Validate(schemas, (s, a) => error = true);
            if (error)
            {
                ShowNoDataMessage();
                return;
            }
            results.Validate(schemas, (s, a) => error = true);
            if (error)
            {
                ShowNoDataMessage();
                return;
            }

            if (results.Root.HasElements)
            {
                ShowResults();
            }
            else
            {
                ShowNoDataMessage();
            }           

        }

        private void ShowNoDataMessage()
        {
            Label noResults = new Label();
            noResults.Text = "No data to display.";
            Controls.Add(noResults);
        }

        private void ShowResults()
        {
            // holy goddess...
            var query =
                from test in tests.Root.Elements(tn + "Test")
                from result in results.Root.Elements(rn + "Result")
                where test.Attribute("id").Value == result.Element(rn + "TestId").Value
                select new
                {
                    TestId = test.Attribute("id").Value,
                    TestTitle = test.Element(tn + "Title").Value,
                    QuestionId = result.Element(rn + "QuestionId").Value,
                    User = result.Element(rn + "User").Value,
                    Passed = result.Attribute("passed").Value == "yes"
                } into testData
                group testData by testData.TestId into testDataByTestId
                select new
                {
                    TestTitle = tests.Root.Elements(tn + "Test")
                        .Where(el => el.Attribute("id").Value == testDataByTestId.Key)
                        .First().Element(tn + "Title").Value,
                    UserResults =
                        from user_passed_count in (   
                            from res in testDataByTestId
                            group res by res.User into g
                            select new
                            {
                                User = g.Key,
                                Count = g.Where(el => el.Passed).Count()
                            }
                        )
                        group user_passed_count by user_passed_count.Count into g
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