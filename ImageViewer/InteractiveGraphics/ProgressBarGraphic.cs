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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.InteractiveGraphics
{
	public enum ProgressBarGraphicStyle
	{
		Blocks,
		Continuous,
		Marquee,
		Scanner
	}

	[Cloneable(true)]
	public abstract partial class ProgressBarGraphic : CompositeGraphic
	{
		public static ProgressBarGraphic Create(ProgressBarGraphicStyle style)
		{
			switch (style)
			{
				case ProgressBarGraphicStyle.Continuous:
					return new ContinuousProgressBarGraphic();
				case ProgressBarGraphicStyle.Marquee:
					return new MarqueeProgressBarGraphic();
				case ProgressBarGraphicStyle.Scanner:
					return new ScannerProgressBarGraphic();
				case ProgressBarGraphicStyle.Blocks:
				default:
					return new BlocksProgressBarGraphic();
			}
		}

		private float _progress;

		public float Progress
		{
			get { return _progress; }
			set
			{
				Platform.CheckTrue(value >= 0f && value <= 1f, "Progress must be between 0 and 1, inclusive.");
				if (_progress != value)
				{
					_progress = value;
					this.OnProgressChanged();
				}
			}
		}

		public int ProgressInt
		{
			get { return (int) (100*this.Progress); }
			set
			{
				Platform.CheckTrue(value >= 0 && value <= 100, "ProgressInt must be between 0 and 100, inclusive.");
				this.Progress = value/100f;
			}
		}

		public abstract Size Size { get; }

		protected virtual void OnProgressChanged()
		{
			this.Update();
		}

		public override void OnDrawing()
		{
			if (base.Graphics.Count == 0)
			{
				base.Graphics.Add(CreateImageGraphic());
			}

			base.OnDrawing();
		}

		protected void Update()
		{
			DisposeAndClear(base.Graphics);
		}

		protected void DrawImageCentered(System.Drawing.Graphics g, Image image)
		{
			Size bounds = this.Size;
			Size size = image.Size;
			Point offset = new Point((bounds.Width - size.Width)/2, (bounds.Height - size.Height)/2);
			g.DrawImageUnscaledAndClipped(image, new Rectangle(offset, image.Size));
		}

		protected abstract void RenderProgressBar(float progress, System.Drawing.Graphics g);

		#region Base Rendering Support

		private ColorImageGraphic CreateImageGraphic()
		{
			byte[] pixelData;
			using (Bitmap buffer = new Bitmap(this.Size.Width, this.Size.Height))
			{
				using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(buffer))
				{
					RenderProgressBar(_progress, g);
				}
				pixelData = GetPixelData(buffer);
			}
			ColorImageGraphic imageGraphic = new ColorImageGraphic(this.Size.Height, this.Size.Width, pixelData);
			return imageGraphic;
		}

		private static byte[] GetPixelData(Bitmap bitmap)
		{
			byte[] pixelData;
			BitmapData data = bitmap.LockBits(new Rectangle(Point.Empty, bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			try
			{
				int length = data.Stride*data.Height;
				pixelData = new byte[length];
				Marshal.Copy(data.Scan0, pixelData, 0, length);
			}
			finally
			{
				bitmap.UnlockBits(data);
			}
			return pixelData;
		}

		private static void DisposeAndClear<T>(ICollection<T> collection) where T : IDisposable
		{
			var items = new List<T>(collection);
			collection.Clear();
			foreach (T item in items)
				item.Dispose();
		}

		#endregion

		#region Image Resource Sharing

		private static readonly Dictionary<string, Image> _cachedImageResources = new Dictionary<string, Image>();

		internal static Image GetImageResource(string resourceName)
		{
			// simple static resource caching - the progress bar graphical elements only total about 6 kilobytes
			if (!_cachedImageResources.ContainsKey(resourceName))
			{
				var resourceResolver = new ResourceResolver(Assembly.GetExecutingAssembly());
				var image = Image.FromStream(resourceResolver.OpenResource(resourceName));
				_cachedImageResources.Add(resourceName, image);
			}
			return _cachedImageResources[resourceName];
		}

		#endregion
	}
}