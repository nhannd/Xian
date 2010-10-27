#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// Provides data for the <see cref="EventBroker.DisplaySetSelected"/> event.
	/// </summary>
	public class DisplaySetSelectedEventArgs : EventArgs
	{
		private readonly IDisplaySet _selectedDisplaySet;

		internal DisplaySetSelectedEventArgs(
			IDisplaySet selectedDisplaySet)
		{
			Platform.CheckForNullReference(selectedDisplaySet, "selectedDisplaySet");
			_selectedDisplaySet = selectedDisplaySet;
		}

		/// <summary>
		/// Gets the selected <see cref="IDisplaySet"/>.
		/// </summary>
		public IDisplaySet SelectedDisplaySet
		{
			get { return _selectedDisplaySet; }
		}
	}
}
