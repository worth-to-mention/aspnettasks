using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.Web.Security;

namespace Maleficus.CustomControls
{
    /// <summary>
    /// Captcha control.
    /// </summary>
    public class Captcha : CompositeControl
    {
        public Captcha() { }

        #region Child controls

        private Image captchaImage;
        private TextBox captchaTextBox;

        #endregion

        private string DefinedKey
        {
            get
            {
                string tmp = Page.Session["DefinedKey"] as String;
                return tmp == null ? String.Empty : tmp;
            }
            set
            {
                Page.Session["DefinedKey"] = value;
            }
        }
        private string UserKey
        {
            get
            {
                EnsureChildControls();
                return captchaTextBox.Text;
            }
        }

        #region Control properties
        /// <summary>
        /// Gets or sets width of captcha image.
        /// </summary>
        public int ImageWidth
        {
            get
            {
                EnsureChildControls();
                int width = (int)captchaImage.Width.Value;
                return width <= 0 ? 100 : width;
            }
            set
            {
                EnsureChildControls();
                captchaImage.Width = value;
            }
        }
        /// <summary>
        /// Gets or sets height of captcha image.
        /// </summary>
        public int ImageHeight
        {
            get
            {
                EnsureChildControls();
                int height = (int)captchaImage.Height.Value;
                return height <= 0 ? 100 : height;
            }
            set
            {
                EnsureChildControls();
                captchaImage.Height = value;
            }
        }

        #endregion
        /// <summary>
        /// Checks whether user entered right captcha text or not.
        /// </summary>
        /// <returns>True if user entered text equals captcha text.</returns>
        public bool CheckValidity()
        {
            return String.Equals(DefinedKey.ToUpper(), UserKey.ToUpper(), StringComparison.Ordinal);
        }


        protected override void CreateChildControls()
        {
            Controls.Clear();

            captchaImage = new Image();
            captchaImage.ID = "CaptchaImage";
            captchaImage.AlternateText = "Captcha image should be here.";

            captchaTextBox = new TextBox();
            captchaTextBox.ID = "CaptchaTextBox";

            Controls.Add(captchaImage);
            Controls.Add(captchaTextBox);

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);


        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            int imageWidth = ImageWidth;
            int imageHeight = ImageHeight;

            string tmp;
            byte[] image = CaptchaGenerator.Generate(imageWidth, imageHeight, ImageFormat.Png, out tmp, null).ToArray();
            DefinedKey = tmp;

            captchaImage.ImageUrl = String.Format("data:image/png;base64,{0}", Convert.ToBase64String(image));
            captchaTextBox.Text = String.Empty;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div class=\"captcha\">");


            writer.Write("<div class=\"captcha-image\">");
            captchaImage.RenderControl(writer);
            writer.WriteEndTag("div");

            writer.Write("<div class=\"captcha-text\">");
            captchaTextBox.RenderControl(writer);
            writer.WriteEndTag("div");
            
            writer.WriteEndTag("div");
        }


    }
}
