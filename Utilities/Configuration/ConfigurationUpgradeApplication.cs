#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Utilities.Configuration
{
	/// <summary>
	/// Class for performing "upgrades" of Configuration files.
	/// </summary>
	/// <remarks>
	/// This application utilizes the configUpgrade.xslt file/embedded resource
	/// to generate an upgraded configuration file from an "old" and a "new" one.  It essentially
	/// just merges them together, but it will only merge nodes that exist
	/// in the "new" configuration file.
	/// </remarks>
	[ExtensionOf(typeof(ApplicationRootExtensionPoint))]
	public class ConfigurationUpgradeApplication : IApplicationRoot
	{
		#region Public Methods
		public void RunApplication(string[] args)
		{
			ConfigurationUpgradeCommandLine cmdLine = new ConfigurationUpgradeCommandLine();
			try
			{
				cmdLine.Parse(args);

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

				Console.WriteLine("Upgrading configuration file {0} ", cmdLine.OldConfigFile);

				if (!UpgradeConfiguration(cmdLine))
					Environment.ExitCode = -1;
				else
				{
					Environment.ExitCode = 0;
				}
			}
			catch (CommandLineException e)
			{
				Console.WriteLine(e.Message);
				cmdLine.PrintUsage(Console.Out);
				Environment.ExitCode = -1;
			}
			catch (Exception e)
			{
				Console.WriteLine("Unexpected exception when upgrading configuration file: {0}", e.Message);
				Environment.ExitCode = -1;
			}
		}
		#endregion

		#region Private Static Methods

		private static bool UpgradeConfiguration(ConfigurationUpgradeCommandLine cmdLine)
		{
			try
			{
				XPathDocument sourceXPathDocument = new XPathDocument(cmdLine.NewConfigFile);
				XslCompiledTransform theXsltTransform = new XslCompiledTransform();

				Stream xsltStream = new ResourceResolver(typeof(ConfigurationUpgradeApplication).Assembly).
										OpenResource("ConfigurationUpgrade.xslt");
				//Load the XslTransform
				theXsltTransform.Load(XmlReader.Create(xsltStream),
				                    new XsltSettings { EnableDocumentFunction = true }, null);

				//Set the formatting conventions for the output
				XmlWriterSettings xmlSettings = new XmlWriterSettings
				                                	{
				                                		Encoding = Encoding.UTF8,
				                                		ConformanceLevel = ConformanceLevel.Fragment,
				                                		Indent = true,
				                                		NewLineOnAttributes = false,
				                                		CheckCharacters = true,
				                                		IndentChars = "  "
				                                	};
				MemoryStream ms = new MemoryStream();
				XmlWriter xmlWriter = XmlWriter.Create(ms, xmlSettings);
				if (xmlWriter == null)
				{
					Console.WriteLine("Unable to create XML Writer");
					return false;
				}

				XsltArgumentList args = new XsltArgumentList();
				args.AddParam("oldDocumentName", string.Empty, cmdLine.OldConfigFile);
				if (!String.IsNullOrEmpty(cmdLine.ExceptionsFile))
					args.AddParam("exceptionsDocumentName", string.Empty, cmdLine.ExceptionsFile);
				args.AddParam("outputDebugComments", string.Empty, cmdLine.OutputDebugComments.ToString());


				// Do the transform
				theXsltTransform.Transform(sourceXPathDocument, args, xmlWriter);

				// Save the output file
				ms.Seek(0, SeekOrigin.Begin);
				using (FileStream writer = new FileStream(cmdLine.OutputConfigFile, FileMode.Create))
				{
					byte[] buffer = ms.GetBuffer();
					writer.Write(buffer, 0, (int)ms.Length);
					writer.Flush();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Unexpected Exception when executing configuration upgrade XSLT : {0}", e.Message);
				Console.WriteLine("Call stack : {0}", e.StackTrace);
				return false;
			}

			return true;
		}

		#endregion
	}
}