#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[MenuAction("activate", "imageviewer-contextmenu/MenuAdjustOpacity", "Select", Flags = ClickActionFlags.CheckAction)]
	[MenuAction("activate", "global-menus/MenuTools/MenuAdvanced/MenuAdjustOpacity", "Select", Flags = ClickActionFlags.CheckAction)]
	[ButtonAction("activate", "global-toolbars/ToolbarAdvanced/ToolbarAdjustOpacity", "Select", Flags = ClickActionFlags.CheckAction)]
	[CheckedStateObserver("activate", "Active", "ActivationChanged")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[TooltipValueObserver("activate", "Tooltip", "TooltipChanged")]
	[MouseButtonIconSet("activate", IconScheme.Colour, "Icons.AdjustOpacityToolSmall.png", "Icons.AdjustOpacityToolMedium.png", "Icons.AdjustOpacityToolLarge.png")]
	[MouseToolButton(XMouseButtons.Right, false)]
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class AdjustOpacityTool : MouseImageViewerTool
	{
		private readonly LayerOpacityOperation _operation;
		private MemorableUndoableCommand _memorableCommand;
		private ImageOperationApplicator _applicator;

		public AdjustOpacityTool()
			: base(SR.TooltipAdjustOpacity)
		{
			this.CursorToken = new CursorToken("Icons.AdjustOpacityToolSmall.png", this.GetType().Assembly);
			_operation = new LayerOpacityOperation(Apply);
		}

		private float CurrentSensitivity
		{
			get { return 0.001f; }
		}

		private ILayerOpacityProvider SelectedLayerOpacityProvider
		{
			get { return this.SelectedPresentationImage as ILayerOpacityProvider; }
		}

		public override event EventHandler TooltipChanged
		{
			add { base.TooltipChanged += value; }
			remove { base.TooltipChanged -= value; }
		}

		private ILayerOpacityManager GetSelectedLayerOpacityManager()
		{
			return _operation.GetOriginator(this.SelectedPresentationImage) as ILayerOpacityManager;
		}

		private bool CanAdjustAlpha()
		{
			ILayerOpacityManager manager = GetSelectedLayerOpacityManager();
			return manager != null && manager.Enabled;
		}

		protected override void OnPresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			this.Enabled = (e.SelectedPresentationImage is ILayerOpacityProvider);

			base.OnPresentationImageSelected(sender, e);
		}

		private void CaptureBeginState()
		{
			if (!CanAdjustAlpha())
				return;

			ILayerOpacityManager originator = GetSelectedLayerOpacityManager();
			_applicator = new ImageOperationApplicator(this.SelectedPresentationImage, _operation);
			_memorableCommand = new MemorableUndoableCommand(originator);
			_memorableCommand.BeginState = originator.CreateMemento();
		}

		private void CaptureEndState()
		{
			if (!CanAdjustAlpha() || _memorableCommand == null)
				return;

			if (this.SelectedLayerOpacityProvider != null)
			{
				_memorableCommand.EndState = GetSelectedLayerOpacityManager().CreateMemento();
				UndoableCommand applicatorCommand = _applicator.ApplyToLinkedImages();
				DrawableUndoableCommand historyCommand = new DrawableUndoableCommand(this.SelectedPresentationImage);

				if (!_memorableCommand.EndState.Equals(_memorableCommand.BeginState))
					historyCommand.Enqueue(_memorableCommand);
				if (applicatorCommand != null)
					historyCommand.Enqueue(applicatorCommand);

				if (historyCommand.Count > 0)
				{
					historyCommand.Name = SR.CommandAdjustOpacity;
					this.Context.Viewer.CommandHistory.AddCommand(historyCommand);
				}
			}
		}

		private void IncrementOpacity(float opacityIncrement)
		{
			if (!CanAdjustAlpha())
				return;

			ILayerOpacityManager manager = this.SelectedLayerOpacityProvider.LayerOpacityManager;

			manager.Opacity = Restrict(manager.Opacity + opacityIncrement, 0, 1);

			this.SelectedLayerOpacityProvider.Draw();
		}

		private void IncrementOpacityWithUndo(float opacityIncrement)
		{
			this.CaptureBeginState();
			this.IncrementOpacity(opacityIncrement);
			this.CaptureEndState();
		}

		private void Apply(IPresentationImage image)
		{
			ILayerOpacityManager manager = this.SelectedLayerOpacityProvider.LayerOpacityManager;

			ILayerOpacityProvider provider = ((ILayerOpacityProvider)image);

			ILayerOpacityManager lut = provider.LayerOpacityManager;
			lut.Opacity = Restrict(manager.Opacity, 0, 1);
		}

		public override bool Start(IMouseInformation mouseInformation)
		{
			if (this.SelectedLayerOpacityProvider == null)
				return false;

			base.Start(mouseInformation);

			CaptureBeginState();

			return true;
		}

		public override bool Track(IMouseInformation mouseInformation)
		{
			if (this.SelectedLayerOpacityProvider == null)
				return false;

			base.Track(mouseInformation);

			double sensitivity = this.CurrentSensitivity;
			double magnitude = Math.Sqrt(DeltaX*DeltaX + DeltaY*DeltaY);
			double angle = Math.Atan2(DeltaY, DeltaX);
			double sign = angle >= -Math.PI/4 && angle < 3*Math.PI/4 ? 1 : -1;
			IncrementOpacity((float) (sign*magnitude*sensitivity));

			return true;
		}

		public override bool Stop(IMouseInformation mouseInformation)
		{
			if (this.SelectedLayerOpacityProvider == null)
				return false;

			base.Stop(mouseInformation);

			this.CaptureEndState();

			return false;
		}

		public override void Cancel()
		{
			if (this.SelectedLayerOpacityProvider == null)
				return;

			this.CaptureEndState();
		}

		private static float Restrict(float value, float min, float max)
		{
			return Math.Max(Math.Min(value, max), min);
		}
	}
}