#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tools
{
	[MouseToolButton(XMouseButtons.Left, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public partial class DefineSlicePlaneTool : MouseImageViewerToolMaster<MprViewerTool>
	{
		protected override IEnumerable<MprViewerTool> CreateTools()
		{
			foreach (IMprVolume volume in this.ImageViewer.StudyTree)
			{
				foreach (IMprSliceSet sliceSet in volume.SliceSets)
				{
					IMprStandardSliceSet standardSliceSet = sliceSet as IMprStandardSliceSet;
					if (standardSliceSet != null && !standardSliceSet.IsReadOnly)
					{
						ModifyStandardSliceSetTool tool = new ModifyStandardSliceSetTool();
						tool.SliceSet = standardSliceSet;
						yield return tool;
					}
				}
			}
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			base.OnPresentationImageSelected(sender, e);
			base.Enabled = (this.ImageViewer != null);
		}

		public new MprViewerComponent ImageViewer
		{
			get { return base.ImageViewer as MprViewerComponent; }
		}

		protected static IImageBox FindImageBox(IMprSliceSet sliceSet, MprViewerComponent viewer)
		{
			if (sliceSet == null || viewer == null)
				return null;

			foreach (IImageBox imageBox in viewer.PhysicalWorkspace.ImageBoxes)
			{
				if (imageBox.DisplaySet != null && imageBox.DisplaySet.Uid == sliceSet.Uid)
					return imageBox;
			}
			return null;
		}
	}
}