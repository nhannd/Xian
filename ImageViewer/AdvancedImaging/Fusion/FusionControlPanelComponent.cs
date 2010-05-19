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
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[ExtensionPoint]
	public sealed class FusionControlPanelComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView> {}

	[AssociateView(typeof (FusionControlPanelComponentViewExtensionPoint))]
	public class FusionControlPanelComponent : ImageViewerToolComponent
	{
		private float _overlayAlpha = 0.5f;
		private float _overlayThreshold = 0.1f;

		public FusionControlPanelComponent(IDesktopWindow desktopWindow)
			: base(desktopWindow) {}

		[ValidateGreaterThan(0f, Inclusive = true, Message = "Alpha must be a value between 0 and 1.")]
		[ValidateLessThan(1f, Inclusive = true, Message = "Alpha must be a value between 0 and 1.")]
		public float OverlayAlpha
		{
			get { return _overlayAlpha; }
			set
			{
				if (_overlayAlpha != value)
				{
					_overlayAlpha = value;
					base.NotifyPropertyChanged("OverlayAlpha");
					this.UpdateOverlay();
				}
			}
		}

		[ValidateGreaterThan(0f, Inclusive = true, Message = "Threshold must be a value between 0 and 1.")]
		[ValidateLessThan(1f, Inclusive = true, Message = "Threshold must be a value between 0 and 1.")]
		public float OverlayThreshold
		{
			get { return _overlayThreshold; }
			set
			{
				if (_overlayThreshold != value)
				{
					_overlayThreshold = value;
					base.NotifyPropertyChanged("OverlayThreshold");
					this.UpdateOverlay();
				}
			}
		}

		protected virtual void UpdateOverlay()
		{
			if (this.ImageViewer != null && this.ImageViewer.SelectedPresentationImage is FusionPresentationImage)
			{
				var image = (FusionPresentationImage) this.ImageViewer.SelectedPresentationImage;

				image.OverlayAlpha = Restrict(_overlayAlpha, 0, 1);
				image.OverlayThreshold = Restrict(_overlayThreshold, 0, 1);

				image.Draw();
			}
		}

		protected override void OnActiveImageViewerChanged(ActiveImageViewerChangedEventArgs e)
		{
			base.OnActiveImageViewerChanged(e);

			if (e.DeactivatedImageViewer != null)
			{
				e.DeactivatedImageViewer.EventBroker.PresentationImageSelected -= EventBroker_PresentationImageSelected;
			}

			if (e.ActivatedImageViewer != null)
			{
				e.ActivatedImageViewer.EventBroker.PresentationImageSelected += EventBroker_PresentationImageSelected;
			}
		}

		public override void Stop()
		{
			if (base.ImageViewer != null)
			{
				base.ImageViewer.EventBroker.PresentationImageSelected -= EventBroker_PresentationImageSelected;
			}

			base.Stop();
		}

		private void EventBroker_PresentationImageSelected(object sender, PresentationImageSelectedEventArgs e)
		{
			UpdateOverlay();
		}

		private static float Restrict(float value, float min, float max)
		{
			return Math.Max(Math.Min(value, max), min);
		}
	}
}