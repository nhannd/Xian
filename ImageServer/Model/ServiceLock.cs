#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageServer.Model
{
	public partial class ServiceLock
	{
		#region Private Members
		private Filesystem _filesystem;
		#endregion

		#region Public Properties
		public Filesystem Filesystem
		{
			get
			{
				if (FilesystemKey == null)
					return null;
				if (_filesystem == null)
					_filesystem = Filesystem.Load(FilesystemKey);
				return _filesystem;
			}
		}
		#endregion
	}
}
