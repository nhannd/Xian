#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.IO;
using System.Text;
using System.Web.Script.Services;
using System.Web.Services;
using System.Xml;

namespace ClearCanvas.ImageServer.Web.Application.Admin.Configuration.ServerRules
{
    /// <summary>
    /// Summary description for ServerRuleSamples
    /// </summary>
    [WebService(Namespace = "http://www.clearcanvas.ca/ImageServer/ServerRuleSamples.asmx")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class ServerRuleSamples : WebService
    {
        [WebMethod]
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
