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
    /// Represents test question with text field for the answer.
    /// </summary>
    public class TextTestQuestion : TestQuestion
    {
        private readonly TextBox textInput;

        /// <summary>
        /// Constructs an instance of TextTestQuestion class.
        /// </summary>
        public TextTestQuestion()
        {
            textInput = new TextBox();
            textInput.Style.Add(HtmlTextWriterStyle.Display, "block");
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
        
        
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Controls.Add(textInput);
        }
    }
}
