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
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Utilities.Configuration
{
	/// <summary>
	/// Class for performing upgrades of Configuration files
	/// </summary>
	/// <remarks>
	/// The class searches for plugins that implement the <see cref="IConfigurationUpgradeXslt"/>
	/// interface to perform XSLT translations to the configuration files.
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

				Version assemblyVersion = LoadAssemblyVersion();
				Version sourceVersion = new Version(cmdLine.Version);

				if (cmdLine.Check)
				{
					CheckConfigVersion(sourceVersion, assemblyVersion, cmdLine);
					return;
				}

				if (string.IsNullOrEmpty(cmdLine.ConfigurationFile)
					|| string.IsNullOrEmpty(cmdLine.NewConfigFile)
					|| string.IsNullOrEmpty(cmdLine.OutputConfigFile) 
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

				Console.WriteLine("The source version of {0} is {1} and assembly version is {2}",
								  cmdLine.ConfigurationFile, sourceVersion.ToString(4),
								  assemblyVersion.ToString(4));

				if (sourceVersion.Equals(assemblyVersion))
				{
					Console.WriteLine("Configuration version is up-to-date.");
					Environment.ExitCode = 0;
					return;
				}

				if (!UpgradeConfiguration(sourceVersion, assemblyVersion, cmdLine))
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
		private static void CheckConfigVersion(Version databaseVersion, Version assemblyVersion, ConfigurationUpgradeCommandLine cmdLine)
		{
			if (databaseVersion.Equals(assemblyVersion))
			{
				Console.WriteLine("The database is up-to-date.");
				Environment.ExitCode = 0;
				return;
			}

			ConfigurationTransformExtensionPoint ep = new ConfigurationTransformExtensionPoint();
			object[] extensions = ep.CreateExtensions();

			bool found = false;
			foreach (IConfigurationTransform script in extensions)
			{
				if (script.SourceVersion.Equals(databaseVersion) && script.ConfigurationFile.Equals(cmdLine.ConfigurationFile))
				{
					found = true;
					break;
				}
			}

			if (found)
			{
				Console.WriteLine("The configuration must be upgraded from {0} to {1}", databaseVersion.ToString(4),
								  assemblyVersion.ToString(4));
				Environment.ExitCode = 1;
			}
			else
			{
				Console.WriteLine("An unsupported version ({0}) is installed which cannot be upgraded from", databaseVersion.ToString(4));
				Environment.ExitCode = -1;
			}
		}

		private static bool UpgradeConfiguration(Version startingVersion, Version assemblyVersion, ConfigurationUpgradeCommandLine cmdLine)
		{
			Version currentVersion = startingVersion;

			ConfigurationTransformExtensionPoint ep = new ConfigurationTransformExtensionPoint();
			object[] extensions = ep.CreateExtensions();

			XPathDocument sourceXPathDocument = new XPathDocument(cmdLine.OldConfigFile);
			MemoryStream savedStream = null;

			while (!currentVersion.Equals(assemblyVersion))
			{
				bool found = false;
				foreach (IConfigurationTransform script in extensions)
				{
					if (!script.SourceVersion.Equals(currentVersion)
					    || !script.ConfigurationFile.Equals(cmdLine.ConfigurationFile))
						continue;

					try
					{
						using (Stream xslStream = script.GetStream())
						{
							XPathDocument myXPathDocument = sourceXPathDocument;
							XslCompiledTransform myXslTransform = new XslCompiledTransform();

							//Load the XslTransform
							myXslTransform.Load(XmlReader.Create(xslStream),
							                    new XsltSettings
							                    	{
							                    		EnableDocumentFunction = true
							                    	},
							                    null);

							//
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
							XmlWriter writer = XmlWriter.Create(ms, xmlSettings);
							if (writer == null)
							{
								Console.WriteLine("Unable to create XML Writer");
								return false;
							}

							XsltArgumentList args = new XsltArgumentList();
							args.AddParam("current", string.Empty, "file://" + cmdLine.NewConfigFile);

							myXslTransform.Transform(myXPathDocument, args, writer);

							ms.Seek(0, SeekOrigin.Begin);

							savedStream = ms;

							StreamReader stream = new StreamReader(ms);

							sourceXPathDocument = new XPathDocument(XmlReader.Create(stream));
						}
					}
					catch (Exception e)
					{
						Console.WriteLine("Unexpected Exception when executing upgrade XSLT :");
						Console.WriteLine("Call stack : {0}", e.StackTrace);
						return false;
					}
					Console.WriteLine("The database has been upgraded from version {0} to version {1}", currentVersion.ToString(4),
					                  script.DestinationVersion.ToString(4));
					currentVersion = script.DestinationVersion;

					found = true;
					break;
				}

				if (!found)
				{
					Console.WriteLine("Unable to find upgrade script for {0}", currentVersion.ToString(4));
					return false;
				}
			}

			if (savedStream != null)
			{
				savedStream.Seek(0, SeekOrigin.Begin);

				using (FileStream writer = new FileStream(cmdLine.OutputConfigFile, FileMode.Create))
				{
					byte[] buffer = savedStream.GetBuffer();
					writer.Write(buffer, 0, (int) savedStream.Length);
					writer.Flush();
				}
			}
			return true;
		}

		private static Version LoadAssemblyVersion()
		{
			return Assembly.GetExecutingAssembly().GetName().Version;
		}
		#endregion
	}
}
