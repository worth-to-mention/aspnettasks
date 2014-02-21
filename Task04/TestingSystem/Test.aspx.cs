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
            questions = new List<TestQuestion>();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);  

            //Save current test id for futher usage
            if (!Int32.TryParse(Request.Params["id"], out testId))
            {
                ShowAvailableTests();
            }
            else
            {
                ShowTest(testId);
            }                        
        }


        protected void Page_Load(object sender, EventArgs e)
        {            
            if (IsPostBack)
            {
                Validate();
                if (IsValid)
                {
                    if (firstTime || true)
                    {
                        SaveResults();
                        firstTime = false;
                    }
                    ShowResults();
                    TestContent.Visible = false;
                }
            }                
        }

        private void ShowResults()
        {
            
            
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            var conSettings = config.ConnectionStrings;
            if (conSettings.ConnectionStrings.Count < 0)
            {
                ShowErrorPage();
                return;
            }
            string connectionString = conSettings.ConnectionStrings["TestingSystem"].ConnectionString;

            var userRes = new DataAccess.UserTestingResults();
            string user = UserName.Text.Trim().ToLower();
                
            using (var context = new DataAccess.TestingSystemDataContext(connectionString))
            {
                int userID = context.GetOrCreateUser(user);
                userRes = context.GetUserTestingResults(userID, testId);
            }
            if (userRes == null)
            {
                ShowErrorPage();
                return;
            }
            HtmlGenericControl paragraph = new HtmlGenericControl("p");

            var resultText = new HtmlGenericControl("p");
            resultText.InnerText = String.Format("{0}, you have gave the right answer for {1} of {2} questions."
                , user
                , userRes.Passed
                , userRes.Count
                );
            paragraph.Controls.Add(resultText);

            int failed = userRes.Count - userRes.Passed;
            var data = new List<Tuple<double, string>>();
            data.Add(new Tuple<double, string>(userRes.Passed, String.Format("You gave {0} right answers.", userRes.Passed.ToString())));
            if (failed > 0)
                data.Add(new Tuple<double, string>(failed, String.Format("You gave {0} wrong answers.", failed.ToString())));
            byte[] buffer = ChartGenerator.CreateChart(500, 250, data).ToArray();

            Image resultDiagram = new Image();
            resultDiagram.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(buffer);
            paragraph.Controls.Add(resultDiagram);

            TestPageContent.Controls.Add(paragraph);
        }

        private void SaveResults()
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            var conSettings = config.ConnectionStrings;
            if (conSettings.ConnectionStrings.Count < 0)
            {
                ShowErrorPage();
                return;
            }
            string connectionString = conSettings.ConnectionStrings["TestingSystem"].ConnectionString;   
            
            var resultsData = new List<DataAccess.Pair<int, bool>>();
            foreach (var testQuestion in questions)
            {
                var pair = new DataAccess.Pair<int, bool>
                {
                    First = testQuestion.QuestionId,
                    Second = testQuestion.CheckAnswer()
                };
                resultsData.Add(pair);
            }
            using (var context = new DataAccess.TestingSystemDataContext(connectionString))
            {
                string user = UserName.Text.Trim().ToLower();
                int userID = context.GetOrCreateUser(user);
                context.SaveUserResults(userID, testId, resultsData);
            }
        }

        private void ShowErrorPage()
        {
            Response.StatusCode = 404;
            Response.End();
        }

        private void ShowTest(int testId)
        {
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            var conSettings = config.ConnectionStrings;
            if (conSettings.ConnectionStrings.Count < 0)
            {
                ShowErrorPage();
                return;
            }
            string connectionString = conSettings.ConnectionStrings["TestingSystem"].ConnectionString;

            DataAccess.Test test;
            using(var context = new DataAccess.TestingSystemDataContext(connectionString))
            {
                test = context.GetTest(testId);
            }
            if (test == null)
            {
                ShowErrorPage();
                return;
            }

            testHeader.Text = test.Title;
            foreach(var question in test.Questions)
            {
                CreateQuestion(question);
            }
            questions.ForEach(el => TestQuestions.Controls.Add(el));
        }

        private void ShowAvailableTests()
        {
            TestPageContent.Controls.Clear();
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            var conSettings = config.ConnectionStrings;
            if (conSettings.ConnectionStrings.Count < 0)
            {
                ShowErrorPage();
                return;
            }
            string connectionString = conSettings.ConnectionStrings["TestingSystem"].ConnectionString;
            
            var h2 = new HtmlGenericControl("h2");
            h2.InnerText = "Available tests:";

            var ul = new HtmlGenericControl("ul");

            var tests = new List<DataAccess.Test>();
            using (var context = new DataAccess.TestingSystemDataContext(connectionString))
            {
                tests = context.GetTest();
            }

            foreach (var test in tests)
            {
                var li = new HtmlGenericControl("li");
                HyperLink a = new HyperLink();
                a.Text = test.Title;
                a.NavigateUrl = "/Test.aspx?id=" + test.TestID.ToString();
                li.Controls.Add(a);

                ul.Controls.Add(li);
            }
            TestPageContent.Controls.Add(h2);
            TestPageContent.Controls.Add(ul);
        }

        private void CreateQuestion(DataAccess.Question question)
        {
            switch (question.Type)
            {
                case TestingSystem.DataAccess.QuestionType.Radio:
                    CreateRadioQuestion(question);
                    break;
                case TestingSystem.DataAccess.QuestionType.Select:
                    CreateSelectQuestion(question);
                    break;
                case TestingSystem.DataAccess.QuestionType.Text:
                    CreateTextQuestion(question);
                    break;
                default:
                    ShowErrorPage();
                    return;
            }
        }

        private void CreateTextQuestion(DataAccess.Question question)
        {
            TextTestQuestion textQuestion = new TextTestQuestion();
            textQuestion.CssClass = "test-testQuestion test-textTestQuestion";
            textQuestion.Title = question.Text;
            textQuestion.QuestionId = question.QuestionID;
            textQuestion.Answer = question.Options[0].Text;
            questions.Add(textQuestion);
        }

        private void CreateSelectQuestion(DataAccess.Question question)
        {
            SelectTestQuestion selectQuestion = new SelectTestQuestion();
            selectQuestion.CssClass = "test-testQuestion test-selectTestQuestion";
            selectQuestion.Title = question.Text;
            selectQuestion.QuestionId = question.QuestionID;
            selectQuestion.AddItems(question.Options.Select(option =>
            {
                if (option.IsAnswer)
                {
                    selectQuestion.AddAnswerId(option.OptionID.ToString());
                }
                return new ListItem
                {
                    Text = option.Text,
                    Value = option.OptionID.ToString()
                };
            }));
            questions.Add(selectQuestion);
        }

        private void CreateRadioQuestion(DataAccess.Question question)
        {
            RadioTestQuestion radioQuestion = new RadioTestQuestion();
            radioQuestion.CssClass = "test-testQuestion test-radioTestQuestion";
            radioQuestion.Title = question.Text;
            radioQuestion.QuestionId = question.QuestionID;
            radioQuestion.AddItems(question.Options.Select(option =>
            {
                if (option.IsAnswer)
                {
                    radioQuestion.AnswerItemId = option.OptionID.ToString();
                }
                return new ListItem
                {
                    Text = option.Text,
                    Value = option.OptionID.ToString()
                };
            }));
            questions.Add(radioQuestion);
        }
    }
}