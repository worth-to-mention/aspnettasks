using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestQuestionControls
{
    public abstract class TestQuestion : WebControl, INamingContainer
    {
        private Label titleLabel; 

        public TestQuestion()
        {
        }
        
        public int QuestionId { get; set; }

        #region Control attributes
        /// <summary>
        /// Backing field for Title. 
        /// </summary>
        protected string TestTitle;
        /// <summary>
        /// Gets or sets a title of the test question.
        /// </summary>
        public virtual string Title
        {
            get
            {
                return TestTitle;
            }
            set
            {
                TestTitle = value;
            }
        }

        #endregion

        public abstract bool CheckAnswer();
        
        protected override void CreateChildControls()
        {
            Controls.Clear();

            titleLabel = new Label();
            titleLabel.Text = TestTitle;

            Controls.Add(titleLabel);
        }
        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteBeginTag("div");
            writer.WriteAttribute("class", CssClass);
            writer.Write(">");
            foreach(Control child in Controls)
            {
                child.RenderControl(writer);
            }
            writer.WriteEndTag("div");
        }
    }
}
