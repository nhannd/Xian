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