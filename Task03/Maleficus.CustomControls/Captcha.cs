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
    public class Captcha : WebControl
    {
        public Captcha() : base(HtmlTextWriterTag.Div) { }

        #region Child controls

        private Image captchaImage;
        private TextBox captchaTextBox;

        #endregion


        #region Control properties

        public string DefinedKey
        {
            get
            {
                string tmp = ViewState["DefinedKey"] as String;
                return tmp == null ? String.Empty : tmp;
            }
            set
            {
                ViewState["DefinedKey"] = value;
            }
        }
        public string UserKey
        {
            get
            {
                EnsureChildControls();
                return captchaTextBox.Text;
            }
        }

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
        protected override void RenderContents(HtmlTextWriter writer)
        {

            
            base.RenderContents(writer);
        }


    }
}
