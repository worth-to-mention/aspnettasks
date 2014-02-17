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
    /// <summary>
    /// Base class for test questions. Contains only title
    /// of a question.
    /// </summary>
    public abstract class TestQuestion : WebControl, INamingContainer
    {
        private Label titleLabel; 

        /// <summary>
        /// Get or sets question id.
        /// </summary>
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

        /// <summary>
        /// Checks whether answer for the question is right
        /// or not
        /// </summary>
        /// <returns>True if answer is right.</returns>
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
