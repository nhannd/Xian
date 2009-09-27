using System;

namespace ClearCanvas.Controls.WinForms
{
	public abstract class FolderObject : IEquatable<FolderObject>
	{
		private readonly string _displayName;
		private readonly string _path;
		private readonly string _virtualPath;
		private readonly bool _isFolder;

		protected FolderObject(Pidl pidl)
		{
			_displayName = pidl.DisplayName;
			_path = pidl.Path;
			_virtualPath = pidl.VirtualPath;
			_isFolder = pidl.IsFolder;
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