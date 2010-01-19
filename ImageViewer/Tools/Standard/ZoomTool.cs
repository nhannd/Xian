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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.InputManagement;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuZoom", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuZoom", "Select", Flags = ClickActionFlags.CheckAction)]
	[DropDownButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarZoom", "Select", "ZoomDropDownMenuModel", Flags = ClickActionFlags.CheckAction, KeyStroke = XKeys.Z)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", IconScheme.Colour, "Icons.ZoomToolSmall.png", "Icons.ZoomToolMedium.png", "Icons.ZoomToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Zoom")]

	[KeyboardAction("zoomin", "imageviewer-keyboard/ToolsStandardZoom/ZoomIn", "ZoomIn", KeyStroke = XKeys.OemPeriod)]
	[KeyboardAction("zoomout", "imageviewer-keyboard/ToolsStandardZoom/ZoomOut", "ZoomOut", KeyStroke = XKeys.Oemcomma)]

	[MouseWheelHandler(ModifierFlags.Control)]
	[MouseToolButton(XMouseButtons.Right, false)]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class ZoomTool : MouseImageViewerTool
	{
		internal static readonly float DefaultMinimumZoom = 0.25F;
		internal static readonly float DefaultMaximumZoom = 64F;

		private readonly bool _invertedOperation;
		private readonly ImageSpatialTransformImageOperation _operation; 
		private MemorableUndoableCommand _memorableCommand;
		private ImageOperationApplicator _applicator;

		public ZoomTool()
			: base(SR.TooltipZoom)
		{
			this.CursorToken = new CursorToken("Icons.ZoomToolSmall.png", this.GetType().Assembly);
			_operation = new ImageSpatialTransformImageOperation(Apply);

			try
			{
				_invertedOperation = ToolSettings.Default.InvertedZoomToolOperation;
			}
			catch(Exception)
			{
				_invertedOperation = false;
			}
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		public ActionModelNode ZoomDropDownMenuModel
		{
			get
			{
				SimpleActionModel actionModel = new SimpleActionModel(new ResourceResolver(this.GetType().Assembly));

				actionModel.AddAction("fit", SR.LabelZoomFit, null, SR.LabelZoomFit, delegate { SetScale(0); });
				AddFixedZoomAction(actionModel, 1);
				AddFixedZoomAction(actionModel, 2);
				AddFixedZoomAction(actionModel, 4);
				AddFixedZoomAction(actionModel, 8);

				return actionModel;
			}	
		}

		private ImageSpatialTransform GetSelectedImageTransform()
		{
			return _operation.GetOriginator(this.SelectedPresentationImage) as ImageSpatialTransform;
		}

		private bool CanZoom()
		{
			return GetSelectedImageTransform() != null;
		}

		private void AddFixedZoomAction(SimpleActionModel actionModel, int scale)
		{
			string label = String.Format(SR.FormatLabelZoomFixed, scale);
			actionModel.AddAction("fixedzoom" + label, label, null, label, delegate { SetScale(scale); });
		}

		private void CaptureBeginState()
		{
			if (!CanZoom())
				return;

			ImageSpatialTransform originator = GetSelectedImageTransform();
			_applicator = new ImageOperationApplicator(this.SelectedPresentationImage, _operation);
			_memorableCommand = new MemorableUndoableCommand(originator);
			_memorableCommand.BeginState = originator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (!CanZoom() || _memorableCommand == null)
				return;

			_memorableCommand.EndState = GetSelectedImageTransform().CreateMemento();
			UndoableCommand applicatorCommand = _applicator.ApplyToLinkedImages();
			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(this.SelectedPresentationImage);

			if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
				historyCommand.Enqueue(_memorableCommand);
			if (applicatorCommand != null)
				historyCommand.Enqueue(applicatorCommand);

			if (historyCommand.Count > 0)
			{
				historyCommand.Name = SR.CommandZoom;
				this.Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}

			_memorableCommand = null;
		}
		
		private void ZoomIn()
		{
			CaptureBeginState();

			float increment = 0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			IncrementScale(increment);

			CaptureEndState();
		}

		private void ZoomOut()
		{
			CaptureBeginState();

			float increment = -0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			IncrementScale(increment);

			CaptureEndState();
		}

		private void SetScale(float scale)
		{
			if (this.SelectedPresentationImage == null)
				return;

			CaptureBeginState();

			if (!CanZoom())
				return;

			IImageSpatialTransform transform = GetSelectedImageTransform();
			if (scale <= 0)
			{
				transform.ScaleToFit = true;
			}
			else
			{
				transform.ScaleToFit = false;
				transform.Scale = scale;
			}

			CaptureEndState();

			this.SelectedSpatialTransformProvider.Draw();
		}
		
		private void IncrementScale(float scaleIncrement)
		{
			if (!CanZoom())
				return;

			IImageSpatialTransform transform = GetSelectedImageTransform();

			float currentScale = transform.Scale;

			// Use the 'to fit' scale value to calculate a minimum scale value.
			transform.ScaleToFit = true;
			float minimum = transform.Scale;
			
			// in the case of ridiculously small client rectangles, don't allow the scale to get any smaller than it is.
			if (base.SelectedPresentationImage.ClientRectangle.Width > 32 && 
				base.SelectedPresentationImage.ClientRectangle.Height > 32)
				minimum /= 2;

			// Set the minimum scale to 1/2 the size of the 'to fit' scale value, or the default, whichever is smaller.
			minimum = Math.Min(minimum, DefaultMinimumZoom);
			// When the scale is already smaller than the 'preferred' minimum, just don't let it get any smaller.
			minimum = Math.Min(minimum, currentScale);

			//make sure to reset the scale to what it was before we calculated the minimum.
			transform.ScaleToFit = false;
			transform.Scale = currentScale;

			float newScale = currentScale + scaleIncrement;
			if (newScale < minimum)
				newScale = minimum;
			else if (newScale > DefaultMaximumZoom)
				newScale = DefaultMaximumZoom;

			transform.Scale = newScale;

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

			float increment = -base.DeltaY*0.025f;
			increment *= _invertedOperation ? -1f : 1f;
			IncrementScale(increment);

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

		public override void StartWheel()
		{
			CaptureBeginState();
		}

		public override void StopWheel()
		{
			CaptureEndState();
		}

		protected override void WheelBack()
		{
			float increment = -0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			increment *= _invertedOperation ? -1f : 1f;
			IncrementScale(increment);
		}

		protected override void WheelForward()
		{
			float increment = 0.1F * this.SelectedSpatialTransformProvider.SpatialTransform.Scale;
			increment *= _invertedOperation ? -1f : 1f;
			IncrementScale(increment);
		}

		public void Apply(IPresentationImage image)
		{
			IImageSpatialTransform transform = (IImageSpatialTransform)_operation.GetOriginator(image);
			IImageSpatialTransform referenceTransform = (IImageSpatialTransform)this.SelectedSpatialTransformProvider.SpatialTransform;

			transform.Scale = referenceTransform.Scale;
			transform.ScaleToFit = referenceTransform.ScaleToFit;
		}
	}
}
