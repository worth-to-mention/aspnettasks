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
                    schemaSet.Add(ns.ToString(), schemaPath);
                    XDocument testDoc = XDocument.Load(TestXmlFileUpload.FileName);
                    bool error = false;
                    testDoc.Validate(schemaSet, (s, args) =>
                        {
                            error = true;
                        });
                    if (!error)
                    {
                        SaveTest(testDoc);
                    }
                }
            }
            
        }

        private void SaveTest(XDocument testDoc)
        {
            var query = 
                from test in testDoc.Elements("Test")
                select new DataAccess.Test
                {
                    Title = (string)test.Element("Title"),
                    Questions = (
                        from question in test.Element("Questions").Elements("Question")
                        select new DataAccess.Question
                        {
                            Text = (string)question.Element("Text"),
                            Type = (DataAccess.QuestionType)Enum.Parse(typeof(DataAccess.QuestionType), (string)question.Attribute("type")),
                            Options = (
                                from option in question.Element("Options").Elements("Option")
                                select new DataAccess.Option
                                {
                                    Text = (string)option.Element("Text"),
                                    IsAnswer = (bool)option.Attribute("isanswer")
                                }
                            ).ToList()
                        }
                    ).ToList()
                };
            DataAccess.Test newTest = query.FirstOrDefault();
            if (newTest != null)
            {
                var config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~/");
                var conSettings = config.ConnectionStrings;
                if (conSettings.ConnectionStrings.Count < 0)
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