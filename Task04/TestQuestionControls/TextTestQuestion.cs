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
    public class TextTestQuestion : TestQuestion
    {
        private readonly TextBox textInput;

        public TextTestQuestion()
        {
            textInput = new TextBox();
            textInput.Style.Add(HtmlTextWriterStyle.Display, "block");
        }

        public string Answer { get; set; }


        public override bool CheckAnswer()
        {
            return String.Equals(textInput.Text, Answer, StringComparison.CurrentCultureIgnoreCase);
        }
        
        
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Controls.Add(textInput);
        }
    }
}
