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
	/// A collection of <see cref="Sop"/> objects.
	/// </summary>
	public class SopCollection : ObservableList<Sop>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SopCollection"/>.
		/// </summary>
		public SopCollection()
		{
		}

		internal Sop this[string sopInstanceUid]
		{
			get
			{
				return CollectionUtils.SelectFirst(this, delegate(Sop sop) { return sop.SopInstanceUid == sopInstanceUid; });
			}
		}
	}
}
