#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
	/// Provides data for the <see cref="EventBroker.DisplaySetChanged"/> event.
	/// </summary>
	public class DisplaySetChangedEventArgs : EventArgs
	{
		private readonly IDisplaySet _oldDisplaySet;
		private readonly IDisplaySet _newDisplaySet;

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetChangedEventArgs"/>.
		/// </summary>
		public DisplaySetChangedEventArgs(
			IDisplaySet oldDisplaySet,
			IDisplaySet newDisplaySet)
		{
			_oldDisplaySet = oldDisplaySet;
			_newDisplaySet = newDisplaySet;
		}

		/// <summary>
		/// Gets the old <see cref="IDisplaySet"/>.
		/// </summary>
		public IDisplaySet OldDisplaySet
		{
			get { return _oldDisplaySet; }
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
