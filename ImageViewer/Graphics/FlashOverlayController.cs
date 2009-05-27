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
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// Controller for generating colour <see cref="ImageGraphic"/>s that flash up on an
	/// <see cref="IPresentationImage"/>'s overlay and disappear shortly thereafter.
	/// </summary>
	/// <example lang="CS">
	/// <![CDATA[
	/// FlashOverlayController controller = new FlashOverlayController("Icons.CreateKeyImageToolLarge.png", new ResourceResolver(this.GetType(), false));
	/// controller.Flash(base.SelectedPresentationImage);
	/// ]]>
	/// </example>
	public class FlashOverlayController
	{
		private delegate void DoFlashDelegate(IPresentationImage image, SynchronizationContext syncContext);
		private readonly DoFlashDelegate _doFlashDelegate;
		private readonly byte[] _pixelData;
		private readonly int _rows, _columns;
		private int _flashSpeed = 500;

		/// <summary>
		/// Constructs a default controller.
		/// </summary>
		/// <remarks>
		/// If a base class uses this form of the constructor then it must also override
		/// the <see cref="GetPixelData"/>, <see cref="Rows"/> and <see cref="Columns"/> virtual members.
		/// </remarks>
		protected FlashOverlayController()
		{
			_doFlashDelegate = this.DoFlash;
			_pixelData = null;
			_rows = _columns = 0;
		}

		/// <summary>
		/// Constructs a controller that uses a 32-bit colour ARGB image specified by the provided pixel data.
		/// </summary>
		/// <param name="colorPixelData">The 32-bit colour ARGB pixel data.</param>
		public FlashOverlayController(ColorPixelData colorPixelData) : this()
		{
			_pixelData = colorPixelData.Raw;
			_rows = colorPixelData.Rows;
			_columns = colorPixelData.Columns;
		}

		/// <summary>
		/// Constructs a controller that uses the 32-bit colour ARGB bitmap specified by the resource name and resource resolver.
		/// </summary>
		/// <param name="resourceName">The partially or fully qualified name of the resource to access.</param>
		/// <param name="resourceResolver">A resource resolver for the resource.</param>
		public FlashOverlayController(string resourceName, IResourceResolver resourceResolver) : this()
		{
			using (Bitmap bitmap = new Bitmap(resourceResolver.OpenResource(resourceName)))
			{
				BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				try
				{
					int length = data.Stride*data.Height;
					_pixelData = new byte[length];
					_rows = data.Height;
					_columns = data.Width;
					Marshal.Copy(data.Scan0, _pixelData, 0, length);
				}
				finally
				{
					bitmap.UnlockBits(data);
				}
			}
		}

		/// <summary>
		/// Gets the pixel data of the flash overlay in 32-bit ARGB format.
		/// </summary>
		/// <returns>An ARGB byte array.</returns>
		protected virtual byte[] GetPixelData()
		{
			return _pixelData;
		}

		/// <summary>
		/// Gets the number of rows in the flash overlay.
		/// </summary>
		protected virtual int Rows
		{
			get { return _rows; }
		}

		/// <summary>
		/// Gets the number of columns in the flash overlay.
		/// </summary>
		protected virtual int Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// Gets or sets the delay time of the flash overlay in milliseconds.
		/// </summary>
		public int FlashSpeed
		{
			get { return _flashSpeed; }
			set { _flashSpeed = value; }
		}

		/// <summary>
		/// Flashes the overlay on the specified image.
		/// </summary>
		/// <param name="image">The image on which to display the overlay. The image must implement <see cref="IOverlayGraphicsProvider"/>.</param>
		public void Flash(IPresentationImage image)
		{
			_doFlashDelegate.Invoke(image, SynchronizationContext.Current);
		}

		/// <summary>
		/// Flashes the overlay on the specified image asynchronously.
		/// </summary>
		/// <param name="image">The image on which to display the overlay. The image must implement <see cref="IOverlayGraphicsProvider"/>.</param>
		/// <param name="userCallback">The method to be called when the asynchronous flash operation is completed. </param>
		/// <param name="state">A user-provided object that distinguishes this particular asynchronous flash request from other requests. </param>
		/// <returns>An <see cref="IAsyncResult"/> that references the asynchronous read.</returns>
		public IAsyncResult BeginFlash(IPresentationImage image, AsyncCallback userCallback, object state)
		{
			return _doFlashDelegate.BeginInvoke(image, SynchronizationContext.Current, userCallback, state);
		}

		/// <summary>
		/// Waits for the pending asynchronous flash to complete.
		/// </summary>
		/// <param name="asyncResult">The reference to the pending asynchronous request to wait for. </param>
		public void EndFlash(IAsyncResult asyncResult)
		{
			_doFlashDelegate.EndInvoke(asyncResult);
		}

		private void DoFlash(IPresentationImage image, SynchronizationContext syncContext)
		{
			if (!(image is IOverlayGraphicsProvider))
				return;

			GraphicCollection overlayGraphics = ((IOverlayGraphicsProvider) image).OverlayGraphics;
			using (FlashOverlayGraphic flashOverlay = new FlashOverlayGraphic(this))
			{
				if (syncContext == null)
				{
					overlayGraphics.Add(flashOverlay);
					image.Draw();
				}
				else
				{
					syncContext.Post(delegate
					                 	{
					                 		overlayGraphics.Add(flashOverlay);
					                 		image.Draw();
					                 	}, null);
				}

				Thread.Sleep(_flashSpeed);

				if (syncContext == null)
				{
					overlayGraphics.Remove(flashOverlay);
					image.Draw();
				}
				else
				{
					syncContext.Post(delegate
					                 	{
					                 		overlayGraphics.Remove(flashOverlay);
					                 		image.Draw();
					                 	}, null);
				}
			}
		}

		private class FlashOverlayGraphic : ColorImageGraphic
		{
			public FlashOverlayGraphic(FlashOverlayController controller) : base(controller.Rows, controller.Columns, controller.GetPixelData) {}

			public override void OnDrawing()
			{
				// This method makes the graphic one time use only... but that's okay because we dispose of it almost immediately
				SpatialTransform transform = base.SpatialTransform;

				PointF srcXAxis = new PointF(1, 0);
				PointF srcOrigin = new PointF(0, 0);
				PointF dstOrigin = transform.ConvertToDestination(srcOrigin);

				// figure out where the positive x axis went to determine the cumulative rotation
				transform.RotationXY = -(int) Vector.SubtendedAngle(transform.ConvertToDestination(srcXAxis) - new SizeF(dstOrigin), srcOrigin, srcXAxis);

				transform.TranslationX = (base.ParentPresentationImage.ClientRectangle.Width - base.Columns)/2 - dstOrigin.X;
				transform.TranslationY = (base.ParentPresentationImage.ClientRectangle.Height - base.Rows)/2 - dstOrigin.Y;

				transform.Scale = 1/transform.CumulativeScale;

				base.OnDrawing();
			}
		}
	}
}