using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using ClickOncePublisher.Properties;
using System.IO;
using Microsoft.Build.Tasks.Deployment.ManifestUtilities;

namespace ClickOncePublisher
{
	class Publisher : INotifyPropertyChanged
	{
		private string _publishPath;
		private string _productsPath;
		private List<string> _products;
		private List<string> _versions;
		private string _selectedProduct;
		private string _selectedVersion;

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler ProductsChanged;
		public event EventHandler ProductChanged;

		public Publisher()
		{
			if (Directory.Exists(Settings.Default.PublishPath))
				PublishPath = Settings.Default.PublishPath;
			else
				PublishPath = "C:\\";

			if (Directory.Exists(Settings.Default.ProductsPath))
				ProductsPath = Settings.Default.ProductsPath;
			else
				ProductsPath = "C:\\";

			SelectedProduct = Settings.Default.SelectedProduct;
			SelectedVersion = Settings.Default.SelectedVersion;
		}

		public string PublishPath
		{
			get { return _publishPath; }
			set 
			{
				if (_publishPath != value)
				{
					_publishPath = value;
					NotifyPropertyChanged("PublishPath");
				}
			}
		}

		public string ProductsPath
		{
			get { return _productsPath; }
			set
			{
				if (_productsPath != value)
				{
					_productsPath = value;
					NotifyPropertyChanged("ProductPath");

					if (ProductsChanged != null)
						ProductsChanged(this, EventArgs.Empty);
				}
			}
		}

		public List<string> Products
		{
			get
			{
				List<string> products = new List<string>();

				if (Directory.Exists(ProductsPath))
				{
					string[] dirs = Directory.GetDirectories(ProductsPath);

					foreach (string dir in dirs)
						products.Add(GetLastElementInPath(dir));
				}

				return products;
			}
		}

		public List<string> Versions
		{
			get
			{
				List<string> versions = new List<string>();

				string productDir = String.Format("{0}\\{1}", ProductsPath, SelectedProduct);

				if (Directory.Exists(productDir))
				{
					string[] dirs = Directory.GetDirectories(productDir);

					foreach (string dir in dirs)
						versions.Add(GetLastElementInPath(dir));
				}

				return versions;
			}
		}

		public string SelectedProduct
		{
			get { return _selectedProduct; }
			set
			{
				if (_selectedProduct != value)
				{
					_selectedProduct = value;
					NotifyPropertyChanged("SelectedProduct");

					if (ProductChanged != null)
						ProductChanged(this, EventArgs.Empty);
				}
			}
		}

		public string SelectedVersion
		{
			get { return _selectedVersion; }
			set
			{
				if (_selectedVersion != value)
				{
					_selectedVersion = value;
					NotifyPropertyChanged("SelectedVersion");
				}
			}
		}

		private string SelectedProductPath
		{
			get
			{
				string str = String.Format("{0}\\{1}", ProductsPath, SelectedProduct);
				return str;
			}
		}

		private string SelectedProductVersionPath
		{
			get
			{
				string str = String.Format("{0}\\{1}", SelectedProductPath, SelectedVersion);
				return str;
			}
		}

		private string PublishProductPath
		{
			get
			{
				string str = String.Format("{0}\\{1}", PublishPath, SelectedProduct);
				return str;
			}
		}

		private string PublishProductVersionPath
		{
			get
			{
				string str = String.Format("{0}\\{1}", PublishProductPath, SelectedVersion);
				return str;
			}
		}

		public void Publish()
		{
			if (Directory.Exists(PublishProductVersionPath))
				Directory.Delete(PublishProductVersionPath, true);

			Directory.CreateDirectory(PublishProductVersionPath);

			ApplicationManifest appManifest = CreateApplicationManifest();
			CreateDeploymentManifest(appManifest);
		}

		private ApplicationManifest CreateApplicationManifest()
		{
			string appManifestPath = String.Format("{0}\\{1}.manifest", PublishProductVersionPath, SelectedProduct);

			ApplicationManifest appManifest = new ApplicationManifest();
			appManifest.SourcePath = appManifestPath;

			string exePath = Path.Combine(SelectedProductVersionPath, "ClearCanvas.Desktop.Executable.exe");
			appManifest.EntryPoint = new AssemblyReference(exePath);
			appManifest.EntryPoint.AssemblyIdentity = AssemblyIdentity.FromFile(exePath);

			appManifest.TrustInfo = new TrustInfo();
			appManifest.TrustInfo.IsFullTrust = true;

			appManifest.AssemblyIdentity = new AssemblyIdentity(SelectedProduct, SelectedVersion);
			appManifest.AssemblyIdentity.ProcessorArchitecture = "msil";

			ManifestHelper.AddFilesToAppManifest(SelectedProductVersionPath, appManifest);
			ManifestHelper.AddFilesToTargetFolder(SelectedProductVersionPath, PublishProductVersionPath, appManifest);
			appManifest.ResolveFiles();
			appManifest.UpdateFileInfo();

			ManifestWriter.WriteManifest(appManifest, appManifestPath);

			ManifestHelper.SignManifest(appManifest, "C:\\ClearCanvas.Desktop.Executable_TemporaryKey.pfx", "");
			return appManifest;
		}

		private void CreateDeploymentManifest(ApplicationManifest appManifest)
		{
			string deployManifestPath = String.Format("{0}\\{1}.application", PublishProductPath, SelectedProduct);

			DeployManifest deployManifest = new DeployManifest();
			deployManifest.SourcePath = deployManifestPath;
			deployManifest.MapFileExtensions = true;
			deployManifest.UpdateEnabled = true;
			deployManifest.UpdateMode = UpdateMode.Foreground;
			deployManifest.Install = true;
			deployManifest.Product = SelectedProduct;
			deployManifest.DeploymentUrl = "http://apps.clearcanvas.ca";

			deployManifest.AssemblyIdentity = new AssemblyIdentity(SelectedProduct, SelectedVersion);
			deployManifest.AssemblyIdentity.ProcessorArchitecture = "msil";

			AssemblyReference assemblyReference = new AssemblyReference();
			string relativeFolder = ManifestHelper.GetRelativeFolderPath(appManifest.SourcePath, Path.GetDirectoryName(deployManifestPath));
			string targetPath = Path.Combine(relativeFolder, Path.GetFileName(appManifest.SourcePath));
			assemblyReference.TargetPath = targetPath;
			assemblyReference.AssemblyIdentity = AssemblyIdentity.FromFile(appManifest.SourcePath);

			deployManifest.AssemblyReferences.Add(assemblyReference);
			deployManifest.ResolveFiles();
			deployManifest.UpdateFileInfo();

			ManifestWriter.WriteManifest(deployManifest, deployManifestPath);
			ManifestHelper.SignManifest(deployManifest, "C:\\ClearCanvas.Desktop.Executable_TemporaryKey.pfx", "");
		}


		private void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public void Close()
		{
			Settings.Default.PublishPath = PublishPath;
			Settings.Default.ProductsPath = ProductsPath;
			Settings.Default.SelectedProduct = SelectedProduct;
			Settings.Default.SelectedVersion = SelectedVersion;
			Settings.Default.Save();
		}

		private string GetLastElementInPath(string path)
		{
			string[] elements = path.Split('\\');
			return elements[elements.Length - 1];
		}
	}
}
