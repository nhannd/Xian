#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClickOncePublisher.Properties;
using System.IO;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using System.Xml;

namespace ClickOncePublisher
{
	public class Publisher
	{
		private ProductProfile _profile;
		private string _msbuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

		public Publisher(ProductProfile profile)
		{
			_profile = profile;
		}

		public void Publish()
		{
			// Delete the previously published product/version so we can start fresh
			if (Directory.Exists(PublishProductVersionPath))
			{
				Directory.Delete(PublishProductVersionPath, true);
			}

			Directory.CreateDirectory(PublishProductVersionPath);

			ApplicationManifest appManifest = CreateApplicationManifest();

			DeployManifest deployManifest = CreateDeploymentManifest(appManifest);
			CreateBootstrapper(deployManifest);
		}

		private string PublishProductPath
		{
			get
			{
				string str = String.Format("{0}\\{1}", Settings.Default.PublishDirectory, _profile.Name);
				return str.Replace(' ', '_');
			}
		}

		private string PublishProductVersionPath
		{
			get
			{
				string str = String.Format("{0}\\{1}", PublishProductPath, _profile.Version);
				return str.Replace(' ', '_');
			}
		}

		private ApplicationManifest CreateApplicationManifest()
		{
			string appManifestFile = _profile.Name + ".manifest";
			appManifestFile = appManifestFile.Replace(' ', '_');
			string appManifestPath = Path.Combine(PublishProductVersionPath, appManifestFile);

			ApplicationManifest appManifest = new ApplicationManifest();
			appManifest.SourcePath = appManifestPath;

			// Set the entry point
			appManifest.EntryPoint = new AssemblyReference(_profile.EntryPointPath);
			AssemblyIdentity identity = AssemblyIdentity.FromFile(_profile.EntryPointPath);
			appManifest.EntryPoint.AssemblyIdentity = identity;

			if (identity == null)
				throw new Exception("Entry point must be a .NET executable.");

			appManifest.IconFile = "app.ico";
			appManifest.TrustInfo = new TrustInfo();
			appManifest.TrustInfo.IsFullTrust = true;

			appManifest.AssemblyIdentity = new AssemblyIdentity(_profile.Name, _profile.Version);
			appManifest.AssemblyIdentity.ProcessorArchitecture = "msil";

			ManifestHelper.AddFilesToAppManifest(_profile.Directory, appManifest);
			ManifestHelper.AddFilesToTargetFolder(_profile.Directory, PublishProductVersionPath, appManifest);
			appManifest.ResolveFiles();
			appManifest.UpdateFileInfo();

			ManifestWriter.WriteManifest(appManifest, appManifestPath);

			ManifestHelper.SignManifest(appManifest, Settings.Default.CertificateFile, Settings.Default.CertificatePassword);
			return appManifest;
		}

		private DeployManifest CreateDeploymentManifest(ApplicationManifest appManifest)
		{
			string deployManifestFilename = _profile.Name + ".application";
			deployManifestFilename = deployManifestFilename.Replace(' ', '_');
			string deployManifestPath = Path.Combine(PublishProductPath, deployManifestFilename);

			DeployManifest deployManifest = new DeployManifest();
			deployManifest.SourcePath = deployManifestPath;
			deployManifest.MapFileExtensions = true;
			deployManifest.UpdateEnabled = true;
			deployManifest.UpdateMode = UpdateMode.Foreground;
			deployManifest.Install = true;
			deployManifest.Product = _profile.Name;
			deployManifest.DeploymentUrl = _profile.ApplicationUrl;
			deployManifest.Publisher = "ClearCanvas Inc.";
			deployManifest.MinimumRequiredVersion = _profile.Version;

			deployManifest.AssemblyIdentity = new AssemblyIdentity(_profile.Name, _profile.Version);
			deployManifest.AssemblyIdentity.ProcessorArchitecture = "msil";

			// Add reference to the application manifest
			AssemblyReference assemblyReference = new AssemblyReference();
			string relativeFolder = ManifestHelper.GetRelativeFolderPath(appManifest.SourcePath, Path.GetDirectoryName(deployManifestPath));
			string targetPath = Path.Combine(relativeFolder, Path.GetFileName(appManifest.SourcePath));
			assemblyReference.TargetPath = targetPath;
			assemblyReference.AssemblyIdentity = AssemblyIdentity.FromFile(appManifest.SourcePath);

			deployManifest.AssemblyReferences.Add(assemblyReference);
			deployManifest.ResolveFiles();
			deployManifest.UpdateFileInfo();

			ManifestWriter.WriteManifest(deployManifest, deployManifestPath);
			ManifestHelper.SignManifest(deployManifest, Settings.Default.CertificateFile, Settings.Default.CertificatePassword);

			return deployManifest;
		}


		private void CreateBootstrapper(DeployManifest deployManifest)
		{
			Engine.GlobalEngine.BinPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			Engine buildEngine = new Engine();
			buildEngine.BinPath = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
			Project project = new Project(buildEngine);
			string projectXml = CreateBootstrapperXml(Resources.Bootstrapper, deployManifest);
			project.LoadXml(projectXml);

			FileLogger logger = new FileLogger();
			logger.Parameters = @"logfile=build.log";
			buildEngine.RegisterLogger(logger);
			bool result = buildEngine.BuildProject(project);
			buildEngine.UnregisterAllLoggers();

			if (!result)
				throw new Exception("Unable to create boostrapper.");
		}

		private string CreateBootstrapperXml(string bootstrapper, DeployManifest deployManifest)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(bootstrapper);
			
			XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
			manager.AddNamespace("msbuild", _msbuildNamespace);
			
			AddPrerequisitesXml(doc, manager);
			ConfigureTargetXml(doc, manager, deployManifest);

			StringWriter sw = new StringWriter();
			XmlWriter xw = XmlWriter.Create(sw);
			doc.WriteTo(xw);
			xw.Close();

			return sw.ToString();
		}

		private void AddPrerequisitesXml(XmlDocument doc, XmlNamespaceManager manager)
		{
			XmlNode itemGroup = doc.SelectSingleNode("/msbuild:Project/msbuild:ItemGroup", manager);

			foreach (string package in _profile.Prerequisites)
			{
				XmlElement bootstrapperFile = doc.CreateElement("BootstrapperFile", _msbuildNamespace);
				bootstrapperFile.SetAttribute("Include", package);
				XmlElement productName = doc.CreateElement("ProductName", _msbuildNamespace);
				productName.InnerText = package;
				bootstrapperFile.AppendChild(productName);
				itemGroup.AppendChild(bootstrapperFile);
			}
		}

		private void ConfigureTargetXml(XmlDocument doc, XmlNamespaceManager manager, DeployManifest deployManifest)
		{
			XmlElement node = doc.SelectSingleNode("/msbuild:Project/msbuild:Target/msbuild:GenerateBootstrapper", manager) as XmlElement;

			node.SetAttribute("OutputPath", PublishProductPath);

			// Get only the filename--that's what the bootstrap generator wants
			node.SetAttribute("ApplicationFile", Path.GetFileName(deployManifest.SourcePath));
			node.SetAttribute("ApplicationName", deployManifest.Product);

			// Get the url of where the .application file resides, but don't include the file itself in the url
			int pos = _profile.ApplicationUrl.LastIndexOf('/');
			string url = _profile.ApplicationUrl.Substring(0, pos);

			node.SetAttribute("ApplicationUrl", url);
		}
	}
}
