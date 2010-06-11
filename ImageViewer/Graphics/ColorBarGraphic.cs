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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	[Cloneable]
	public class ColorBarGraphic : CompositeGraphic, IColorMapProvider, IColorMapInstaller
	{
		[CloneIgnore]
		private readonly ColorMapManager _colorMapManagerProxy = new ColorMapManager(new ColorMapInstallerProxy());

		[CloneIgnore]
		private GrayscaleImageGraphic _colorBar;

		[CloneIgnore]
		private IGradientPixelData _gradientPixelData;

		private ColorBarOrientation _orientation;
		private PointF _location;
		private Size _size;
		private bool _reversed;

		public ColorBarGraphic()
			: this(ColorBarOrientation.Vertical) {}

		public ColorBarGraphic(ColorBarOrientation orientation)
			: this(125, 15, orientation) {}

		public ColorBarGraphic(int length, int width, ColorBarOrientation orientation)
			: this(orientation == ColorBarOrientation.Horizontal ? new Size(length, width) : new Size(width, length), orientation) {}

		public ColorBarGraphic(Size size)
			: this(size, size.Width > size.Height ? ColorBarOrientation.Horizontal : ColorBarOrientation.Vertical) {}

		public ColorBarGraphic(Size size, ColorBarOrientation orientation)
		{
			_size = size;
			_location = new PointF(0, 0);
			_orientation = orientation;
			_reversed = false;
			_gradientPixelData = null;
			_colorMapManagerProxy = new ColorMapManager(new ColorMapInstallerProxy());
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		/// <param name="source">The source object from which to clone.</param>
		/// <param name="context">The cloning context object.</param>
		protected ColorBarGraphic(ColorBarGraphic source, ICloningContext context)
		{
			context.CloneFields(source, this);

			_colorMapManagerProxy.SetMemento(source._colorMapManagerProxy.CreateMemento());

			if (source._gradientPixelData != null)
				_gradientPixelData = source._gradientPixelData.Clone();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_colorBar = null;

				if (_gradientPixelData != null)
				{
					_gradientPixelData.Dispose();
					_gradientPixelData = null;
				}
			}

			base.Dispose(disposing);
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_colorBar = (GrayscaleImageGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is GrayscaleImageGraphic);
		}

		public int Width
		{
			get { return _orientation == ColorBarOrientation.Horizontal ? _size.Height : _size.Width; }
			set { this.Size = _orientation == ColorBarOrientation.Horizontal ? new Size(_size.Width, value) : new Size(value, _size.Height); }
		}

		public int Length
		{
			get { return _orientation == ColorBarOrientation.Horizontal ? _size.Width : _size.Height; }
			set { this.Size = _orientation == ColorBarOrientation.Horizontal ? new Size(value, _size.Height) : new Size(_size.Width, value); }
		}

		public Size Size
		{
			get { return _size; }
			set
			{
				if (_size != value)
				{
					_size = value;
					this.OnSizeChanged(EventArgs.Empty);
					this.OnVisualStateChanged("Size");
				}
			}
		}

		public PointF Location
		{
			get
			{
				if (base.CoordinateSystem == CoordinateSystem.Source && base.ParentGraphic != null)
					return base.ParentGraphic.SpatialTransform.ConvertToDestination(_location);
				return _location;
			}
			set
			{
				if (base.CoordinateSystem == CoordinateSystem.Source && base.ParentGraphic != null)
					value = base.ParentGraphic.SpatialTransform.ConvertToDestination(value);
				if (_location != value)
				{
					_location = value;
					this.SpatialTransform.TranslationX = _location.X;
					this.SpatialTransform.TranslationY = _location.Y;
					this.OnLocationChanged(EventArgs.Empty);
				}
			}
		}

		public ColorBarOrientation Orientation
		{
			get { return _orientation; }
			set
			{
				if (_orientation != value)
				{
					_orientation = value;
					this.OnOrientationChanged(EventArgs.Empty);
					this.OnVisualStateChanged("Orientation");
				}
			}
		}

		public bool Reversed
		{
			get { return _reversed; }
			set
			{
				if (_reversed != value)
				{
					_reversed = value;
					this.OnReversedChanged(EventArgs.Empty);
					this.OnVisualStateChanged("Reversed");
				}
			}
		}

		protected GrayscaleImageGraphic ColorBar
		{
			get
			{
				if (_colorBar == null)
				{
					base.Graphics.Add(_colorBar = CreateVerticalGradient());
					this.UpdateColorBar();
				}
				return _colorBar;
			}
		}

		public override void Move(SizeF delta)
		{
			this.Location += delta;
		}

		protected virtual void OnLocationChanged(EventArgs e) {}

		protected virtual void OnSizeChanged(EventArgs e)
		{
			this.UnloadColorBar();
		}

		protected virtual void OnReversedChanged(EventArgs e)
		{
			this.UpdateColorBar();
		}

		protected virtual void OnOrientationChanged(EventArgs e)
		{
			this.UpdateColorBar();
		}

		protected virtual GrayscaleImageGraphic CreateVerticalGradient()
		{
			if (_gradientPixelData != null)
				_gradientPixelData.Dispose();
			_gradientPixelData = GradientPixelData.GetGradient(this.Length, this.Width);
			return new GrayscaleImageGraphic(_gradientPixelData.Length, _gradientPixelData.Width, 8, 8, 7, false, false, 1, 0, () => _gradientPixelData.Data);
		}

		protected virtual void UnloadColorBar()
		{
			if (_colorBar != null)
			{
				base.Graphics.Remove(_colorBar);
				_colorBar.Dispose();
				_colorBar = null;
			}

			if (_gradientPixelData != null)
			{
				_gradientPixelData.Dispose();
				_gradientPixelData = null;
			}
		}

		private void UpdateColorBar()
		{
			if (_colorBar != null)
			{
				bool horizontal = _orientation == ColorBarOrientation.Horizontal;
				_colorBar.SpatialTransform.RotationXY = horizontal ? -90 : 0;
				_colorBar.SpatialTransform.TranslationX = horizontal ? -_colorBar.Columns : 0;
				_colorBar.VoiLutManager.Invert = _reversed;
			}
		}

		protected override SpatialTransform CreateSpatialTransform()
		{
			return new InvariantSpatialTransform(this);
		}

		public override void OnDrawing()
		{
			// ensure the colorbar is created and the colormap is up to date
			var colorBar = this.ColorBar;
			colorBar.ColorMapManager.SetMemento(_colorMapManagerProxy.CreateMemento());

			base.OnDrawing();
		}

		#region IColorMapProvider Members

		public IColorMapManager ColorMapManager
		{
			get { return _colorMapManagerProxy; }
		}

		#endregion

		#region IColorMapInstaller Members

		public IDataLut ColorMap
		{
			get { return this.ColorMapManager.ColorMap; }
		}

		public void InstallColorMap(string name)
		{
			this.ColorMapManager.InstallColorMap(name);
		}

		public void InstallColorMap(ColorMapDescriptor descriptor)
		{
			this.ColorMapManager.InstallColorMap(descriptor);
		}

		public void InstallColorMap(IDataLut colorMap)
		{
			this.ColorMapManager.InstallColorMap(colorMap);
		}

		IEnumerable<ColorMapDescriptor> IColorMapInstaller.AvailableColorMaps
		{
			get { return this.ColorMapManager.AvailableColorMaps; }
		}

		#endregion

		#region GradientPixelData Class

		private interface IGradientPixelData : IDisposable
		{
			int Width { get; }
			int Length { get; }
			byte[] Data { get; }
			IGradientPixelData Clone();
		}

		private class GradientPixelData : IDisposable
		{
			private static readonly Dictionary<Size, GradientPixelData> _cachedGradients = new Dictionary<Size, GradientPixelData>();
			private readonly Size _normalizedSize;
			private byte[] _data;

			public static IGradientPixelData GetGradient(int length, int width)
			{
				var normalizedSize = new Size(width, length);
				if (_cachedGradients.ContainsKey(normalizedSize))
					return _cachedGradients[normalizedSize].CreateTransientReference();
				return new GradientPixelData(normalizedSize).CreateTransientReference();
			}

			private GradientPixelData(Size normalizedSize)
			{
				_normalizedSize = normalizedSize;
				_cachedGradients.Add(normalizedSize, this);
			}

			protected void Dispose(bool disposing)
			{
				if (disposing)
				{
					_data = null;
					_cachedGradients.Remove(_normalizedSize);
				}
			}

			public byte[] Data
			{
				get
				{
					if (_data == null)
					{
						int bufferSize = _normalizedSize.Height*_normalizedSize.Width;
						var buffer = new byte[bufferSize];
						for (int n = 0; n < bufferSize; n++)
							buffer[n] = (byte) (((int) (n/_normalizedSize.Width))*255f/(_normalizedSize.Height - 1));
						_data = buffer;
					}
					return _data;
				}
			}

			#region Transient Reference Support

			private class GradientReference : IGradientPixelData
			{
				private GradientPixelData _gradientPixelData;

				public GradientReference(GradientPixelData gradientPixelData)
				{
					_gradientPixelData = gradientPixelData;
					_gradientPixelData.OnReferenceCreated();
				}

				public int Length
				{
					get { return _gradientPixelData._normalizedSize.Height; }
				}

				public int Width
				{
					get { return _gradientPixelData._normalizedSize.Width; }
				}

				public byte[] Data
				{
					get { return _gradientPixelData.Data; }
				}

				public IGradientPixelData Clone()
				{
					return _gradientPixelData.CreateTransientReference();
				}

				public void Dispose()
				{
					if (_gradientPixelData != null)
					{
						_gradientPixelData.OnReferenceDisposed();
						_gradientPixelData = null;
					}
				}
			}

			private readonly object _syncLock = new object();
			private int _transientReferenceCount = 0;
			private bool _selfDisposed = false;

			private void OnReferenceDisposed()
			{
				lock (_syncLock)
				{
					if (_transientReferenceCount > 0)
						--_transientReferenceCount;

					if (_transientReferenceCount == 0 && _selfDisposed)
						DisposeInternal();
				}
			}

			private void OnReferenceCreated()
			{
				lock (_syncLock)
				{
					if (_transientReferenceCount == 0 && _selfDisposed)
						throw new ObjectDisposedException("");

					++_transientReferenceCount;
				}
			}

			private void DisposeInternal()
			{
				try
				{
					this.Dispose(true);
					GC.SuppressFinalize(this);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e);
				}
			}

			/// <summary>
			/// Creates a new 'transient reference' to this <see cref="FusionOverlayFrameData"/>.
			/// </summary>
			/// <remarks>See <see cref="IFusionOverlayFrameDataReference"/> for a detailed explanation of 'transient references'.</remarks>
			public IGradientPixelData CreateTransientReference()
			{
				return new GradientReference(this);
			}

			/// <summary>
			/// Implementation of the <see cref="IDisposable"/> pattern.
			/// </summary>
			public void Dispose()
			{
				lock (_syncLock)
				{
					_selfDisposed = true;

					//Only dispose for real when self has been disposed and all the transient references have been disposed.
					if (_transientReferenceCount == 0)
						DisposeInternal();
				}
			}

			#endregion
		}

		#endregion

		#region ColorMapInstallerProxy Class

		private class ColorMapInstallerProxy : IColorMapInstaller
		{
			private IDataLut _colorMap;
			private string _colorMapName = string.Empty;

			public IDataLut ColorMap
			{
				get
				{
					if (_colorMap == null && !string.IsNullOrEmpty(_colorMapName))
					{
						using (var lutFactory = LutFactory.Create())
						{
							_colorMap = lutFactory.GetColorMap(_colorMapName);
						}
					}
					return _colorMap;
				}
			}

			public void InstallColorMap(string name)
			{
				if (_colorMapName != name)
				{
					_colorMapName = name;
					_colorMap = null;
				}
			}

			public void InstallColorMap(ColorMapDescriptor descriptor)
			{
				this.InstallColorMap(descriptor.Name);
			}

			public void InstallColorMap(IDataLut colorMap)
			{
				if (_colorMap != colorMap)
				{
					_colorMap = colorMap;
					_colorMapName = null;
				}
			}

			public IEnumerable<ColorMapDescriptor> AvailableColorMaps
			{
				get
				{
					using (var lutFactory = LutFactory.Create())
					{
						return lutFactory.AvailableColorMaps;
					}
				}
			}
		}

		#endregion
	}

	public enum ColorBarOrientation
	{
		Horizontal,
		Vertical
	}
}