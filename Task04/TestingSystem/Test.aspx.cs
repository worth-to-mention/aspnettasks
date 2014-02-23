using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using TestingSystem.Controls;
using TestingSystem.Utils.Data;
using TestingSystem.Utils.Visualization;

namespace TestingSystem
{
    public partial class Test : System.Web.UI.Page
    {
        private TestControl testControl;
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
            testControl = new TestControl();
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
            MembershipUser user = Membership.GetUser();
                
            using (var context = new DataAccess.TestingSystemDataContext(connectionString))
            {                
                userRes = context.GetUserTestingResults(user.ProviderUserKey, testId);
            }
            if (userRes == null)
            {
                ShowErrorPage();
                return;
            }


            var resultText = new HtmlGenericControl("div");
            resultText.Attributes["class"] = "content-list-item content-list-header";
            resultText.InnerText = String.Format("{0}, you have gave the right answer for {1} of {2} questions."
                , user.UserName
                , userRes.Passed
                , userRes.Count
                );
            ContentPlaceholder.Controls.Add(resultText);

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Attributes["class"] = "content-list-item";

            int failed = userRes.Count - userRes.Passed;
            var data = new List<Tuple<double, string>>();
            data.Add(new Tuple<double, string>(userRes.Passed, String.Format("You gave {0} right answers.", userRes.Passed.ToString())));
            if (failed > 0)
                data.Add(new Tuple<double, string>(failed, String.Format("You gave {0} wrong answers.", failed.ToString())));
            byte[] buffer = ChartGenerator.CreateChart(500, 250, data).ToArray();

            Image resultDiagram = new Image();
            resultDiagram.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(buffer);
            div.Controls.Add(resultDiagram);

            ContentPlaceholder.Controls.Add(div);
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
            
            var resultsData = new List<Pair<int, bool>>();
            foreach (var testQuestion in testControl.Questions)
            {
                var pair = new Pair<int, bool>
                {
                    First = testQuestion.QuestionId,
                    Second = testQuestion.CheckAnswer()
                };
                resultsData.Add(pair);
            }
            using (var context = new DataAccess.TestingSystemDataContext(connectionString))
            {
                object userID = Membership.GetUser().ProviderUserKey;
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
            testControl = new TestControl();
            testControl.Title = test.Title;
            testControl.CssClass = "test";
            testControl.TitleCssClass = "test-header content-list-item content-list-header";
            testControl.TestContentCssClass = "test-content";
            testControl.QuestionCssClass = "test-question content-list-item";
            testControl.TestSubmitButtonCssClass = "test-submit content-list-item content-list-footer";
            testControl.QuestionTitleCssClass = "test-question-title";

            foreach(var question in test.Questions)
            {
                testControl.AddQuestion(CreateQuestion(question));
            }
            testControl.SubmitButtonText = "Send";
            testControl.Submit += testControl_Submit;

            ContentPlaceholder.Controls.Add(testControl);
        }

        private void testControl_Submit(object sender, TestSubmitEventArgs e)
        {
            var usr = Membership.GetUser();
            if (usr == null)
            {
                FormsAuthentication.RedirectToLoginPage();
                return;
            }
            if (IsValid)
            {
                if (firstTime || true)
                {
                    SaveResults();
                    firstTime = false;
                }
                ShowResults();
                testControl.Visible = false;
            }
        }

        private void ShowAvailableTests()
        {
            ContentPlaceholder.Controls.Clear();
            var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
            var conSettings = config.ConnectionStrings;
            if (conSettings.ConnectionStrings.Count < 0)
            {
                ShowErrorPage();
                return;
            }
            string connectionString = conSettings.ConnectionStrings["TestingSystem"].ConnectionString;

            var header = new HtmlGenericControl("div");
            header.Attributes["class"] = "content-list-item content-list-header";

            var h2 = new HtmlGenericControl("h2");
            h2.InnerText = "Available tests:";
            header.Controls.Add(h2);
            
            ContentPlaceholder.Controls.Add(header);

            var tests = new List<DataAccess.Test>();
            using (var context = new DataAccess.TestingSystemDataContext(connectionString))
            {
                tests = context.GetTest();
            }

            foreach (var test in tests)
            {
                var content = new HtmlGenericControl("div");
                content.Attributes["class"] = "content-list-item";

                HyperLink a = new HyperLink();
                a.Text = test.Title;
                a.NavigateUrl = "/Test.aspx?id=" + test.TestID.ToString();
                content.Controls.Add(a);

                ContentPlaceholder.Controls.Add(content);
            }
        }

        private TestQuestion CreateQuestion(DataAccess.Question question)
        {
            switch (question.Type)
            {
                case TestingSystem.DataAccess.QuestionType.Radio:
                    return CreateRadioQuestion(question);
                case TestingSystem.DataAccess.QuestionType.Select:
                    return CreateSelectQuestion(question);
                case TestingSystem.DataAccess.QuestionType.Text:
                    return CreateTextQuestion(question);
                default:
                    ShowErrorPage();
                    return null;
            }
        }

        private TextTestQuestion CreateTextQuestion(DataAccess.Question question)
        {
            TextTestQuestion textQuestion = new TextTestQuestion();
            textQuestion.CssClass = "test-testQuestion test-textTestQuestion";
            textQuestion.Title = question.Text;
            textQuestion.QuestionId = question.QuestionID;
            textQuestion.Answer = question.Options[0].Text;
            return textQuestion;
        }

        private SelectTestQuestion CreateSelectQuestion(DataAccess.Question question)
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
            return selectQuestion;
        }

        private RadioTestQuestion CreateRadioQuestion(DataAccess.Question question)
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
            return radioQuestion;
        }
    }
}