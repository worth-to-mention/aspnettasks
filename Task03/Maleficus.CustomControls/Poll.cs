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
    public class Poll : CompositeControl, IPostBackDataHandler
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
                var tmp = ViewState["Voted"];
                return tmp == null ? false : (bool)tmp;
            }
            set
            {
                ViewState["Voted"] = value;
            }
        }

        private string sourceFile;
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

            if (!File.Exists(SourceFile))
                return;

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
                if (Page.Request.Params["pollSubmit"] != null && Page.Request.Params["poll"] != null)
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
                    writer.Write(sb.ToString());
                }
                captchaControl.RenderControl(writer);
                writer.Write("<input name=\"pollSubmit\" type=\"submit\" value=\"Send\" runat=\"server\" />");
            }
            else
            {
                foreach (var pair in options)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div class=\"poll-results\"> ")
                        .Append("<div class=\"poll-result\">")
                        .AppendFormat("<div class=\"poll-option-title\">{0}</div>", pair.Key)
                        .AppendFormat("<div class=\"poll-option-votes\">{0}</div>", pair.Value.ToString())
                        .Append("</div>")
                        .Append("</div>");
                    writer.Write(sb.ToString());
                }
            }
            
        }


      

        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            var postData = postCollection[postDataKey];
            
            return true;
        }

        public void RaisePostDataChangedEvent()
        {
            
        }
    }
}
