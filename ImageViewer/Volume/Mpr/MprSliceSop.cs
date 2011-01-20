#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	public class MprSliceSop : ImageSop
	{
		// JY: In general, it is bad practice to derive your own special Sop classes since it
		// results in messy inheritance problems when new SOP types are introduced (e.g. KO, PR).
		// Fortunately for us, MPR slices will ALWAYS be an ImageSop.

		private IMprSliceSet _parent;

		public MprSliceSop(ISopDataSource dataSource) : base(dataSource) { }

		public IMprSliceSet Parent
		{
			get { return _parent; }
			internal set { _parent = value; }
		}

		public new ISopDataSource DataSource
		{
			get { return base.DataSource; }
		}
	}
}