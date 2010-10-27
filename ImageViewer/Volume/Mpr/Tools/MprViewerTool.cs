#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	public abstract class MprViewerTool : MouseImageViewerTool
	{
		protected MprViewerTool() {}

		public new MprViewerComponent ImageViewer
		{
			get { return base.ImageViewer as MprViewerComponent; }
		}

		protected MprSliceSop SelectedMprSliceSop
		{
			get
			{
				IImageSopProvider selectedImageSopProvider = base.SelectedImageSopProvider;
				if (selectedImageSopProvider != null)
					return selectedImageSopProvider.ImageSop as MprSliceSop;
				return null;
			}
		}

		protected IMprSliceSet SelectedMprSliceSet
		{
			get
			{
				MprSliceSop selectedMprSliceSop = this.SelectedMprSliceSop;
				if (selectedMprSliceSop != null)
					return selectedMprSliceSop.Parent;
				return null;
			}
		}

		protected IMprVolume SelectedMprVolume
		{
			get
			{
				IMprSliceSet selectedMprSliceSet = this.SelectedMprSliceSet;
				if (selectedMprSliceSet != null)
					return selectedMprSliceSet.Parent;
				return null;
			}
		}
	}
}