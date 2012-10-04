#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;

// ... (other using namespace statements here)

namespace MyPlugin.Imaging
{
	/// <remarks>
	/// You can override some or all parts of the default layout process.
	/// Only include the overrides you want, and make sure they don't call
	/// the default implementation, or some very strange things may happen!
	/// 
	/// But don't include both this class and the MyOtherLayoutManager class below!
	/// 
	/// To select the one you wish to enable, change the flag on one of them to True.
	/// i.e. [ExtensionOf(typeof (LayoutManagerExtensionPoint), Enabled = true)]
	/// </remarks>
	[ExtensionOf(typeof (LayoutManagerExtensionPoint), Enabled = false)]
	public class MyLayoutManager : LayoutManager
	{
		protected override void BuildLogicalWorkspace()
		{
			// Build your logical workspace here, 
			// i.e., add your DisplaySets and PresentationImages
		}

		protected override void LayoutPhysicalWorkspace()
		{
			// Build your physical workspace here, 
			// i.e., add your ImageBoxes and Tiles
		}

		protected override void FillPhysicalWorkspace()
		{
			// Populate your ImageBoxes with your DisplaySets here
		}

		protected override void SortStudies(StudyCollection studies)
		{
			// Do your own sorting of studies here
		}

		/// <remarks>
		/// Don't override SortStudies if you use this.
		/// </remarks>
		protected override IComparer<Study> GetStudyComparer()
		{
			// return your own IComparer<Study>
			return null;
		}
	}

	/// <remarks>
	/// Or you can override all of it entirely!
	/// 
	/// But don't include both this class and the MyLayoutManager class above!
	/// 
	/// To select the one you wish to enable, change the flag on one of them to True.
	/// i.e. [ExtensionOf(typeof (LayoutManagerExtensionPoint), Enabled = true)]
	/// </remarks>
	[ExtensionOf(typeof (LayoutManagerExtensionPoint), Enabled = false)]
	public class MyOtherLayoutManager : LayoutManager
	{
		public override void Layout()
		{
			// if you override this, you will need to manually call
			// BuildLogicalWorkspace, LayoutPhysicalWorkspace, etc.
			//
			// of course, you can also put all the code in here instead!
		}
	}
}