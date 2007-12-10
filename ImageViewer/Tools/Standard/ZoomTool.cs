#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuZoom", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuZoom", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarZoom", "Select", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardZoom/Activate", "Select", KeyStroke = XKeys.Z)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.ZoomToolSmall.png", "Icons.ZoomToolMedium.png", "Icons.ZoomToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Zoom")]

	//Mark the delegates as keyboard controllable, without assigning a default keystroke.
	[KeyboardAction("zoomin", "imageviewer-keyboard/ToolsStandardZoom/ZoomIn", "ZoomIn", KeyStroke = XKeys.OemPeriod)]
	[KeyboardAction("zoomout", "imageviewer-keyboard/ToolsStandardZoom/ZoomOut", "ZoomOut", KeyStroke = XKeys.Oemcomma)]

	[MouseWheelHandler(ModifierFlags.Control)]
	[MouseToolButton(XMouseButtons.Right, false)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ZoomTool : MouseImageViewerTool
	{
		private readonly ImageSpatialTransformImageOperation _operation; 
		private UndoableCommand _command;
		private ImageOperationApplicator _applicator;

		public ZoomTool()
			: base(SR.TooltipZoom)
		{
			this.CursorToken = new CursorToken("Icons.ZoomToolSmall.png", this.GetType().Assembly);
			_operation = new ImageSpatialTransformImageOperation(Apply);
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		private void CaptureBeginState()
		{
			if (!_operation.AppliesTo(this.SelectedPresentationImage))
				return;

			_applicator = new ImageOperationApplicator(this.SelectedPresentationImage, _operation);
			_command = new UndoableCommand(_applicator);
			_command.Name = SR.CommandZoom;
			_command.BeginState = _applicator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (!_operation.AppliesTo(this.SelectedPresentationImage) || _command == null)
				return;

			_applicator.ApplyToLinkedImages();

			_command.EndState = _applicator.CreateMemento();

			// If the state hasn't changed since MouseDown just return
			if (!_command.EndState.Equals(_command.BeginState))
				this.Context.Viewer.CommandHistory.AddCommand(_command);

			_command = null;
		}

		private void ZoomIn()
		{
			CaptureBeginState();

			float increment = 0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			IncrementZoom(increment);

			CaptureEndState();
		}

		private void ZoomOut()
		{
			CaptureBeginState();

			float increment = -0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			IncrementZoom(increment);

			CaptureEndState();
		}

		private void IncrementZoom(float scaleIncrement)
		{
			if (!_operation.AppliesTo(this.SelectedPresentationImage))
				return;

			IImageSpatialTransform transform = (IImageSpatialTransform)this.SelectedSpatialTransformProvider.SpatialTransform;
			transform.ScaleToFit = false;
			transform.Scale += scaleIncrement;

			this.SelectedSpatialTransformProvider.Draw();
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			base.Start(mouseInformation);

			CaptureBeginState();

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			base.Track(mouseInformation);

			IncrementZoom((float)base.DeltaY * 0.025F);

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			base.Stop(mouseInformation);

			CaptureEndState();
			
			return false;
		}

		public override void Cancel()
		{
			this.CaptureEndState();
		}

		protected override void StartWheel()
		{
			CaptureBeginState();
		}

		protected override void StopWheel()
		{
			CaptureEndState();
		}

		protected override void WheelDown()
		{
			float increment = 0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			IncrementZoom(increment);
		}

		protected override void WheelUp()
		{
			float increment = -0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			IncrementZoom(increment);
		}

		public void Apply(IPresentationImage image)
		{
			IImageSpatialTransform transform = (IImageSpatialTransform)_operation.GetOriginator(image);
			transform.ScaleToFit = false;
			transform.Scale = this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
		}
	}
}
