using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TestingSystem.Controls
{
    [
        ParseChildren(true, "Text"),
        ToolboxData("<{0}:SelectTestQuestion runat=\"server\"> </{0}:SelectTestQuestion>")
    ]
    /// <summary>
    /// Represents test question with text field for the answer.
    /// </summary>
    public class TextTestQuestion : TestQuestion
    {
        private TextBox textInput;

        [
            PersistenceMode(PersistenceMode.InnerDefaultProperty)
        ]
        public string Text
        {
            get
            {
                EnsureChildControls();
                return textInput.Text;
            }
            set
            {
                EnsureChildControls();
                textInput.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets answer for the question.
        /// </summary>
        public string Answer { get; set; }

        /// <summary>
        /// Check whether user input answer is right or not.
        /// </summary>
        /// <returns>True if user answer is right.</returns>
        public override bool CheckAnswer()
        {
            string inputText = textInput.Text.Trim();
            if (String.IsNullOrEmpty(inputText))
                return false;            
            return String.Equals(inputText, Answer, StringComparison.CurrentCultureIgnoreCase);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EnsureChildControls();
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            
            textInput = new TextBox();
            textInput.Style.Add(HtmlTextWriterStyle.Display, "block");
            
            Controls.Add(textInput);
        }
    }
}
