#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Configure.ServerRules
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
            XmlDocument doc = null;

            string inputString = Server.HtmlEncode(type);
            if (String.IsNullOrEmpty(inputString))
                inputString = string.Empty;

            var ep = new SampleRuleExtensionPoint();
            object[] extensions = ep.CreateExtensions();

            foreach (ISampleRule extension in extensions)
            {
                if (extension.Name.Equals(inputString))
                {
                    doc = extension.Rule;
                    break;
                }
            }

            if (doc == null)
            {
                doc = new XmlDocument();
                XmlNode node = doc.CreateElement("rule");
                doc.AppendChild(node);
                XmlElement conditionNode = doc.CreateElement("condition");
                node.AppendChild(conditionNode);
                conditionNode.SetAttribute("expressionLanguage", "dicom");
                XmlNode actionNode = doc.CreateElement("action");
                node.AppendChild(actionNode);
            }

            var sw = new StringWriter();

            var xmlSettings = new XmlWriterSettings
                                  {
                                      Encoding = Encoding.UTF8,
                                      ConformanceLevel = ConformanceLevel.Fragment,
                                      Indent = true,
                                      NewLineOnAttributes = true,
                                      CheckCharacters = true,
                                      IndentChars = "  "
                                  };

            XmlWriter tw = XmlWriter.Create(sw, xmlSettings);

            if (tw != null)
            {
                doc.WriteTo(tw);
                tw.Close();
            }

            return sw.ToString();
        }
    }
}