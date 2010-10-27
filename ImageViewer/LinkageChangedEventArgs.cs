#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// 
	/// </summary>
	internal class LinkageChangedEventArgs : EventArgs
	{
		private bool _isLinked;

		internal LinkageChangedEventArgs(bool isLinked)
		{
			_isLinked = isLinked;
		}

		/// <summary>
		/// 
		/// </summary>
		public bool IsLinked { get { return _isLinked; } }
	}
}
