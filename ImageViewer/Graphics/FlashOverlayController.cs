#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
            DoFlash(image, SynchronizationContext.Current);
		}

        private void DoFlash(IPresentationImage image, SynchronizationContext syncContext)
        {
            if (!(image is IOverlayGraphicsProvider))
                return;

            GraphicCollection overlayGraphics = ((IOverlayGraphicsProvider) image).OverlayGraphics;
            //Prevent multiple flash graphics per image.
            var existing = overlayGraphics.OfType<FlashOverlayGraphic>().Where(g => g.Controller == this).FirstOrDefault();
            if (existing != null)
                return;

            //Very little utility in flashing if nobody's looking at it.
            if (syncContext == null)
                return;

            var flashOverlayGraphic = new FlashOverlayGraphic(this);
            overlayGraphics.Add(flashOverlayGraphic);
            image.Draw();

            ThreadPool.QueueUserWorkItem(
                delegate
                    {
                        Thread.Sleep(FlashSpeed);
                        syncContext.Post(delegate
                                             {
                                                 if (flashOverlayGraphic.IsDisposed)
                                                     return;

                                                 overlayGraphics.Remove(flashOverlayGraphic);
                                                 image.Draw();
                                             }, null);
                    }, null);
        }

	    private class FlashOverlayGraphic : ColorImageGraphic
		{
		    public FlashOverlayGraphic(FlashOverlayController controller) : base(controller.Rows, controller.Columns, controller.GetPixelData)
			{
		        Controller = controller;
			}

		    public FlashOverlayController Controller { get; private set; }
            public bool IsDisposed { get; private set; }

		    protected override SpatialTransform CreateSpatialTransform()
			{
				return new InvariantSpatialTransform(this);
			}

			public override void OnDrawing()
			{
				if (base.ParentPresentationImage != null)
				{
					var clientRectangle = base.ParentPresentationImage.ClientRectangle;
					var spatialTransform = base.SpatialTransform;
					spatialTransform.TranslationX = (clientRectangle.Width - Columns)/2f;
					spatialTransform.TranslationY = (clientRectangle.Height - Rows)/2f;
				}
				base.OnDrawing();
			}

            protected override void Dispose(bool disposing)
            {
                IsDisposed = true;
                base.Dispose(disposing);
            }
		}
	}
}