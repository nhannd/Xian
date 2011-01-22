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
using System.Windows.Forms;

namespace ClearCanvas.ImageServer.Model.SqlServer.CodeGenerator
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
            textBoxEntityImplementationFolder.Text = Path.Combine(Path.Combine(_basePath, "SqlServer"), "EntityBrokers");

            comboBoxDatabase.Items.Add("ImageServer");
            comboBoxDatabase.Items.Add("MigrationTool");
            comboBoxDatabase.Items.Add("UsageTracking");
        }

        private void buttonGenerateCode_Click(object sender, EventArgs e)
        {
            Generator generator = new Generator
                                      {
                                          ModelNamespace = textBoxModelNamespace.Text,
                                          ImageServerModelFolder = textBoxModelFolder.Text,
                                          EntityImplementationFolder = textBoxEntityImplementationFolder.Text,
                                          EntityImplementationNamespace = textBoxEntityImplementationNamespace.Text,
                                          EntityInterfaceFolder = textBoxEntityInterfaceFolder.Text,
                                          EntityInterfaceNamespace = textBoxEntityInterfaceNamespace.Text
                                      };

            if (comboBoxDatabase.Text.Equals("ImageServer"))
            {
                generator.ConnectionStringName = "ImageServerConnectString";
                generator.Proprietary = false;
            }
            else if (comboBoxDatabase.Text.Equals("MigrationTool"))
            {
                generator.ConnectionStringName = "MonarchConnectString";
                generator.Proprietary = true;
            }
            else if (comboBoxDatabase.Text.Equals("UsageTracking"))
            {
                generator.ConnectionStringName = "UsageTrackingConnectString";
                generator.Proprietary = true;
            }

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
                Path.Combine(Path.Combine(folderBrowserDialog.SelectedPath, "SqlServer"), "EntityBrokers");
        }

        private void textBoxEntityImplementationNamespace_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDatabase.SelectedItem.Equals("ImageServer"))
            {
                textBoxModelNamespace.Text = "ClearCanvas.ImageServer.Model";
                textBoxEntityInterfaceNamespace.Text = "ClearCanvas.ImageServer.Model.EntityBrokers";
                textBoxEntityImplementationNamespace.Text = "ClearCanvas.ImageServer.Model.SqlServer.EntityBrokers";
                textBoxModelFolder.Text = "d:\\Projects\\ClearCanvas\\ImageServer\\Model";
                textBoxEntityInterfaceFolder.Text = "d:\\Projects\\ClearCanvas\\ImageServer\\Model\\EntityBrokers";
                textBoxEntityImplementationFolder.Text = "d:\\Projects\\ClearCanvas\\ImageServer\\Model\\SqlServer\\EntityBrokers";
            }
            if (comboBoxDatabase.SelectedItem.Equals("MigrationTool"))
            {
                textBoxModelNamespace.Text = "ClearCanvas.Migration.Dicom.Model";
                textBoxEntityInterfaceNamespace.Text = "ClearCanvas.Migration.Dicom.Model.EntityBrokers";
                textBoxEntityImplementationNamespace.Text = "ClearCanvas.Migration.Dicom.Model.SqlServer.EntityBrokers";
                textBoxModelFolder.Text = "d:\\Projects\\Jin\\Migration\\Dicom\\Model";
                textBoxEntityInterfaceFolder.Text = "D:\\Projects\\Jin\\Migration\\Dicom\\Model\\EntityBrokers";
                textBoxEntityImplementationFolder.Text = "d:\\Projects\\Jin\\Migration\\Dicom\\Model\\SqlServer\\EntityBrokers";
            }
            if (comboBoxDatabase.SelectedItem.Equals("UsageTracking"))
            {
                textBoxModelNamespace.Text = "ClearCanvas.UsageTracking.Model";
                textBoxEntityInterfaceNamespace.Text = "ClearCanvas.UsageTracking.Model.EntityBrokers";
                textBoxEntityImplementationNamespace.Text = "ClearCanvas.UsageTracking.Model.SqlServer.EntityBrokers";
                textBoxModelFolder.Text = "d:\\Projects\\Jin\\UsageTracking\\Model";
                textBoxEntityInterfaceFolder.Text = "d:\\Projects\\Jin\\UsageTracking\\Model\\EntityBrokers";
                textBoxEntityImplementationFolder.Text = "d:\\Projects\\Jin\\UsageTracking\\Model\\SqlServer\\EntityBrokers";
            }
            Refresh();
        }
    }
}