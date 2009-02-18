using System;
using System.IO;
using System.Windows.Forms;

namespace ClearCanvas.ImageServer.Model.SqlServer2005.CodeGenerator
{
	public partial class MainForm : Form
	{
		private String _basePath;

		public MainForm()
		{
			InitializeComponent();

			_basePath = Path.GetDirectoryName(Application.ExecutablePath);
			DirectoryInfo dir = new DirectoryInfo(_basePath);
			dir = dir.Parent.Parent.Parent.Parent;
			_basePath = dir.FullName;

			textBoxModelFolder.Text = _basePath;
			textBoxEntityInterfaceFolder.Text = Path.Combine(_basePath, "EntityBrokers");
			textBoxEntityImplementationFolder.Text = Path.Combine(Path.Combine(_basePath, "SqlServer2005"), "EntityBrokers");

		}

		private void buttonGenerateCode_Click(object sender, EventArgs e)
		{
			Generator generator = new Generator();

			generator.ModelNamespace = textBoxModelNamespace.Text;
			generator.ImageServerModelFolder = textBoxModelFolder.Text;

			generator.EntityImplementationFolder = textBoxEntityImplementationFolder.Text;
			generator.EntityImplementationNamespace = textBoxEntityImplementationNamespace.Text;

			generator.EntityInterfaceFolder = textBoxEntityInterfaceFolder.Text;
			generator.EntityInterfaceNamespace = textBoxEntityInterfaceNamespace.Text;

			generator.Generate();
		}

		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.Description = "Select Root ImageServer Project Folder";
			folderBrowserDialog.SelectedPath = textBoxModelFolder.Text;

			folderBrowserDialog.ShowDialog();

			textBoxModelFolder.Text = folderBrowserDialog.SelectedPath;
			textBoxEntityInterfaceFolder.Text = Path.Combine(folderBrowserDialog.SelectedPath, "EntityBrokers");
			textBoxEntityImplementationFolder.Text =
				Path.Combine(Path.Combine(folderBrowserDialog.SelectedPath, "SqlServer2005"), "EntityBrokers");
		}

		private void textBoxEntityImplementationNamespace_TextChanged(object sender, EventArgs e)
		{

		}
	}
}