using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Controls.WinForms;
using Microsoft.VisualBasic.FileIO;

namespace ClearCanvas.ImageViewer.Dashboard.Local
{
	public partial class DetailViewControl : UserControl
	{
		private string _selectedPath;
		private event EventHandler _openImageEvent;
		private readonly string _myStudies = "My Studies";
		private readonly string _hardDisk = "C:\\";
		private readonly string _myDocuments = "My Documents";
		private readonly string _desktop = "Desktop";

		public DetailViewControl()
		{
			InitializeComponent();

			this._goToComboBox.Items.Add(_myStudies);
			this._goToComboBox.Items.Add(_hardDisk);
			this._goToComboBox.Items.Add(_myDocuments);
			this._goToComboBox.Items.Add(_desktop);

			this._toolStrip.Layout += new LayoutEventHandler(ToolStrip_Layout);
			this._fileSystemTreeView.AfterSelect += new TreeViewEventHandler(FileSystemTreeView_AfterSelect);
			this._loadButton.Click += new EventHandler(OnOpenImage);
			this._loadMenuItem.Click += new EventHandler(OnOpenImage);
			this._goToComboBox.SelectedIndexChanged += new EventHandler(GoToComboBox_SelectedIndexChanged);
			this._fileSystemTreeView.MouseDoubleClick += new MouseEventHandler(FileSystemTreeView_MouseDoubleClick);
		}


		public event EventHandler OpenImage
		{
			add { _openImageEvent += value; }
			remove { _openImageEvent -= value; }
		}

		public string HeaderText
		{
			get { return this._headerLabel.Text; }
			set { this._headerLabel.Text = value; }
		}

		public string SelectedPath
		{
			get { return _selectedPath; }
		}

		public void Initialize()
		{
			this._goToComboBox.SelectedIndex = 0;
		}

		private void LoadDirectory(string directoryPath)
		{
			this._fileSystemTreeView.Load(directoryPath);

			TreeNodeCollection nodes = this._fileSystemTreeView.Nodes;

			// Select the root node by default
			if (nodes.Count > 0)
				this._fileSystemTreeView.SelectedNode = nodes[0];
		}

		private void FileSystemTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			FileNode fileNode = e.Node as FileNode;

			if (fileNode != null)
			{
				_selectedPath = fileNode.FileInfo.FullName;
			}
			else
			{
				DirectoryNode directoryNode = e.Node as DirectoryNode;

				if (directoryNode != null)
					_selectedPath = directoryNode.DirectoryInfo.FullName;
			}

			this._pathBox.Text = _selectedPath;
			this._pathBox.ToolTipText = _selectedPath;
		}

		private void OnOpenImage(object sender, EventArgs e)
		{
			EventsHelper.Fire(_openImageEvent, this, EventArgs.Empty);
		}

		private void ToolStrip_Layout(object sender, LayoutEventArgs e)
		{
			// Make the path box fill remaining width of the toolstrip
			// Code taken from MSDN forums
			int width = this._toolStrip.DisplayRectangle.Width;

			foreach (ToolStripItem tsi in this._toolStrip.Items)
			{
				if (!(tsi == this._pathBox))
				{
					width -= tsi.Width;
					width -= tsi.Margin.Horizontal;
				}
			}

			this._pathBox.Width = Math.Max(0, width - this._pathBox.Margin.Horizontal);
		}

		private void FileSystemTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (this._fileSystemTreeView.SelectedNode is FileNode)
				EventsHelper.Fire(_openImageEvent, this, EventArgs.Empty);
		}

		private void GoToComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			string selectedRootFolder = this._goToComboBox.SelectedItem.ToString();
			string rootFolder;

			if (selectedRootFolder == _myStudies)
				rootFolder = Platform.StudyDir;
			else if (selectedRootFolder == _hardDisk)
				rootFolder = "C:\\";
			else if (selectedRootFolder == _myDocuments)
				rootFolder = SpecialDirectories.MyDocuments;
			else if (selectedRootFolder == _desktop)
				rootFolder = SpecialDirectories.Desktop;
			else
				rootFolder = selectedRootFolder;

			LoadDirectory(rootFolder);
		}
	}
}
