#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
	[MenuAction("activate", "imageviewer-contextmenu/MenuPan", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuStandard/MenuPan", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarStandard/ToolbarPan", "Select", Flags = ClickActionFlags.CheckAction)]
	[KeyboardAction("activate", "imageviewer-keyboard/ToolsStandardPan/Activate", "Select", KeyStroke = XKeys.P)]
    [CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[IconSet("activate", IconScheme.Colour, "Icons.PanToolSmall.png", "Icons.PanToolMedium.png", "Icons.PanToolLarge.png")]
	[GroupHint("activate", "Tools.Image.Manipulation.Pan")]

	[KeyboardAction("panleft", "imageviewer-keyboard/ToolsStandardPan/PanLeft", "PanLeft", KeyStroke = XKeys.Control | XKeys.Left)]
	[KeyboardAction("panright", "imageviewer-keyboard/ToolsStandardPan/PanRight", "PanRight", KeyStroke = XKeys.Control | XKeys.Right)]
	[KeyboardAction("panup", "imageviewer-keyboard/ToolsStandardPan/PanUp", "PanUp", KeyStroke = XKeys.Control | XKeys.Up)]
	[KeyboardAction("pandown", "imageviewer-keyboard/ToolsStandardPan/PanDown", "PanDown", KeyStroke = XKeys.Control | XKeys.Down)]

	[MouseToolButton(XMouseButtons.Left, false)]
	[DefaultMouseToolButton(XMouseButtons.Left, ModifierFlags.Control)]
    
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class PanTool : MouseImageViewerTool
	{
		private readonly SpatialTransformImageOperation _operation;
		private MemorableUndoableCommand _memorableCommand;
		private ImageOperationApplicator _applicator;

		public PanTool()
			: base(SR.TooltipPan)
		{
			this.CursorToken = new CursorToken("Icons.PanToolSmall.png", this.GetType().Assembly);
			_operation = new SpatialTransformImageOperation(Apply);
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		private ISpatialTransform GetSelectedImageTransform()
		{
			return _operation.GetOriginator(this.SelectedPresentationImage) as ISpatialTransform;
		}

		private bool CanPan()
		{
			return GetSelectedImageTransform() != null;
		}

		private void CaptureBeginState()
		{
			if (!CanPan())
				return;

			_applicator = new ImageOperationApplicator(this.SelectedPresentationImage, _operation);
			IMemorable originator = GetSelectedImageTransform();
			_memorableCommand = new MemorableUndoableCommand(originator);
			_memorableCommand.BeginState = originator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (!CanPan() || _memorableCommand == null)
				return;

			_memorableCommand.EndState = GetSelectedImageTransform().CreateMemento();
			CompositeUndoableCommand applicatorCommand = _applicator.ApplyToLinkedImages();
			DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(this.SelectedPresentationImage);

			if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
				historyCommand.Enqueue(_memorableCommand);
			if (applicatorCommand != null)
				historyCommand.Enqueue(applicatorCommand);

			if (historyCommand.Count > 0)
			{
				historyCommand.Name = SR.CommandPan;
				this.Context.Viewer.CommandHistory.AddCommand(historyCommand);
			}

			_memorableCommand = null;
		}

		private void PanLeft()
		{
			IncrementPanWithUndo(-15, 0);
		}

		private void PanRight()
		{
			IncrementPanWithUndo(15, 0);
		}

		private void PanUp()
		{
			IncrementPanWithUndo(0, -15);
		}

		private void PanDown()
		{
			IncrementPanWithUndo(0, 15);
		}

		private void IncrementPanWithUndo(int xIncrement, int yIncrement)
		{
			if (!CanPan())
				return;

			this.CaptureBeginState();
			this.IncrementPan(xIncrement, yIncrement);
			this.CaptureEndState();
		}

		private void IncrementPan(int xIncrement, int yIncrement)
		{
			if (!CanPan())
				return;

			SpatialTransform transform = (SpatialTransform)_operation.GetOriginator(this.SelectedPresentationImage);

			// Because the pan increment is in destination coordinates, we have to convert
			// them to source coordinates, since the transform translation is in source coordinates.
			// This will allow the pan to work properly irrespective of the zoom, flip and rotation.
			
			SizeF sourceIncrement = transform.ConvertToSource(new SizeF(xIncrement, yIncrement));

			transform.TranslationX += sourceIncrement.Width;
			transform.TranslationY += sourceIncrement.Height;

			this.SelectedPresentationImage.Draw();
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

			this.IncrementPan(base.DeltaX, base.DeltaY);

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

		public void Apply(IPresentationImage image)
		{
			ISpatialTransform transform = (ISpatialTransform)_operation.GetOriginator(image);
			transform.TranslationX = this.SelectedSpatialTransformProvider.SpatialTransform.TranslationX;
			transform.TranslationY = this.SelectedSpatialTransformProvider.SpatialTransform.TranslationY;
		}
	}
}
