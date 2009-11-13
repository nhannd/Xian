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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Utilities.Configuration
{
	/// <summary>
	/// Class for generating transform style sheets based on two versions of an app.config file.
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class GenerateXslApplication : IApplicationRoot
	{
		#region Public Methods
		public void RunApplication(string[] args)
		{
			GenerateXslCommandLine cmdLine = new GenerateXslCommandLine();
			try
			{
				cmdLine.Parse(args);

				if (string.IsNullOrEmpty(cmdLine.OutputXslFile)
						|| string.IsNullOrEmpty(cmdLine.NewConfigFile)
						|| string.IsNullOrEmpty(cmdLine.OldConfigFile))
				{
					Console.WriteLine("Command line options incorrect.");
					cmdLine.PrintUsage(Console.Out);
					Environment.ExitCode = -1;
					return;
				}

				if (!File.Exists(cmdLine.NewConfigFile))
				{
					Console.WriteLine("NewConfigFile configuration file not found: {0}", cmdLine.NewConfigFile);
					Environment.ExitCode = -1;
					return;
				}

				if (!File.Exists(cmdLine.OldConfigFile))
				{
					Console.WriteLine("OldConfigFile configuration file not found: {0}", cmdLine.NewConfigFile);
					Environment.ExitCode = -1;
					return;
				}

				GenerateXsl(cmdLine);
		
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdLine.PrintUsage(Console.Out);
				Environment.ExitCode = -1;
			}
			catch (Exception e)
			{
				Console.WriteLine("Unexpected exception when generating XSL: {0}", e.Message);
				Environment.ExitCode = -1;
			}
		}
		#endregion

		#region Private Static Methods

		private static bool GenerateXsl( GenerateXslCommandLine cmdLine)
		{
		
			XmlDocument newDoc = new XmlDocument();
			newDoc.Load(cmdLine.NewConfigFile);

			XmlDocument oldDoc = new XmlDocument();
			oldDoc.Load(cmdLine.OldConfigFile);

			XmlElement templateElement;
			XmlDocument xslt = CreateXsltTemplate(out templateElement);

			XmlElement configElement = xslt.CreateElement("configuration");
			templateElement.AppendChild(configElement);

			XmlNode childNode = newDoc.FirstChild;
			if (!childNode.Name.Equals("configuration"))
			{
				Console.WriteLine("Uncorrect node root node: {0}", childNode.Name);
				return false;
			}
			childNode = childNode.FirstChild;

			while (childNode != null)
			{
				// Just search for the first study node, parse it, then break
				if (childNode.Name.Equals("configSections")
					|| childNode.Name.Equals("runtime"))
				{
					XmlNode node = xslt.ImportNode(childNode, true);
					configElement.AppendChild(node);
				}
				else if (childNode.Name.Equals("connectionStrings"))
				{
					XmlNode node = xslt.ImportNode(childNode, true);
					configElement.AppendChild(node);
				}
				else if (childNode.Name.Equals("applicationSettings"))
				{
					XmlElement applicationSettingsElement = xslt.CreateElement("applicationSettings");
					configElement.AppendChild(applicationSettingsElement);
					CreateXsltApplicationSettingsSection(childNode, oldDoc, applicationSettingsElement);
				}
				else if (childNode.Name.Equals("system.serviceModel"))
				{
					XmlNode node = xslt.ImportNode(childNode, true);
					configElement.AppendChild(node);
				}
				else if (childNode.Name.EndsWith("Settings"))
				{
					CreateXsltCustomSetting(childNode, oldDoc, configElement);
				}
				else
				{
					XmlNode node = xslt.ImportNode(childNode, true);
					configElement.AppendChild(node);
				}

				childNode = childNode.NextSibling;
			}

			// Write the file.
			using (FileStream fs = new FileStream(cmdLine.OutputXslFile, FileMode.Create))
			{
				XmlWriterSettings xmlSettings = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					ConformanceLevel = ConformanceLevel.Fragment,
					Indent = true,
					NewLineOnAttributes = false,
					CheckCharacters = true,
					IndentChars = "  "
				};

				XmlWriter writer = XmlWriter.Create(fs, xmlSettings);
				if (writer == null)
				{
					Console.WriteLine("Unable to create XML Writer");
					return false;
				}

				xslt.WriteTo(writer);
				writer.Flush();
				fs.Flush();
				writer.Close();
				fs.Close();
			}
			return true;
		}

		public static XmlDocument CreateXsltTemplate(out XmlElement templateElement)
		{
			XmlDocument outputXslt = new XmlDocument();

			XmlElement stylesheetElement = outputXslt.CreateElement("xsl","stylesheet", "http://www.w3.org/1999/XSL/Transform");
			
			XmlAttribute attrib = outputXslt.CreateAttribute("version");
			attrib.Value = "1.0";
			stylesheetElement.Attributes.Append(attrib);

			//attrib = outputXslt.CreateAttribute("xmlns:xsl");
			//attrib.Value = "http://www.w3.org/1999/XSL/Transform";
			//stylesheetElement.Attributes.Append(attrib);
			outputXslt.AppendChild(stylesheetElement);

			XmlElement element = outputXslt.CreateElement("xsl", "param", "http://www.w3.org/1999/XSL/Transform");
			attrib = outputXslt.CreateAttribute("name");
			attrib.Value = "current";
			element.Attributes.Append(attrib);
			stylesheetElement.AppendChild(element);

			templateElement = outputXslt.CreateElement("xsl", "template", "http://www.w3.org/1999/XSL/Transform");
			attrib = outputXslt.CreateAttribute("match");
			attrib.Value = "/";
			templateElement.Attributes.Append(attrib);
			stylesheetElement.AppendChild(templateElement);

			return outputXslt;
		}

		/// <summary>
		/// Create Xslt document section for the configuration/applicationSettings section of an app.config file that
		/// duplicates new settings into the Xslt document and copies from the old configuration file values that existed in
		/// the old configuration file.
		/// </summary>
		/// <param name="refAppSettingsNode">The reference (new) config files applicationSettings node.</param>
		/// <param name="oldConfig">The old configuration file.</param>
		/// <param name="xsltAppSettingsElement">The XSLT document's applicationSettings node.</param>
		public static void CreateXsltApplicationSettingsSection(XmlNode refAppSettingsNode, XmlDocument oldConfig, XmlElement xsltAppSettingsElement)
		{
			XmlDocument xsltDoc = xsltAppSettingsElement.OwnerDocument;
			XmlNode refSectionNode = refAppSettingsNode.FirstChild;

			while (refSectionNode != null)
			{
				// Just search for the first study node, parse it, then break
				string refSettingsSectionName = refSectionNode.Name;


				XmlNode xsltSectionNode = xsltDoc.ImportNode(refSectionNode, false);
				xsltAppSettingsElement.AppendChild(xsltSectionNode);

				if (refSettingsSectionName.Equals("ClearCanvas.Common.ProductSettings"))
				{
					XmlElement element = xsltDoc.CreateElement("xsl", "copy-of", "http://www.w3.org/1999/XSL/Transform");
					XmlAttribute attrib = xsltDoc.CreateAttribute("select");
					attrib.Value = string.Format("document($current)/configuration/applicationSettings/{0}/setting", refSettingsSectionName);
					element.Attributes.Append(attrib);
					xsltSectionNode.AppendChild(element);

					refSectionNode = refSectionNode.NextSibling;
					continue;
				}

				XmlNode refSettingNode = refSectionNode.FirstChild;
				while (refSettingNode != null)
				{
					if (refSettingNode is XmlComment)
					{
						xsltSectionNode.AppendChild(xsltDoc.ImportNode(refSettingNode, true));
						refSettingNode = refSettingNode.NextSibling;
						continue;
					}

					string val = refSettingNode.Attributes["name"].Value;

					XmlNodeList nodeList = oldConfig.SelectNodes(String.Format("/configuration/applicationSettings/{0}/setting[@name='{1}']",
					                                                           refSettingsSectionName, val));
					if (nodeList != null && nodeList.Count > 0)
					{
						XmlElement element = xsltDoc.CreateElement("xsl", "copy-of", "http://www.w3.org/1999/XSL/Transform");
						XmlAttribute attrib = xsltDoc.CreateAttribute("select");
						attrib.Value = string.Format("configuration/applicationSettings/{0}/setting[@name='{1}']",
						                             refSettingsSectionName, val);
						element.Attributes.Append(attrib);
						xsltSectionNode.AppendChild(element);				
					}
					else
					{
						xsltSectionNode.AppendChild(xsltDoc.ImportNode(refSettingNode,true));
					}

					refSettingNode = refSettingNode.NextSibling;
				}
				refSectionNode = refSectionNode.NextSibling;
			}

		}

		/// <summary>
		/// Create Xslt document section for a custom setting node.
		/// </summary>
		/// <param name="refSettingNode">The reference (new) config files applicationSettings node.</param>
		/// <param name="oldConfig">The old configuration file.</param>
		/// <param name="xsltConfigureNode">The XSLT document's applicationSettings node.</param>
		public static void CreateXsltCustomSetting(XmlNode refSettingNode, XmlDocument oldConfig, XmlElement xsltConfigureNode)
		{
			XmlDocument xsltDoc = xsltConfigureNode.OwnerDocument;

			XmlNodeList nodeList = oldConfig.SelectNodes(String.Format("/configuration/{0}",
														   refSettingNode.Name));
			if (nodeList != null && nodeList.Count > 0)
			{
				XmlElement element = xsltDoc.CreateElement(refSettingNode.Name);
				xsltConfigureNode.AppendChild(element);

				foreach (XmlAttribute attrib in refSettingNode.Attributes)
				{
					XmlNodeList attribNodeList = oldConfig.SelectNodes(String.Format("/configuration/{0}/@{1}",
																   refSettingNode.Name, attrib.Name));

					if (attribNodeList != null && attribNodeList.Count > 0)
					{
						
						XmlElement xslElement = xsltDoc.CreateElement("xsl", "attribute", "http://www.w3.org/1999/XSL/Transform");
						element.AppendChild(xslElement);
						XmlAttribute xslAttrib = xsltDoc.CreateAttribute("name");
						xslAttrib.Value = attrib.Name;
						xslElement.Attributes.Append(xslAttrib);

						XmlElement valueOf = xsltDoc.CreateElement("xsl", "value-of", "http://www.w3.org/1999/XSL/Transform");
						XmlAttribute valueOfAttrib = xsltDoc.CreateAttribute("select");
						valueOfAttrib.Value = string.Format("configuration/{0}/@{1}", refSettingNode.Name, attrib.Name);
						valueOf.Attributes.Append(valueOfAttrib);
						xslElement.AppendChild(valueOf);
					}
					else
					{
						element.Attributes.Append(attrib.Clone() as XmlAttribute);
					}
				}
			}
			else
			{
				XmlNode xsltSectionNode = xsltDoc.ImportNode(refSettingNode, true);
				xsltConfigureNode.AppendChild(xsltSectionNode);
			}
		}
		#endregion
	}
}
