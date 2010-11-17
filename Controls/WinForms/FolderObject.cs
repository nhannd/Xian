#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Controls.WinForms
{
	public abstract class FolderObject : IEquatable<FolderObject>
	{
		private readonly string _displayName;
		private readonly string _path;
		private readonly string _virtualPath;
		private readonly bool _isFolder;
		private readonly bool _isLink;

		protected FolderObject(string path, string virtualPath, string displayName, bool isFolder, bool isLink)
		{
			_path = path;
			_virtualPath = virtualPath;
			_displayName = displayName;
			_isFolder = isFolder;
			_isLink = isLink;
		}

		public string DisplayName
		{
			get { return _displayName; }
		}

		public string Path
		{
			get { return _path; }
		}

		public string VirtualPath
		{
			get { return _virtualPath; }
		}

		public bool IsFolder
		{
			get { return _isFolder; }
		}

		public bool IsLink
		{
			get { return _isLink; }
		}

		public string ResolveLink()
		{
			return ResolveLink(IntPtr.Zero);
		}

		public string ResolveLink(IntPtr hWnd)
		{
			if (!IsLink)
				throw new InvalidOperationException("Object is not a file system link.");
			return ShellItem.ResolveLink(hWnd, Path);
		}

		public string GetPath()
		{
			return GetPath(false);
		}

		public string GetPath(bool resolveLinks)
		{
			return GetPath(IntPtr.Zero, resolveLinks);
		}

		public string GetPath(IntPtr hWnd, bool resolveLinks)
		{
			if (resolveLinks && IsLink)
			{
				string resolvedPath;
				if (ShellItem.TryResolveLink(hWnd, Path, out resolvedPath))
					return resolvedPath;
			}
			return Path;
		}

		public bool Equals(FolderObject other)
		{
			if (other != null)
				return _path == other._path && _virtualPath == other._virtualPath;
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is FolderObject)
				return this.Equals((FolderObject) obj);
			return false;
		}

		public override int GetHashCode()
		{
			return 0x7B3F7EEF ^ _path.GetHashCode() ^ _virtualPath.GetHashCode();
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(_path))
				return _displayName;
			return _path;
		}
	}
}