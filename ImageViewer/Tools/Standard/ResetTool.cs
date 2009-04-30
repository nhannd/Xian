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

using System;
using System.Drawing;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuReset", "Activate")]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarReset", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipReset")]
	[IconSet("activate", IconScheme.Colour, "Icons.ResetToolSmall.png", "Icons.ResetToolMedium.png", "Icons.ResetToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Reset")]

	[ClearCanvas.Common.ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ResetTool : ImageViewerTool
    {
		private readonly ImageSpatialTransformImageOperation _operation;
		
		public ResetTool()
		{
			_operation = new ImageSpatialTransformImageOperation(Apply);
		}

		public void Activate()
		{
			if (!_operation.AppliesTo(this.SelectedPresentationImage))
				return;

			ImageOperationApplicator applicator = new ImageOperationApplicator(SelectedPresentationImage, _operation);
			CompositeUndoableCommand historyCommand = applicator.ApplyToAllImages();
			if (historyCommand != null)
			{
				historyCommand.Name = SR.CommandReset;
				Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}
		}

		public void Apply(IPresentationImage image)
		{
			IImageSpatialTransform transform = (IImageSpatialTransform)_operation.GetOriginator(image);
			transform.Scale = 1.0f;
			transform.TranslationX = 0.0f;
			transform.TranslationY = 0.0f;
			transform.FlipY = false;
			transform.FlipX = false;
			transform.RotationXY = 0;
			transform.ScaleToFit = true;
		}
	}
}
