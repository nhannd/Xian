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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable(false)]
	internal class FusionOverlayLoaderCompositeGraphic : CompositeGraphic
	{
		private event EventHandler _overlayImageGraphicChanged;

		[CloneIgnore]
		private IFusionOverlaySliceReference _overlaySliceReference;

		[CloneIgnore]
		private GrayscaleImageGraphic _overlayImageGraphic;

		[CloneIgnore]
		private bool _isLoadingProgressShown;

		public FusionOverlayLoaderCompositeGraphic(FusionOverlaySlice overlaySlice)
		{
			_overlaySliceReference = overlaySlice.CreateTransientReference();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected FusionOverlayLoaderCompositeGraphic(FusionOverlayLoaderCompositeGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_overlaySliceReference = source._overlaySliceReference.Clone();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_overlayImageGraphic = null;

				if (_overlaySliceReference != null)
				{
					_overlaySliceReference.Dispose();
					_overlaySliceReference = null;
				}
			}

			base.Dispose(disposing);
		}

		public FusionOverlaySlice OverlaySlice
		{
			get { return _overlaySliceReference.FusionOverlaySlice; }
		}

		public GrayscaleImageGraphic OverlayImageGraphic
		{
			get { return _overlayImageGraphic; }
			private set
			{
				if (_overlayImageGraphic != value)
				{
					if (_overlayImageGraphic != null)
					{
						base.Graphics.Remove(_overlayImageGraphic);
						_overlayImageGraphic.Dispose();
					}

					_overlayImageGraphic = value;

					if (_overlayImageGraphic != null)
					{
						base.Graphics.Add(_overlayImageGraphic);
					}

					OnOverlayImageGraphicChanged();
				}
			}
		}

		public event EventHandler OverlayImageGraphicChanged
		{
			add { _overlayImageGraphicChanged += value; }
			remove { _overlayImageGraphicChanged -= value; }
		}

		protected virtual void OnOverlayImageGraphicChanged()
		{
			EventsHelper.Fire(_overlayImageGraphicChanged, this, EventArgs.Empty);
		}

		public override void OnDrawing()
		{
			if (_overlayImageGraphic == null)
			{
				float progress;
				string message;
				if (_overlaySliceReference.FusionOverlaySlice.BeginLoad(out progress, out message))
				{
					OverlayImageGraphic = _overlaySliceReference.FusionOverlaySlice.CreateImageGraphic();
					_isLoadingProgressShown = false;
				}
				else if (!_isLoadingProgressShown)
				{
					_isLoadingProgressShown = true;
					this.Graphics.Add(new ProgressGraphic(_overlaySliceReference.FusionOverlaySlice, true, ProgressBarGraphicStyle.Continuous));
				}
			}
			base.OnDrawing();
		}
	}
}