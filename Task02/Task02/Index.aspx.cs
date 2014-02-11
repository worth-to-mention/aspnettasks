using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace Task02
{
    public partial class Index : System.Web.UI.Page
    {
        public static int randomNumber;

        protected void Page_Load(object sender, EventArgs e)
        {
            captchaImage.ImageUrl = "image.h";
            captchaImage.AlternateText = "Captcha";
            captchaImage.DataBind();
        }

        protected void captchaSendButton_Click(object sender, EventArgs e)
        {
            int inputNumber;
            StringBuilder sb = new StringBuilder();
            sb.Append("<script>")
                .Append("window.onload = function(){")
                .Append("alert('");
            if (!Int32.TryParse(captchaTextBox.Text, out inputNumber) || inputNumber != randomNumber)
            {
                sb.Append("Wrong!");
            }
            else
            {
                sb.Append("Right!");
            }
            sb.Append("');")
                .Append("}")
                .Append("</script>");

            captchaTextBox.Text = String.Empty;

            Response.Write(sb.ToString());
        }
    }
}