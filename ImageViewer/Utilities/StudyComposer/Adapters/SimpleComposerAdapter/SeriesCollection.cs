#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Utilities.StudyComposer.Adapters.SimpleComposerAdapter
{
	public sealed class SeriesCollection : BindingListWrapper<SeriesItem>
	{
		internal SeriesCollection(BindingList<SeriesItem> list)
			: base(list) {}

		public SeriesItem GetFirstSeries()
		{
			if (this.Count == 0)
				return null;
			return this[0];
		}
	}
}