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