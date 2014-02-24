using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

using TestingSystem.DataAccess;

namespace TestingSystem.Administrative
{
    public partial class CreateTest : System.Web.UI.Page
    {
        private string schemaPath;
        private XNamespace ns = "http://maleficus.com/Test";
        public CreateTest()
        {
            schemaPath = Server.MapPath("~/source/Tests.xsd");
        }
        protected void UploadButton_Click(object sender, EventArgs e)
        {
            if (TestXmlFileUpload.HasFile)
            {
                if (File.Exists(schemaPath))
                {
                    XmlSchemaSet schemaSet = new XmlSchemaSet();
                    schemaSet.Add("http://maleficus.com/Test", schemaPath);
                    XDocument testDoc = XDocument.Load(TestXmlFileUpload.FileContent);
                    bool error = false;
                    testDoc.Validate(schemaSet, (s, args) =>
                        {
                            error = true;
                        });
                    if (!error)
                    {
                        try
                        {
                            SaveTest(testDoc);
                            UploadButtonLabel.Text = "Test has been successfully uploaded.";
                        }
                        catch(Exception)
                        {
                            UploadButtonLabel.Text = "File cannot be uploaded now, please try again later.";
                        }
                        
                        
                    }
                    else
                    {
                        UploadButtonLabel.Text = "Your xml file has invalid content.";
                    }
                }
            }
            else 
            {
                UploadButtonLabel.Text = "You must specify a xml file with test data.";
            }
            
        }

        private void SaveTest(XDocument testDoc)
        {
            var query = 
                from question in testDoc.Root.Element(ns + "Questions").Elements(ns + "Question")
                select new DataAccess.Question
                {
                    Text = (string)question.Element(ns + "Text"),
                    Type = (DataAccess.QuestionType)Enum.Parse(typeof(DataAccess.QuestionType), (string)question.Attribute("type")),
                    Options = (
                        from option in question.Element(ns + "Options").Elements(ns + "Option")
                        select new DataAccess.Option
                        {
                            Text = (string)option,
                            IsAnswer = (bool)option.Attribute("isanswer")
                        }
                    ).ToList()
                };
            DataAccess.Test newTest = new DataAccess.Test
            {
                Title = (string)testDoc.Root.Element(ns + "Title"),
                Questions = query.ToList()
            };
            if (newTest != null)
            {
                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
                var conSettings = config.ConnectionStrings;
                if (conSettings.ConnectionStrings.Count > 0)
                {
                    string connectionString = conSettings.ConnectionStrings["TestingSystem"].ConnectionString;
                
                    using(var context = new TestingSystemDataContext(connectionString))
                    {
                        context.CreateTest(newTest);
                    }
                }
            }


        }
    }
}