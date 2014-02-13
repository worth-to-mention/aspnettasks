using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Maleficus.CustomControls
{
    /// <summary>
    /// Poll control.
    /// </summary>
    public class Poll : CompositeControl
    {
        private readonly Dictionary<string, int> options;

        private Captcha captchaControl;

        public Poll()
        {
            options = new Dictionary<string, int>();
        }

        private bool Voted
        {
            get
            {
                var tmp = Page.Session["Voted"];
                return tmp == null ? false : (bool)tmp;
            }
            set
            {
                Page.Session["Voted"] = value;
            }
        }

        private string sourceFile;
        /// <summary>
        /// Gets or sets xml source file for a poll.
        /// File path mast be virtual server path.
        /// </summary>
        public string SourceFile
        {
            get { return sourceFile; }
            set
            {
                string tmp = HttpContext.Current.Server.MapPath(value);
                sourceFile = tmp;
            }
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();

            captchaControl = new Captcha();
            captchaControl.ImageWidth = 200;
            captchaControl.ImageHeight = 100;

            Controls.Add(captchaControl);
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EnsureChildControls();

            if (!File.Exists(SourceFile))
            {
                Page.Response.Write("No data to display.");
                return;
            }

            XElement xml = XElement.Load(SourceFile);
            var query = from option in xml.Element("Options").Elements("Option")
                        select new
                        {
                            Title = option.Element("Title").Value,
                            Votes = option.Element("Votes").Value
                        };
            foreach (var el in query)
            {
                int votes = 0;
                Int32.TryParse(el.Votes, out votes);

                options.Add(el.Title, votes);
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack)
            {
                if (Page.Request.Params["pollSubmit"] != null && Page.Request.Params["poll"] != null && !Voted)
                {
                    if (captchaControl.CheckValidity())
                    {
                        string val = Page.Request.Params["poll"];
                        int hash;
                        if (Int32.TryParse(val, out hash))
                        {
                            var selectedKey = options.Keys.Where((key) => key.GetHashCode() == hash)
                                .FirstOrDefault();
                            ++options[selectedKey];
                            Voted = true;
                            try
                            {                                
                                XElement xml = XElement.Load(SourceFile);
                                XElement option = xml.Element("Options").Elements("Option").Where(el => el.Element("Title").Value == selectedKey).FirstOrDefault();
                                option.Element("Votes").Value = options[selectedKey].ToString();
                                xml.Save(SourceFile);
                            }
                            catch (Exception)
                            {
                                Voted = false;
                                Page.Response.Write("Your vote cannot be accepted at this time. Try again later.");
                            }
                        }
                    }
                    else
                    {
                        Page.Response.Write("Captcha validation failed. Try again.");
                    }
                }
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write("<div class=\"poll\">");

            if (!Voted)
            {
                foreach (var pair in options)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<input ")
                        .Append("type=\"radio\" ")
                        .Append("name=\"poll\" ")
                        .AppendFormat("value=\"{0}\" ", pair.Key.GetHashCode().ToString())
                        .Append(">")
                        .Append(pair.Key.ToString())
                        .Append("</input>");
                    writer.Write("<div class=\"poll-vote\">{0}</div>", sb.ToString());
                }
                captchaControl.RenderControl(writer);
                writer.Write("<input name=\"pollSubmit\" type=\"submit\" value=\"Send\" runat=\"server\" />");
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class=\"poll-results\"> ");
                foreach (var pair in options)
                {
                    sb.Append("<div class=\"poll-result\">")
                        .AppendFormat("<div class=\"poll-option-title\">{0}</div>", pair.Key)
                        .AppendFormat("<div class=\"poll-option-votes\">{0}</div>", pair.Value.ToString())
                        .Append("</div>");
                }
                sb.Append("</div>");
                writer.Write(sb.ToString());
            }

            writer.WriteEndTag("div");
            
        }

    }
}
