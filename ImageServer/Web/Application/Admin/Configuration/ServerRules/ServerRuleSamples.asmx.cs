using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Collections;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Xml;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules
{
    /// <summary>
    /// Summary description for ServerRuleSamples
    /// </summary>
    [WebService(Namespace = "http://www.clearcanvas.ca/ImageServer/ServerRuleSamples.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class ServerRuleSamples : System.Web.Services.WebService
    {
        [System.Web.Services.WebMethod]
        public string GetXml(string type)
        {
            string inputString = Server.HtmlEncode(type);
            if (String.IsNullOrEmpty(inputString))
                inputString = "";
    
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("rule");
            doc.AppendChild(node);
            XmlElement conditionNode = doc.CreateElement("condition");
            node.AppendChild(conditionNode);
            conditionNode.SetAttribute("expressionLanguage", "dicom");
            XmlNode actionNode = doc.CreateElement("action");
            node.AppendChild(actionNode);

            switch (inputString)
            {
                case "SimpleAutoRoute":
                    {
                        XmlElement regexNode = doc.CreateElement("regex");
                        regexNode.SetAttribute("test", "$StudyDescription");
                        regexNode.SetAttribute("pattern", "chest");
                        conditionNode.AppendChild(regexNode);

                        XmlElement autoRoute = doc.CreateElement("auto-route");
                        autoRoute.SetAttribute("device", "CLEARCANVAS");
                        actionNode.AppendChild(autoRoute);
                    }
                    break;
                case "MultiTagAutoRoute":
                    {
                        XmlElement orNode = doc.CreateElement("or");
                        conditionNode.AppendChild(orNode);
                        XmlElement equalNode = doc.CreateElement("equal");
                        equalNode.SetAttribute("test", "$Modality");
                        equalNode.SetAttribute("refValue", "MR");
                        orNode.AppendChild(equalNode);
                        equalNode = doc.CreateElement("equal");
                        equalNode.SetAttribute("test", "$Modality");
                        equalNode.SetAttribute("refValue", "CT");
                        orNode.AppendChild(equalNode);

                        XmlElement autoRoute = doc.CreateElement("auto-route");
                        autoRoute.SetAttribute("device", "CLEARCANVAS");
                        actionNode.AppendChild(autoRoute);
                    }
                    break;
                case "AgeBasedDelete":
                    {
                        XmlElement dicomAgeNode = doc.CreateElement("dicom-age-less-than");
                        dicomAgeNode.SetAttribute("test", "$PatientsBirthDate");
                        dicomAgeNode.SetAttribute("units", "years");
                        dicomAgeNode.SetAttribute("refValue", "21");
                        conditionNode.AppendChild(dicomAgeNode);
                        
                        XmlElement studyDelete = doc.CreateElement("study-delete");
                        studyDelete.SetAttribute("time", "21");
                        studyDelete.SetAttribute("timeUnits", "patientAge");
                        actionNode.AppendChild(studyDelete);
                    }
                    break;
                case "TagBasedDelete":
                    {
                        XmlElement andNode = doc.CreateElement("and");
                        conditionNode.AppendChild(andNode);
                        XmlElement equalNode = doc.CreateElement("equal");
                        equalNode.SetAttribute("test", "$Modality");
                        equalNode.SetAttribute("refValue", "MR");
                        andNode.AppendChild(equalNode);
                        equalNode = doc.CreateElement("regex");
                        equalNode.SetAttribute("test", "$PatientId");
                        equalNode.SetAttribute("pattern", "1");
                        andNode.AppendChild(equalNode);

                        XmlElement studyDelete = doc.CreateElement("study-delete");
                        studyDelete.SetAttribute("time", "10");
                        studyDelete.SetAttribute("timeUnits", "weeks");
                        actionNode.AppendChild(studyDelete);

                    }
                    break;
            }

            StringWriter sw = new StringWriter();

            XmlWriterSettings xmlSettings = new XmlWriterSettings();

            xmlSettings.Encoding = Encoding.UTF8;
            xmlSettings.ConformanceLevel = ConformanceLevel.Fragment;
            xmlSettings.Indent = true;
            xmlSettings.NewLineOnAttributes = true;
            xmlSettings.CheckCharacters = true;
            xmlSettings.IndentChars = "  ";

            XmlWriter tw = XmlWriter.Create(sw, xmlSettings);

            doc.WriteTo(tw);

            tw.Close();

            return sw.ToString();
        }

    }
}
