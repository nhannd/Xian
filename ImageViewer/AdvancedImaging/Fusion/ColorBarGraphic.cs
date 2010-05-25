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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	[Cloneable(true)]
	public class ColorBarGraphic : CompositeGraphic, IColorMapProvider, IColorMapInstaller
	{
		[CloneIgnore]
		private GrayscaleImageGraphic _colorBar;

		private ColorBarOrientation _orientation;
		private bool _invert;

		public ColorBarGraphic()
		{
			_orientation = ColorBarOrientation.Horizontal;
			_invert = false;
		}

		[OnCloneComplete]
		private void OnCloneComplete()
		{
			_colorBar = (GrayscaleImageGraphic) CollectionUtils.SelectFirst(base.Graphics, g => g is GrayscaleImageGraphic);
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
				}
			}
		}

		public bool Invert
		{
			get { return _invert; }
			set
			{
				if (_invert != value)
				{
					_invert = value;
					this.OnInvertChanged(EventArgs.Empty);
				}
			}
		}

		protected GrayscaleImageGraphic ColorBar
		{
			get
			{
				if (_colorBar == null)
				{
					base.Graphics.Add(_colorBar = CreateColorBar());
					this.UpdateColorBar();
				}
				return _colorBar;
			}
		}

		protected virtual void OnInvertChanged(EventArgs e)
		{
			this.UpdateColorBar();
		}

		protected virtual void OnOrientationChanged(EventArgs e)
		{
			this.UpdateColorBar();
		}

		protected virtual GrayscaleImageGraphic CreateColorBar()
		{
			const int barWidth = 30;
			const int pixelLength = 256*barWidth;
			var pixelData = new byte[pixelLength];
			for (int n = 0; n < pixelLength; n++)
				pixelData[n] = (byte) (n/barWidth);
			return new GrayscaleImageGraphic(256, barWidth, 8, 8, 7, false, false, 1, 0, pixelData);
		}

		private void UpdateColorBar()
		{
			if (_colorBar != null)
			{
				bool horizontal = _orientation == ColorBarOrientation.Horizontal;
				_colorBar.SpatialTransform.RotationXY = horizontal ? -90 : 0;
				_colorBar.SpatialTransform.TranslationX = horizontal ? -_colorBar.Columns : 0;
				_colorBar.VoiLutManager.Invert = _invert;
			}
		}

		public override void OnDrawing()
		{
			// ensure the colorbar is created
			var x = this.ColorBar;

			base.OnDrawing();
		}

		public enum ColorBarOrientation
		{
			Horizontal,
			Vertical
		}

		#region IColorMapProvider Members

		public IColorMapManager ColorMapManager
		{
			get { return this.ColorBar.ColorMapManager; }
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
	}
}