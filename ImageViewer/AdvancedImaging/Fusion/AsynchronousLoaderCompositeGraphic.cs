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
using System.Drawing;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.InteractiveGraphics;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable(false)]
	internal class AsynchronousLoaderCompositeGraphic : CompositeGraphic
	{
		[CloneIgnore]
		private SynchronizationContext _synchronizationContext;

		[CloneIgnore]
		private IFusionOverlaySliceReference _overlaySliceReference;

		[CloneIgnore]
		private BackgroundTask _progressUpdaterTask;

		[CloneIgnore]
		private ProgressCompositeGraphic _progressCompositeGraphic;

		[CloneIgnore]
		private GrayscaleImageGraphic _fusionOverlayImageGraphic;

		public AsynchronousLoaderCompositeGraphic(FusionOverlaySlice overlaySlice)
		{
			_overlaySliceReference = overlaySlice.CreateTransientReference();
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected AsynchronousLoaderCompositeGraphic(AsynchronousLoaderCompositeGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_overlaySliceReference = source._overlaySliceReference.Clone();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_fusionOverlayImageGraphic = null;
				_progressCompositeGraphic = null;
				_synchronizationContext = null;

				if (_overlaySliceReference != null)
				{
					_overlaySliceReference.Dispose();
					_overlaySliceReference = null;
				}

				if (_progressUpdaterTask != null)
				{
					_progressUpdaterTask.RequestCancel();
					_progressUpdaterTask = null;
				}
			}

			base.Dispose(disposing);
		}

		public FusionOverlaySlice OverlaySlice
		{
			get { return _overlaySliceReference.FusionOverlaySlice; }
		}

		public override void OnDrawing()
		{
			if (_synchronizationContext == null)
				_synchronizationContext = SynchronizationContext.Current;

			if (_fusionOverlayImageGraphic == null)
			{
				//BackgroundTask bt = new BackgroundTask(this.GetOverlayImageGraphic, false);
				//ProgressGraphic.Show(bt, _fusionOverlayLayer.Graphics, true, ProgressBarGraphicStyle.Marquee);

				GetOverlayImageGraphic();
			}
			base.OnDrawing();
		}

		private void GetOverlayImageGraphic()
		{
			float progress;
			string message;
			if (_overlaySliceReference.FusionOverlaySlice.CheckIsLoaded(out progress, out message))
			{
				if (_progressCompositeGraphic != null)
				{
					base.Graphics.Remove(_progressCompositeGraphic);
					_progressCompositeGraphic.Dispose();
					_progressCompositeGraphic = null;
				}

				if (_progressUpdaterTask != null)
				{
					_progressUpdaterTask.RequestCancel();
					_progressUpdaterTask = null;
				}

				var overlayImageGraphic = _overlaySliceReference.FusionOverlaySlice.CreateImageGraphic();
				if (overlayImageGraphic != null)
				{
					//_voiLutManagerProxy.SetOverlayVoiLutManager(overlayImageGraphic.VoiLutManager);
					//_overlayColorMapSpec.SetOverlayColorMapManager(overlayImageGraphic.ColorMapManager);
					base.Graphics.Add(_fusionOverlayImageGraphic = overlayImageGraphic);
				}
			}
			else
			{
				if (_progressCompositeGraphic == null)
				{
					base.Graphics.Add(_progressCompositeGraphic = new ProgressCompositeGraphic(ProgressBarGraphicStyle.Continuous));
				}
				_progressCompositeGraphic.Progress = progress;
				_progressCompositeGraphic.Text = message;

				if (_progressUpdaterTask == null)
				{
					_progressUpdaterTask = new BackgroundTask(TheShrew, true, null);
					_progressUpdaterTask.Terminated += _progressUpdaterTask_Terminated;
					_progressUpdaterTask.Run();
				}
			}
		}

		private void _progressUpdaterTask_Terminated(object sender, BackgroundTaskTerminatedEventArgs e)
		{
			BackgroundTask task = sender as BackgroundTask;
			if (task != null)
			{
				task.Dispose();
			}
			_progressUpdaterTask = null;
		}

		private void TheShrew(IBackgroundTaskContext context)
		{
			while (!context.CancelRequested)
			{
				if (!context.CancelRequested)
				{
					ThePossum thePossum = new ThePossum();
					_synchronizationContext.Send(TheOpposum, thePossum);
					if (!thePossum.Continue)
						break;
				}
				Thread.Sleep(100);
			}
		}

		private class ThePossum
		{
			public bool Continue;
		}

		private void TheOpposum(object state)
		{
			if (this.ParentPresentationImage.Visible)
			{
				this.Draw();
				((ThePossum) state).Continue = true;
			}
			else
			{
				((ThePossum) state).Continue = false;
			}
		}

		#region Asdf

		[Cloneable]
		internal class ProgressCompositeGraphic : CompositeGraphic
		{
			[CloneIgnore]
			private readonly Size _size = new Size(240, 80);

			[CloneIgnore]
			private ITextGraphic _progressTextGraphic;

			[CloneIgnore]
			private ProgressBarGraphic _progressBarGraphic;

			[CloneIgnore]
			private ColorImageGraphic _background;

			[CloneIgnore]
			private readonly ProgressBarGraphicStyle _style;

			public ProgressCompositeGraphic(ProgressBarGraphicStyle style)
			{
				_style = style;

				Initialize();
			}

			/// <summary>
			/// Cloning constructor.
			/// </summary>
			/// <param name="source">The source object from which to clone.</param>
			/// <param name="context">The cloning context object.</param>
			protected ProgressCompositeGraphic(ProgressCompositeGraphic source, ICloningContext context)
			{
				context.CloneFields(source, this);

				_style = source._style;
			}

			[OnCloneComplete]
			private void OnCloneComplete()
			{
				_progressTextGraphic = (ITextGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ITextGraphic);
				_progressBarGraphic = (ProgressBarGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ProgressBarGraphic);
				_background = (ColorImageGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is ColorImageGraphic);

				Initialize();
			}

			private void Initialize()
			{
				if (_background == null)
				{
					byte[] pixelData = new byte[4*_size.Width*_size.Height];
					for (int n = 0; n < pixelData.Length; n += 4)
					{
						byte[] pixel = BitConverter.GetBytes(Color.FromArgb(196, 85, 85, 85).ToArgb());
						pixelData[n + 0] = pixel[0];
						pixelData[n + 1] = pixel[1];
						pixelData[n + 2] = pixel[2];
						pixelData[n + 3] = pixel[3];
					}
					base.Graphics.Add(_background = new ColorImageGraphic(_size.Height, _size.Width, pixelData));
				}

				if (_progressBarGraphic == null)
				{
					base.Graphics.Add(_progressBarGraphic = ProgressBarGraphic.Create(_style));
					var offset = Center(_size, _progressBarGraphic.Size) + new Size(0, 10);
					_progressBarGraphic.SpatialTransform.TranslationX = offset.X;
					_progressBarGraphic.SpatialTransform.TranslationY = offset.Y;
				}

				if (_progressTextGraphic == null)
				{
					base.Graphics.Add(_progressTextGraphic = new InvariantTextPrimitive());
					var offset = Center(_size, new Size(1, 1)) - new Size(0, 15);
					_progressTextGraphic.SpatialTransform.TranslationX = offset.X;
					_progressTextGraphic.SpatialTransform.TranslationY = offset.Y;
				}
			}

			public override void OnDrawing()
			{
				if (base.ParentPresentationImage != null)
				{
					// We do graphics on a separate composite so that we can constantly fix our spatial transformation to center the contents in the client area
					SpatialTransform transform = base.SpatialTransform;

					float scale = 1/base.ParentGraphic.SpatialTransform.CumulativeScale;

					PointF srcXAxis = new PointF(1, 0);
					PointF srcOrigin = new PointF(0, 0);
					PointF dstOrigin = base.ParentGraphic.SpatialTransform.ConvertToDestination(srcOrigin);

					// figure out where the positive x axis went to determine the cumulative rotation
					transform.RotationXY = -(int) Vector.SubtendedAngle(base.ParentGraphic.SpatialTransform.ConvertToDestination(srcXAxis) - new SizeF(dstOrigin), srcOrigin, srcXAxis);

					transform.TranslationX = (base.ParentPresentationImage.ClientRectangle.Width - this.Width)/2f - dstOrigin.X;
					transform.TranslationY = (base.ParentPresentationImage.ClientRectangle.Height - this.Height)/2f - dstOrigin.Y;

					transform.Scale = scale;
				}

				base.OnDrawing();
			}

			public string Text
			{
				get { return _progressTextGraphic.Text; }
				set { _progressTextGraphic.Text = value; }
			}

			public float Progress
			{
				get { return _progressBarGraphic.Progress; }
				set { _progressBarGraphic.Progress = value; }
			}

			public ProgressBarGraphicStyle Style
			{
				get { return _style; }
			}

			public Size Size
			{
				get { return _size; }
			}

			public int Width
			{
				get { return _size.Width; }
			}

			public int Height
			{
				get { return _size.Height; }
			}

			private static Point Center(Size bounds, Size size)
			{
				return new Point((bounds.Width - size.Width)/2, (bounds.Height - size.Height)/2);
			}
		}

		#endregion
	}
}