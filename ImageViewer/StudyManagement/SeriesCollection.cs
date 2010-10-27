#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A collection of <see cref="Series"/> objects.
	/// </summary>
	public class SeriesCollection : ObservableList<Series>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SeriesCollection"/>.
		/// </summary>
		public SeriesCollection()
		{

		}

		internal Series this[string seriesInstanceUid]
		{
			get
			{
				return CollectionUtils.SelectFirst(this, delegate(Series series) { return series.SeriesInstanceUid == seriesInstanceUid; });
			}
		}
	}
}
