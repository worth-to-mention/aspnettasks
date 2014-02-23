using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using TestingSystem.Utils.Data;

namespace TestingSystem.Controls
{
    public enum TestMode
    {
        Edit,
        Show
    }

    [
        ParseChildren(true, "Questions"),
        ToolboxData("<{0}:TestControl runat=\"server\"> </{0}:TestControl>")
    ]
    public class TestControl : WebControl, INamingContainer
    {
        private List<TestQuestion> questions;
        private Label title;
        private Button submit;
        private HtmlGenericControl testContent;


        public TestControl() : base(HtmlTextWriterTag.Div) { }

        #region Attrubites

        #region Appearance
        [Category("Appearance")]
        public string QuestionCssClass { get; set; }


        [Category("Appearance")]
        public string TextQuestionCssClass { get; set; }


        [Category("Appearance")]
        public string SelectQuestionCssClass { get; set; }


        [Category("Appearance")]
        public string RadioQuestionCssClass { get; set; }


        [Category("Appearance")]
        public string QuestionTitleCssClass { get; set; }


        [Category("Appearance")]
        public string TitleCssClass { get; set; }


        [Category("Appearance")]
        public string TestSubmitButtonCssClass { get; set; }


        [Category("Appearance")]
        public string TestContentCssClass { get; set; }
        #endregion

        #region Behavior
        [Category("Behavior")]
        public TestMode Mode { get; set; }

        #endregion

        #region Data
        [Category("Data")]
        public string Title { get; set; }

        [Category("Data")]
        public string SubmitButtonText { get; set; }

        [Category("Data")]
        public List<TestQuestion> Questions
        {
            get
            {
                if (questions == null)
                {
                    questions = new List<TestQuestion>();
                }
                return questions;
            }
        }

        #endregion

        #endregion

        #region Events

        public event EventHandler<TestSubmitEventArgs> Submit;
        protected virtual void OnSubmit(TestSubmitEventArgs e)
        {
            var handler = Submit;
            if (handler != null)
            {
                Submit(this, e);
            }
        }

        #endregion

        #region Public methods

        public void AddQuestion(TestQuestion question)
        {
            if (question == null)
            {
                throw new ArgumentNullException("question", "Missing object reference.");
            }
            Questions.Add(question);
        }

        public void AddQuestionsRange(IEnumerable<TestQuestion> questions)
        {
            foreach(var q in questions)
            {
                AddQuestion(q);
            }
        }

        #endregion

        #region Initializaition

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        protected override void CreateChildControls()
        {

            Controls.Clear();

            // Create test title
            title = new Label();
            title.CssClass = TitleCssClass;
            title.Text = Title;
            Controls.Add(title);

            // Create test questions
            testContent = new HtmlGenericControl("div");
            testContent.Attributes.Add("class", TestContentCssClass);
            questions.ForEach(el =>
            {
                if (!String.IsNullOrEmpty(QuestionCssClass))
                {
                    el.CssClass = QuestionCssClass;
                }                    
                if (!String.IsNullOrEmpty(TextQuestionCssClass) && el is TextTestQuestion)
                {
                    el.CssClass += " " + TextQuestionCssClass;
                }                    
                else if (!String.IsNullOrEmpty(RadioQuestionCssClass) && el is RadioTestQuestion)
                {
                    el.CssClass = " " + RadioQuestionCssClass;
                }                    
                else if (!String.IsNullOrEmpty(SelectQuestionCssClass) && el is SelectTestQuestion)
                {
                    el.CssClass = " " + SelectQuestionCssClass;
                }
                el.TitleCssClass = QuestionTitleCssClass;
                el.Mode = Mode;
                testContent.Controls.Add(el);
            });
            Controls.Add(testContent);

            // Create test submit button
            submit = new Button();
            submit.Text = SubmitButtonText;
            submit.CssClass = TestSubmitButtonCssClass;
            submit.Click += submit_Click;
            Controls.Add(submit);
        }



        #endregion

        #region Event handlers
        private void submit_Click(object sender, EventArgs e)
        {
            var results = new List<Pair<int, bool>>();
            questions.ForEach(el =>
                {
                    var res = new Pair<int, bool>
                    {
                        First = el.QuestionId,
                        Second = el.CheckAnswer()
                    };
                    results.Add(res);
                });
            OnSubmit(new TestSubmitEventArgs(results));
        }

        #endregion


        #region Rendering

        protected override void RenderChildren(HtmlTextWriter writer)
        {
            title.RenderControl(writer);

            testContent.RenderControl(writer);

            submit.RenderControl(writer);
        }

        #endregion
    }
}
