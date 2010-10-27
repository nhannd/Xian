#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.DisplaySetChanging"/> event.
	/// </summary>
	public class DisplaySetChangingEventArgs : EventArgs
	{
		private readonly IDisplaySet _currentDisplaySet;
		private readonly IDisplaySet _newDisplaySet;

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetChangingEventArgs"/>.
		/// </summary>
		public DisplaySetChangingEventArgs(
			IDisplaySet currentDisplaySet,
			IDisplaySet newDisplaySet)
		{
			_currentDisplaySet = currentDisplaySet;
			_newDisplaySet = newDisplaySet;
		}

		/// <summary>
		/// Gets the current <see cref="IDisplaySet"/>.
		/// </summary>
		public IDisplaySet CurrentDisplaySet
		{
			get { return _currentDisplaySet; }
		}

		/// <summary>
		/// Gets the new <see cref="IDisplaySet"/>.
		/// </summary>
		public IDisplaySet NewDisplaySet
		{
			get { return _newDisplaySet; }
		}
	}
}
