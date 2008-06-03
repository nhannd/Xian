using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CodeGenerator
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

            generator.ModelNamespace = this.textBoxModelNamespace.Text;
            generator.ImageServerModelFolder = textBoxModelFolder.Text;

            generator.EntityImplementationFolder = this.textBoxEntityImplementationFolder.Text;
            generator.EntityImplementationNamespace = this.textBoxEntityImplementationNamespace.Text;

            generator.EntityInterfaceFolder = this.textBoxEntityInterfaceFolder.Text;
            generator.EntityInterfaceNamespace = this.textBoxEntityInterfaceNamespace.Text;

            generator.Generate();
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog.Description = "Select Root ImageServer Project Folder";
            this.folderBrowserDialog.SelectedPath = this.textBoxModelFolder.Text;

            folderBrowserDialog.ShowDialog();

            textBoxModelFolder.Text = folderBrowserDialog.SelectedPath;
            this.textBoxEntityInterfaceFolder.Text = Path.Combine(folderBrowserDialog.SelectedPath, "EntityBrokers");
            this.textBoxEntityImplementationFolder.Text =
                Path.Combine(Path.Combine(folderBrowserDialog.SelectedPath, "SqlServer2005"), "EntityBrokers");
        }

        private void textBoxEntityImplementationNamespace_TextChanged(object sender, EventArgs e)
        {

        }
    }
}