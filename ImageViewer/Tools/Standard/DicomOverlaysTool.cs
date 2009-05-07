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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.PresentationStates.Dicom;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("showHide", "global-menus/MenuTools/MenuStandard/MenuShowHideDicomOverlay", "ShowHide")]
	[Tooltip("showHide", "TooltipShowHideDicomOverlay")]
	[GroupHint("showHide", "Tools.Image.Overlays.DicomOverlay.ShowHide")]
	[IconSet("showHide", IconScheme.Colour, "Icons.DicomOverlaysToolSmall.png", "Icons.DicomOverlaysToolMedium.png", "Icons.DicomOverlaysToolLarge.png")]
	//
	[ButtonAction("toggle", "overlays-dropdown/ToolbarDicomOverlay", "ShowHide")]
	[CheckedStateObserver("toggle", "Checked", "CheckedChanged")]
	[Tooltip("toggle", "TooltipDicomOverlay")]
	[GroupHint("toggle", "Tools.Image.Overlays.DicomOverlay.ShowHide")]
	[IconSet("toggle", IconScheme.Colour, "Icons.DicomOverlaysToolSmall.png", "Icons.DicomOverlaysToolMedium.png", "Icons.DicomOverlaysToolLarge.png")]
	//
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class DicomOverlaysTool : OverlayToolBase
	{
		public DicomOverlaysTool()
		{
		}

		private static IEnumerable<OverlayPlaneGraphic> GetOverlayPlanesGraphic(IDicomPresentationImage image)
		{
			DicomGraphicsPlane dicomGraphicsPlane = DicomGraphicsPlane.GetDicomGraphicsPlane(image, false);
			if (dicomGraphicsPlane != null)
			{
				foreach (ILayer layer in (IEnumerable<ILayer>)dicomGraphicsPlane.Layers)
				{
					foreach (OverlayPlaneGraphic overlayGraphic in CollectionUtils.Select(layer.Graphics,
						delegate(IGraphic graphic) { return graphic is OverlayPlaneGraphic; }))
					{
						yield return overlayGraphic;
					}
				}
			}
		}

		protected override void  UpdateVisibility(IPresentationImage image, bool visible)
		{
			if (image is IDicomPresentationImage)
			{
				foreach (OverlayPlaneGraphic overlayGraphic in GetOverlayPlanesGraphic(image as IDicomPresentationImage))
					overlayGraphic.Visible = Checked;
			}
		}
	}
}