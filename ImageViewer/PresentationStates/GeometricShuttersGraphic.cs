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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.PresentationStates
{
	/// <summary>
	/// A display shutter graphic consisting of a combination of a circular, rectangular, and polygonal shutter.
	/// </summary>
	[Cloneable]
	public class GeometricShuttersGraphic : CompositeGraphic, IShutterGraphic
	{
		internal const string DefaultName = "Geometric Shutters";

		private readonly Rectangle _imageRectangle;

		[CloneIgnore]
		private readonly List<GeometricShutter> _dicomShutters;

		[CloneIgnore]
		private readonly ReadOnlyCollection<GeometricShutter> _readOnlyDicomShutters;

		[CloneIgnore]
		private readonly ObservableList<GeometricShutter> _customShutters;

		[CloneIgnore]
		private ColorImageGraphic _imageGraphic;

		[CloneIgnore]
		private byte[] _buffer;

		private Color _fillColor = Color.Black;

		/// <summary>
		/// Constructs a new <see cref="GeometricShuttersGraphic"/> with the specified dimensions.
		/// </summary>
		/// <param name="rows">The number of rows in the display shutter.</param>
		/// <param name="columns">The number of columns in the display shutter.</param>
		public GeometricShuttersGraphic(int rows, int columns)
		{
			_imageRectangle = new Rectangle(0, 0, columns, rows);

			_customShutters = new ObservableList<GeometricShutter>();
			_customShutters.ItemAdded += OnCustomShuttersChanged;
			_customShutters.ItemRemoved += OnCustomShuttersChanged;
			_customShutters.ItemChanging += OnCustomShuttersChanged;
			_customShutters.ItemChanged += OnCustomShuttersChanged;

			_dicomShutters = new List<GeometricShutter>();
			_readOnlyDicomShutters = new ReadOnlyCollection<GeometricShutter>(_dicomShutters);

			base.Name = DefaultName;
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected GeometricShuttersGraphic(GeometricShuttersGraphic source, ICloningContext context)
			: this(source._imageRectangle.Height, source._imageRectangle.Width)
		{
			context.CloneFields(source, this);

			foreach (GeometricShutter shutter in source._customShutters)
				_customShutters.Add(shutter.Clone());

			foreach (GeometricShutter shutter in source._dicomShutters)
				_dicomShutters.Add(shutter.Clone());
		}

		internal void AddDicomShutter(GeometricShutter dicomShutter)
		{
			_dicomShutters.Add(dicomShutter);
			Invalidate();
		}

		private bool HasShutters
		{
			get { return _customShutters.Count > 0 || _dicomShutters.Count > 0; }
		}

		/// <summary>
		/// Gets a readonly collection of the <see cref="GeometricShutter"/>s.
		/// </summary>
		public ReadOnlyCollection<GeometricShutter> DicomShutters
		{
			get { return _readOnlyDicomShutters; }
		}

		/// <summary>
		/// Gets a list of custom display shutters.
		/// </summary>
		public ObservableList<GeometricShutter> CustomShutters
		{
			get { return _customShutters; }
		}

		/// <summary>
		/// Gets or sets the presentation color which should replace the shuttered pixels.
		/// </summary>
		public Color FillColor
		{
			get { return _fillColor; }
			set
			{
				if (_fillColor == value)
					return;

				_fillColor = value;
				Invalidate();
			}
		}

		/// <summary>
		/// Fires the <see cref="Graphic.Drawing"/> event.  Should be called by an <see cref="IRenderer"/>
		/// for each object just before it is drawn/rendered, hence the reason it is public.
		/// </summary>
		public override void OnDrawing()
		{
			base.OnDrawing();

			RenderImageGraphic();
		}

		/// <summary>
		/// Releases all resources used by this <see cref="CompositeGraphic"/>.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
				ReleaseBuffer();
		}

		private void RenderImageGraphic()
		{
			if (_imageGraphic != null || !HasShutters)
				return;

			CodeClock clock = new CodeClock();
			clock.Start();

			int stride = _imageRectangle.Width*4;
			int size = _imageRectangle.Height*stride;
			_buffer = MemoryManager.Allocate<byte>(size);
			Diagnostics.OnLargeObjectAllocated(_buffer.Length);

			GCHandle bufferHandle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);

			try
			{
				using (Bitmap bitmap = new Bitmap(_imageRectangle.Width, _imageRectangle.Height, stride, PixelFormat.Format32bppPArgb, bufferHandle.AddrOfPinnedObject()))
				{
					using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap))
					{
						graphics.Clear(Color.FromArgb(0, Color.Black));
						using (Brush brush = new SolidBrush(_fillColor))
						{
							foreach (GeometricShutter shutter in GetAllShutters())
							{
								using (GraphicsPath path = new GraphicsPath())
								{
									path.FillMode = FillMode.Alternate;
									path.AddRectangle(_imageRectangle);
									shutter.AddToGraphicsPath(path);
									path.CloseFigure();
									graphics.FillPath(brush, path);
								}
							}
						}
					}

					//NOTE: we are not doing this properly according to Dicom.  We should be rendering
					//to a 16-bit image so we can set the 16-bit p-value.
					_imageGraphic = new ColorImageGraphic(_imageRectangle.Height, _imageRectangle.Width, _buffer);
					base.Graphics.Add(_imageGraphic);
				}
			}
			finally
			{
				bufferHandle.Free();
			}

			clock.Stop();
			PerformanceReportBroker.PublishReport("Shutters", "Render", clock.Seconds);
		}

		private IEnumerable<GeometricShutter> GetAllShutters()
		{
			foreach (GeometricShutter shutter in _dicomShutters)
				yield return shutter;

			foreach (GeometricShutter shutter in _customShutters)
				yield return shutter;
		}

		private void Invalidate()
		{
			if (_imageGraphic != null)
			{
				base.Graphics.Remove(_imageGraphic);
				_imageGraphic.Dispose();
				_imageGraphic = null;

				//don't de-allocate and reallocate unnecessarily.
				if (!HasShutters)
					ReleaseBuffer();
			}
		}

		private void ReleaseBuffer()
		{
			if (_buffer != null)
				Diagnostics.OnLargeObjectReleased(_buffer.Length);

			_buffer = null;
		}

		private void OnCustomShuttersChanged(object sender, ListEventArgs<GeometricShutter> e)
		{
			Invalidate();
		}

		#region IShutterGraphic Members

		/// <summary>
		/// Gets or sets the 16-bit grayscale presentation value which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="IShutterGraphic.PresentationValue"/> and <see cref="IShutterGraphic.PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		ushort IShutterGraphic.PresentationValue
		{
			get { return 0; }
			set { }
		}

		/// <summary>
		/// Gets or sets the presentation color which should replace the shuttered pixels.
		/// </summary>
		/// <remarks>
		/// The display of shuttered pixels is no longer defined by the data source but is now
		/// implementation specific. The <see cref="IShutterGraphic.PresentationValue"/> and <see cref="IShutterGraphic.PresentationColor"/>
		/// properties allows this display to be customized by client code.
		/// </remarks>
		Color IShutterGraphic.PresentationColor
		{
			get { return this.FillColor; }
			set { this.FillColor = value; }
		}

		#endregion
	}
}