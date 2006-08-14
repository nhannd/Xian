//////////////////////////////////////////////////////////////////
//
// Adapted from CodeProject.com article, "Filesystem TreeView"
// By Michael Ceranski 
//
//////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections;

namespace ClearCanvas.Controls.WinForms
{
	public class FileSystemTreeView : TreeView
	{
		private bool _showFiles = true;
		private ImageList _imageList = new ImageList();
		private Hashtable _systemIcons = new Hashtable();

		public static readonly int _folder = 0;

		public FileSystemTreeView()
		{
			this.ImageList = _imageList;
			this.MouseDown += new MouseEventHandler(FileSystemTreeView_MouseDown);
			this.BeforeExpand += new TreeViewCancelEventHandler(FileSystemTreeView_BeforeExpand);
		}

		public bool ShowFiles
		{
			get { return _showFiles; }
			set { _showFiles = value; }
		}

		public void Load(string directoryPath)
		{
			if (Directory.Exists(directoryPath) == false)
				throw new DirectoryNotFoundException("Directory Not Found");

			_systemIcons.Clear();
			_imageList.Images.Clear();
			Nodes.Clear();

			Icon folderIcon = new Icon(typeof(FileSystemTreeView), "Icons.folder.ico");

			_imageList.Images.Add(folderIcon);
			_systemIcons.Add(FileSystemTreeView._folder, 0);

			DirectoryNode node = new DirectoryNode(this, new DirectoryInfo(directoryPath));
			node.Expand();
		}

		internal int GetIconImageIndex(string path)
		{
			string extension = Path.GetExtension(path);

			if (_systemIcons.ContainsKey(extension) == false)
			{
				Icon icon = ShellIcon.GetSmallIcon(path);
				_imageList.Images.Add(icon);
				_systemIcons.Add(extension, _imageList.Images.Count - 1);
			}

			return (int)_systemIcons[Path.GetExtension(path)];
		}

		private void FileSystemTreeView_MouseDown(object sender, MouseEventArgs e)
		{
			TreeNode node = this.GetNodeAt(e.X, e.Y);

			if (node == null)
				return;

			this.SelectedNode = node; //select the node under the mouse         
		}

		private void FileSystemTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			if (e.Node is FileNode) return;

			DirectoryNode node = (DirectoryNode)e.Node;

			if (!node.Loaded)
			{
				node.Nodes[0].Remove(); //remove the fake child node used for virtualization
				node.LoadDirectory();
				if (this._showFiles == true)
					node.LoadFiles();
			}
		}
	}

	public class DirectoryNode : TreeNode
	{
		private DirectoryInfo _directoryInfo;

		public DirectoryNode(DirectoryNode parent, DirectoryInfo directoryInfo)
			: base(directoryInfo.Name)
		{
			this._directoryInfo = directoryInfo;

			this.ImageIndex = FileSystemTreeView._folder;
			this.SelectedImageIndex = this.ImageIndex;

			parent.Nodes.Add(this);

			Virtualize();
		}

		public DirectoryNode(FileSystemTreeView treeView, DirectoryInfo directoryInfo)
			: base(directoryInfo.Name)
		{
			this._directoryInfo = directoryInfo;

			this.ImageIndex = FileSystemTreeView._folder;
			this.SelectedImageIndex = this.ImageIndex;

			treeView.Nodes.Add(this);

			Virtualize();

		}

		public DirectoryInfo DirectoryInfo
		{
			get { return _directoryInfo; }
		}

		void Virtualize()
		{
			int fileCount = 0;

			try
			{
				if (this.TreeView.ShowFiles == true)
					fileCount = this._directoryInfo.GetFiles().Length;

				if ((fileCount + this._directoryInfo.GetDirectories().Length) > 0)
					new FakeChildNode(this);
			}
			catch
			{
			}
		}

		public void LoadDirectory()
		{
			foreach (DirectoryInfo directoryInfo in _directoryInfo.GetDirectories())
			{
				new DirectoryNode(this, directoryInfo);
			}
		}

		public void LoadFiles()
		{
			foreach (FileInfo file in _directoryInfo.GetFiles())
			{
				new FileNode(this, file);
			}
		}

		public bool Loaded
		{
			get
			{
				if (this.Nodes.Count != 0)
				{
					if (this.Nodes[0] is FakeChildNode)
						return false;
				}

				return true;
			}
		}

		public new FileSystemTreeView TreeView
		{
			get { return (FileSystemTreeView)base.TreeView; }
		}
	}

	public class FileNode : TreeNode
	{
		private FileInfo _fileInfo;
		private DirectoryNode _directoryNode;

		public FileNode(DirectoryNode directoryNode, FileInfo fileInfo)
			: base(fileInfo.Name)
		{
			this._directoryNode = directoryNode;
			this._fileInfo = fileInfo;

			this.ImageIndex = ((FileSystemTreeView)_directoryNode.TreeView).GetIconImageIndex(_fileInfo.FullName);
			this.SelectedImageIndex = this.ImageIndex;

			_directoryNode.Nodes.Add(this);
		}

		public FileInfo FileInfo
		{
			get { return _fileInfo; }
		}
	}

	internal class FakeChildNode : TreeNode
	{
		public FakeChildNode(TreeNode parent)
			: base()
		{
			parent.Nodes.Add(this);
		}
	}

	/// <summary>
	/// Summary description for ShellIcon.
	/// </summary>
	/// <summary>
	/// Summary description for ShellIcon.  Get a small or large Icon with an easy C# function call
	/// that returns a 32x32 or 16x16 System.Drawing.Icon depending on which function you call
	/// either GetSmallIcon(string fileName) or GetLargeIcon(string fileName)
	/// </summary>
	internal class ShellIcon
	{
		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO
		{
			public IntPtr hIcon;
			public IntPtr iIcon;
			public uint dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};

		class Win32
		{
			public const uint SHGFI_ICON = 0x100;
			public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
			public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

			[DllImport("shell32.dll")]
			public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
		}


		public ShellIcon()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public static Icon GetSmallIcon(string fileName)
		{
			IntPtr hImgSmall; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();


			//Use this to get the small Icon
			hImgSmall = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);


			//The icon is returned in the hIcon member of the shinfo struct
			return System.Drawing.Icon.FromHandle(shinfo.hIcon);
		}

		public static Icon GetLargeIcon(string fileName)
		{
			IntPtr hImgLarge; //the handle to the system image list
			SHFILEINFO shinfo = new SHFILEINFO();

			//Use this to get the large Icon
			hImgLarge = Win32.SHGetFileInfo(fileName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_LARGEICON);

			//The icon is returned in the hIcon member of the shinfo struct
			return System.Drawing.Icon.FromHandle(shinfo.hIcon);
		}
	}
}
