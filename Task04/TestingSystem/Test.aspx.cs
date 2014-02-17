using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Text;

using TestQuestionControls;

namespace TestingSystem
{
    public partial class Test : System.Web.UI.Page
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
        private readonly List<TestQuestion> questions;
        private int testId;
        private bool firstTime
        {
            get
            {
                var tmp = Session["FirstTIme"];
                return tmp == null ? true : (bool)tmp;
            }
            set
            {
                Session["FirstTIme"] = value;
            }
        }

        public Test()
        {
            tn = "http://maleficus.com/Test";
            testPath = Server.MapPath("~/DB/Tests.xml");
            testSchemaPath = Server.MapPath("~/DB/Tests.xsd");
            rn = "http://maleficus.com/Result";
            resultsPath = Server.MapPath("~/DB/Results.xml");
            resultsSchemaPath = Server.MapPath("~/DB/Results.xsd");
            questions = new List<TestQuestion>();
            schemas = new XmlSchemaSet();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);            
            if (!File.Exists(testPath))
            {
                ShowErrorPage();
                return;
            }
            tests = XDocument.Load(testPath);



            // Load schemas

            if (!File.Exists(testSchemaPath))
            {
                ShowErrorPage();
                return;
            }
            if (!File.Exists(resultsSchemaPath))
            {
                ShowErrorPage();
                return;
            }
            schemas.Add(tn.ToString(), testSchemaPath);
            schemas.Add(rn.ToString(), resultsSchemaPath);

            // Check tests

            bool error = false;
            
            tests.Validate(schemas, (s, a) => error = true);
            if (error)
            {
                ShowErrorPage();
                return;
            }


            //Save current test id for futher usage
            if (!Int32.TryParse(Request.Params["id"], out testId))
            {
                ShowErrorPage();
                return;
            }
            CreateTest(testId);                        
        }
        protected void Page_Load(object sender, EventArgs e)
        {            
            if (IsPostBack)
            {
                Validate();
                if (IsValid)
                {
                    if (!File.Exists(resultsPath))
                        CreateResultsFile();
                    else
                        results = XDocument.Load(resultsPath);
                    bool error = false;

                    results.Validate(schemas, (s, a) => error = true);
                    if (error)
                    {
                        ShowErrorPage();
                        return;
                    }

                    if (firstTime || true)
                    {
                        SaveResults();
                        firstTime = false;
                    }
                    ShowResults();

                    TestPlaceHolder.Visible = false;
                }
            }                
        }

        private void ShowResults()
        {
            var query = 
                from result in results.Root.Elements(rn + "Result")
                .Where(el => String.Equals(el.Element(rn + "User").Value, UserName.Text.Trim().ToLower(), StringComparison.CurrentCultureIgnoreCase))
                .Where(el => el.Element(rn + "TestId").Value == testId.ToString())
                select new 
                {
                    Passed = result.Attribute("passed").Value == "yes"
                };
            var userResults = query.ToList();

            int passed = userResults.Where(res => res.Passed).Count();
            int failed = userResults.Count - passed;

            HtmlGenericControl paragraph = new HtmlGenericControl("p");

            var resultText = new HtmlGenericControl("p");
            resultText.InnerText = String.Format("{0}, you have gave the right answer for {1} of {2} questions."
                , UserName.Text
                , passed.ToString()
                , userResults.Count
                );
            paragraph.Controls.Add(resultText);

            var data = new List<Tuple<double, string>>();
            data.Add(new Tuple<double, string>(passed, String.Format("You gave {0} right answers.", passed.ToString())));
            if (failed > 0)
                data.Add(new Tuple<double, string>(failed, String.Format("You gave {0} wrong answers.", failed.ToString())));
            byte[] buffer = ChartGenerator.CreateChart(500, 250, data).ToArray();

            Image resultDiagram = new Image();
            resultDiagram.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(buffer);
            paragraph.Controls.Add(resultDiagram);

            Form.Controls.Add(paragraph);
        }

        private void SaveResults()
        {
            foreach(var testQuestion in questions)
            {
                string user = UserName.Text.Trim().ToLower();
                string test = testId.ToString();
                string question = testQuestion.QuestionId.ToString();
                bool passed = testQuestion.CheckAnswer();
                XElement result = null;
                if (results.Root.HasElements)
                {
                    //Check if we already have record for such a user, a test, and an question.
                    result = results.Root.Elements(rn + "Result")
                    .Where(res =>
                    {
                        return String.Equals(res.Element(rn + "User").Value, user, StringComparison.CurrentCultureIgnoreCase)
                            && res.Element(rn + "TestId").Value == test
                            && res.Element(rn + "QuestionId").Value == question;
                    }).FirstOrDefault();
                }                 
                if (result == null)                
                {
                    //If such a record does not exist, create it.
                    result = new XElement(rn + "Result",
                        new XElement(rn + "User", user),
                        new XElement(rn + "TestId", test),
                        new XElement(rn + "QuestionId", question)
                        );
                    results.Root.Add(result);
                }
                result.SetAttributeValue("passed", passed ? "yes" : "no");
            }

            results.Save(resultsPath);
        }

        private void ShowErrorPage()
        {
            Response.StatusCode = 404;
            Response.End();
        }

        private void CreateTest(int testId)
        {
            XElement test = tests.Root
                .Elements(tn + "Test")
                .Where(el => el.Attribute("id").Value == testId.ToString())
                .FirstOrDefault();
            if (test == null)
            {
                ShowErrorPage();
                return;
            }
            testHeader.Text = test.Element(tn + "Title").Value;
            foreach (var question in test.Element(tn + "Questions").Elements(tn + "Question"))
            {
                switch (question.Attribute("type").Value)
                {
                    case "Radio":
                        CreateRadioQuestion(question);
                        break;
                    case "Select":
                        CreateSelectQuestion(question);
                        break;
                    case "Text":
                        CreateTextQuestion(question);
                        break;
                    default:
                        ShowErrorPage();
                        return;
                }
            }
            questions.ForEach(el => TestQuestions.Controls.Add(el));
        }

        private void CreateTextQuestion(XElement question)
        {
            TextTestQuestion textQuestion = new TextTestQuestion();
            textQuestion.Title = question.Element(tn + "Text").Value;
            textQuestion.QuestionId = Convert.ToInt32(question.Attribute("id").Value);
            textQuestion.Answer = question.Element(tn + "Answer").Value;
            questions.Add(textQuestion);
        }

        private void CreateSelectQuestion(XElement question)
        {
            SelectTestQuestion selectQuestion = new SelectTestQuestion();
            selectQuestion.Title = question.Element(tn + "Text").Value;
            selectQuestion.QuestionId = Convert.ToInt32(question.Attribute("id").Value);
            selectQuestion.AddItems(question.Element(tn + "Options").Elements(tn + "Option").Select(option =>
            {
                if (option.Attribute("isanswer") != null)
                {
                    selectQuestion.AddAnswerId(option.Attribute("id").Value);
                }
                return new ListItem
                {
                    Text = option.Value,
                    Value = option.Attribute("id").Value
                };
            }));
            questions.Add(selectQuestion);
        }

        private void CreateRadioQuestion(XElement question)
        {
            RadioTestQuestion radioQuestion = new RadioTestQuestion();
            radioQuestion.Title = question.Element(tn + "Text").Value;
            radioQuestion.QuestionId = Convert.ToInt32(question.Attribute("id").Value);
            radioQuestion.AddItems(question.Element(tn + "Options").Elements(tn + "Option").Select(option =>
            {
                if (option.Attribute("isanswer") != null)
                {
                    radioQuestion.AnswerItemId = option.Attribute("id").Value;
                }
                return new ListItem
                {
                    Text = option.Value,
                    Value = option.Attribute("id").Value
                };
            }));
            questions.Add(radioQuestion);
        }

        private void CreateResultsFile()
        {
            results = new XDocument(
                new XElement(rn + "Results",
                    new XAttribute(XNamespace.Xmlns + "x", "http://maleficus.com/Result")));
            results.Save(resultsPath);
        }
    }
}